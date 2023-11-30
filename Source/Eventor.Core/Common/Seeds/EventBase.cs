namespace Eventor.Core.Common.Seeds;

/// <summary>
/// Base class that all events must implement in order to be recognised by the EventAggregator.
/// </summary>
public abstract class EventBase
{
    public string SenderName        { get; }
    public Int64  PublishTimeTicks  { get; internal set; }

    public EventBase(string senderName) => SenderName = senderName;

}
