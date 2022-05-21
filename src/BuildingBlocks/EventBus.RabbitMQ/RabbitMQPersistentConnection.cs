using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {
        #region Constants and Fields

        private readonly IConnectionFactory _connectionFactory;

        private readonly int _retryCount;

        private IConnection connection;

        private object lockObject = new object();

        private bool _disposed;

        #endregion

        #region Public Properties

        public int RetryCount { get; }

        #endregion

        #region Constructors and Destructors

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            _connectionFactory = connectionFactory;
            _retryCount = retryCount;
        }

        #endregion

        #region Public Methods

        public bool IsConnected => connection != null && connection.IsOpen;        

        public IModel CreateModel()
        {
            return connection.CreateModel();
        }

        public void Dispose()
        {
            connection?.Dispose();
            _disposed = true;
        }

        public bool TryConnect()
        {
            lock (lockObject)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                    });

                policy.Execute(() =>
                {
                    connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    connection.CallbackException += Connection_CallbackException;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Methods

        private void Connection_ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed)
                return;
 
            TryConnect();
        }

        private void Connection_CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed)
                return;

            TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (_disposed)
                return;

            TryConnect();
        }

        #endregion
    }
}
