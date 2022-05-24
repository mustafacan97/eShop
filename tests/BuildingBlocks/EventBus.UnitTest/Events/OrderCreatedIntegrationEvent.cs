using EventBus.Base.Events;
using System;

namespace EventBus.UnitTest.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public OrderCreatedIntegrationEvent() : base()
        {
        }
    }
}
