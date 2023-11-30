using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventor.ConsoleDemo.Common.DynamicEventHandlers
{
    public class OrderProcessedEventHandler : IEventHandler<OrderProcessedEvent>
    {
        public async Task Handle(OrderProcessedEvent theEvent, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync($"Handling the event {nameof(OrderProcessedEvent)} for order: {theEvent.OrderID}\r\n in the dynamic event handler: {nameof(OrderProcessedEventHandler)}\r\n");
        }
    }
}
