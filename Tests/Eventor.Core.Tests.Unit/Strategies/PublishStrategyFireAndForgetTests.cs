using Eventor.Core.Common.Seeds;
using Eventor.Core.Strategies;
using Eventor.Core.Tests.FixturesAndData.Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Eventor.Core.Tests.Unit.Strategies;

public class PublishStrategyFireAndForgetTests
{
    private int     _handledCount       = 0;
    private int     HandledCount        => _handledCount;
    private void    IncrementCount()    => Interlocked.Increment(ref _handledCount);

    [Fact]
    public async Task Should_not_propagate_any_unhandled_errors_in_the_event_handlers()
    {
        var publishStrategy = new PublishStrategyFireAndForget();
        var handlerList     = new List<TheEventHandler<BasicEvent>> { HandleBasicEvent, HandleBasicEvent2 };
        var theEvent        = new BasicEvent("TheTester");

        Exception? exception = null;

        try
        {
            await publishStrategy.Publish(theEvent, handlerList, CancellationToken.None);
            await Task.Delay(20);// fire and forget so need to wait other wise we will just exit straight away
        }
        catch (Exception ex) { exception = ex; }

        using(new AssertionScope())
        {
            exception.Should().BeNull();
            this.HandledCount.Should().Be(2);
        }

        Task HandleBasicEvent(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            throw new Exception();
        }
        async Task HandleBasicEvent2(BasicEvent theBasicEvent, CancellationToken cancellationToken)
        {
            this.IncrementCount();
            await Task.Delay(1,cancellationToken);
            throw new Exception();

        }

    }
}
