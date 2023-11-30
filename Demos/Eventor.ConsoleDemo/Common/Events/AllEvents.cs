using Eventor.Core.Common.Seeds;

namespace Eventor.ConsoleDemo.Common.Events;

public class BasicEvent(string senderName)              : EventBase(senderName) { };
public class AnotherBasicEvent(string senderName)       : EventBase(senderName) { };
public class YetAnotherBasicEvent(string senderName)    : EventBase(senderName) { };

public class CustomPublisherEvent(string senderName)    : EventBase(senderName) { };

public class OrderProcessedEvent : EventBase
{
    public Guid OrderID { get;}
    public OrderProcessedEvent(string senderName, Guid orderID) : base(senderName) 
    {
        OrderID = orderID;
    }
}


