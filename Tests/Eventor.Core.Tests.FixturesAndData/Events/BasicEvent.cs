using Eventor.Core.Common.Seeds;

namespace Eventor.Core.Tests.FixturesAndData.Events;

public class BasicEvent : EventBase
{
    public BasicEvent(string senderName) : base(senderName) { }
       

}
