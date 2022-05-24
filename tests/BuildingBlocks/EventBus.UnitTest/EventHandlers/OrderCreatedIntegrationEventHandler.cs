using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events;
using System.Threading.Tasks;

namespace EventBus.UnitTest.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
