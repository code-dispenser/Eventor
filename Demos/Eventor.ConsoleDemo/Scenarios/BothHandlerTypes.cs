using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Scenarios
{
    public class BothHandlerTypes(IEventAggregator eventAggregator)
    {
        private readonly IEventAggregator _eventAggregator = eventAggregator;

        public async Task RunHandlersUsingFireAndForget()
        {
            var orderProcessedEvent = new OrderProcessedEvent(nameof(RunHandlersUsingFireAndForget), Guid.NewGuid());
            var orderSubscription   = _eventAggregator.Subscribe<OrderProcessedEvent>(LocalOrderHandler);

            await Console.Out.WriteLineAsync($"Publishing the event {nameof(OrderProcessedEvent)}.\r\n");
            await _eventAggregator.Publish(orderProcessedEvent);
        }

        public async Task LocalOrderHandler(OrderProcessedEvent theEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Handling the event {nameof(OrderProcessedEvent)} for order: {theEvent.OrderID}\r\n in the local event handler: {nameof(LocalOrderHandler)}\r\n");
        }
    }
}
