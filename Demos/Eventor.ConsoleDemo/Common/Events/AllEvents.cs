using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Common.Events;

public class BasicEvent(string senderName)              : EventBase(senderName) { };
public class AnotherBasicEvent(string senderName)       : EventBase(senderName) { };
public class YetAnotherBasicEvent(string senderName)    : EventBase(senderName) { };

public class CustomPublisherEvent(string senderName)    : EventBase(senderName) { };

public class OrderProcessedEvent(string senderName, Guid orderID) : EventBase(senderName)
{
    public Guid OrderID { get; } = orderID;
}


