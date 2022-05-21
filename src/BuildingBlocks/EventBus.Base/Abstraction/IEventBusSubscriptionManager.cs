using EventBus.Base.Events;
using EventBus.Base.Models;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        #region Public Properties

        bool IsEmpty { get; }

        #endregion

        #region Public Methods        

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();

        #endregion
    }
}
