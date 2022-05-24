using EventBus.Base.Models.Enums;

namespace EventBus.Base.Models
{
    public class EventBusConfig
    {
        #region Public Properties

        public int ConnectionRetryCount { get; set; } = 5;

        public string DefaultTopicName { get; set; } = "eShopEventBus";

        public string EventBusConnectionString { get; set; } = string.Empty;

        public string SubscriberClientAppName { get; set; } = string.Empty;

        public string EventNamePrefix { get; set; } = string.Empty;

        public string EventNameSuffix { get; set; } = "IntegrationEvent";

        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;

        public object Connection { get; set; }

        #endregion

        #region Public Methods

        public bool DeleteEventPrefix => !string.IsNullOrEmpty(EventNamePrefix);

        public bool DeleteEventSuffix => !string.IsNullOrEmpty(EventNameSuffix);

        #endregion
    }
}
