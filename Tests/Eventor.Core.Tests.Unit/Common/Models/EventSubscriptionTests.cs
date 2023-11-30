using Eventor.Core.Common.Models;
using FluentAssertions;
using Xunit;

namespace Eventor.Core.Tests.Unit.Common.Models;

public class EventSubscriptionTests
{
    [Fact]
    public void A_null_action_should_not_throw_an_exception_when_attempting_to_dispose_of_the_event()

        => FluentActions.Invoking(() => new EventSubscription(null!, null!).Dispose()).Should().NotThrow();
    
}
