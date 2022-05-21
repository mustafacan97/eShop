using EventBus.Base.Abstraction;
using EventBus.Base.Models;
using EventBus.Base.Models.Enums;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
    public static class EventBusFactory
    {
        #region Public Methods

        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            return config.EventBusType switch
            {
                EventBusType.RabbitMQ => new EventBusRabbitMQ(config, serviceProvider),
                _ => throw new NotImplementedException()
            };
        }

        #endregion
    }
}
