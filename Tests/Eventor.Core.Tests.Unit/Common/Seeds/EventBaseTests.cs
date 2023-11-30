using Eventor.Core.Common.Seeds;
using Eventor.Core.Tests.FixturesAndData.Events;
using FluentAssertions;
using Xunit;

namespace Eventor.Core.Tests.Unit.Common.Seeds;

public class EventBaseTests
{
    [Fact]
    public void Event_base_properties_should_return_their_values()
    {
        var theEvent = new BasicEvent("SenderName");

        theEvent.Should().BeAssignableTo<EventBase>().And.Match<BasicEvent>(e => e.SenderName == "SenderName");
    }
}
