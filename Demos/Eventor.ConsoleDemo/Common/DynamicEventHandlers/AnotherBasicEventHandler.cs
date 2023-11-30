using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Common.DynamicEventHandlers;

public class AnotherBasicEventHandler : IEventHandler<AnotherBasicEvent>
{
    public async Task Handle(AnotherBasicEvent theEvent, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync($"Handling the event {nameof(AnotherBasicEvent)} in the dynamic event handler: {nameof(AnotherBasicEventHandler)}\r\n");
    }
}
