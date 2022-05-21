using EventBus.Base.Events;
using EventBus.Base.Models;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        #region Constants and Fields

        private RabbitMQPersistentConnection _persistentConnection;

        private readonly IConnectionFactory _connectionFactory;

        private readonly IModel _consumerChannel;

        #endregion

        #region Constructors and Destructors

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (config.Connection != null)
            {
                var connJson = JsonSerializer.Serialize(config, new JsonSerializerOptions()
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                
                _connectionFactory = JsonSerializer.Deserialize<ConnectionFactory>(connJson);
            }
            else            
                _connectionFactory = new ConnectionFactory();

            _persistentConnection = new RabbitMQPersistentConnection(_connectionFactory, config.ConnectionRetryCount);

            _consumerChannel = CreateConsumerChannel();

            _subscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
        }

        #endregion

        #region Public Methods

        public override void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_config.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    // Logging
                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            _consumerChannel.ExchangeDeclare(exchange: _config.DefaultTopicName, type: "direct");

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = _consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName), durable: true, exclusive: false, autoDelete: false, arguments: null);

                _consumerChannel.BasicPublish(
                    exchange: _config.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName), durable: true, exclusive: false,
                     autoDelete: false, arguments: null);

                _consumerChannel.QueueBind(queue: GetSubName(eventName), exchange: _config.DefaultTopicName, routingKey: eventName);
            }

            _subscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void UnSubscribe<T, TH>()
        {
            _subscriptionManager.RemoveSubscription<T, TH>();
        }

        #endregion

        #region Methods

        private void SubscriptionManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!_persistentConnection.IsConnected)            
                _persistentConnection.TryConnect();

            _consumerChannel.QueueUnbind(queue: eventName, exchange: _config.DefaultTopicName, routingKey: eventName);

            if (_subscriptionManager.IsEmpty)
                _consumerChannel.Close();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: _config.DefaultTopicName, type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (_consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(queue: GetSubName(eventName), autoAck: false, consumer: consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception e)
            {
                // Logging
            }

            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        #endregion
    }
}
