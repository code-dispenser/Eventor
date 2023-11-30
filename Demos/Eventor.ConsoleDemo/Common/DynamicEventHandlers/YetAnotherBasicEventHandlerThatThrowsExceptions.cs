using Eventor.ConsoleDemo.Common.Events;
using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Common.DynamicEventHandlers;

public class YetAnotherBasicEventHandlerThatThrowsExceptions : IEventHandler<YetAnotherBasicEvent>
{
    public async Task Handle(YetAnotherBasicEvent theEvent, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync($"Inside the dynamic event handler: {nameof(YetAnotherBasicEventHandlerThatThrowsExceptions)} and throwing exceptions.");
        throw new NotImplementedException();
    }
}
