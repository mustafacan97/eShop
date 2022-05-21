using EventBus.Base.Abstraction;
using EventBus.Base.Models;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        #region Constants and Fields

        private EventBusConfig _config;

        #endregion

        #region Public Properties

        public readonly IServiceProvider _serviceProvider;

        public readonly IEventBusSubscriptionManager _subscriptionManager;

        #endregion

        #region Constructors and Destructors

        public BaseEventBus(
            EventBusConfig config,
            IServiceProvider serviceProvider)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _subscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }

        #endregion

        #region Public Methods

        public virtual string ProcessEventName(string eventName)
        {
            if (_config.DeleteEventPrefix)
                eventName = eventName.TrimStart(_config.EventNamePrefix.ToArray());

            if (_config.DeleteEventSuffix)
                eventName = eventName.TrimEnd(_config.EventNameSuffix.ToArray());

            return eventName;
        }

        public virtual string GetSubName(string eventName)
        {
            return $"{_config.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public virtual void Dispose()
        {
            _config = null;
        }

        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);

            var processed = false;

            if (_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
                using (var scope = _serviceProvider.CreateScope())
                {
                    foreach (var subscription in subscriptions)
                    {
                        var handler = _serviceProvider.GetService(subscription.HandlerType);
                        if (handler == null)
                            continue;

                        var eventType = _subscriptionManager.GetEventTypeByName($"{_config.EventNamePrefix}{eventName}{_config.EventNameSuffix}");
                        var integrationEvent = JsonSerializer.Serialize(message, eventType);

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }

                processed = true;
            }

            return processed;
        }

        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        #endregion
    }
}
