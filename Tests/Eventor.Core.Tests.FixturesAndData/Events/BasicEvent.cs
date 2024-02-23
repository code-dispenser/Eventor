using Eventor.Core.Common.Seeds;

namespace Eventor.Core.Tests.FixturesAndData.Events;

public class BasicEvent(string senderName) : EventBase(senderName) {}
