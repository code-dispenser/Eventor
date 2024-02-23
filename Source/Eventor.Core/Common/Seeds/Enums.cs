namespace Eventor.Core.Common.Seeds;

/// <summary>
/// The PublishMethod enumeration is used to select one of the built-in publishers.
/// FireAndForget does not propagate any unhandled exceptions in the handlers, whilst WaitForAll will await
/// for Task.WhenAll to complete and return an AggregateException containing any unhandled exceptions from any handlers.
/// </summary>
public enum PublishMethod : int
{
    FireAndForget = 0,
    WaitForAll    = 1
}

