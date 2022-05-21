using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IIntegrationEventHandler<T> where T : IntegrationEvent
    {
        #region Public Methods

        Task<T> Handle(T @event);

        #endregion
    }
}
