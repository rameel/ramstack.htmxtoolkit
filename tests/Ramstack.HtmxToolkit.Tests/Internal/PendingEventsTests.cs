using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests.Internal;

[TestFixture]
public class PendingEventsTests
{
    [Test]
    public void AddEvent_StoresEventsInOrder()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "b", "1");
        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "2");

        Assert.That(
            pending.GetEvents(HtmxTriggerTiming.Receive),
            Is.EqualTo([
                KeyValuePair.Create("b", "1"),
                KeyValuePair.Create("a", "2")]));
    }

    [Test]
    public void AddEvent_KeepsRepeatedNames()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"first\"");
        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"second\"");

        Assert.That(
            pending.GetEvents(HtmxTriggerTiming.Receive),
            Is.EqualTo([
                KeyValuePair.Create("message", "\"first\""),
                KeyValuePair.Create("message", "\"second\"")]));
    }

    [Test]
    public void AddEvent_WithRepeatedName_WritesAllEventsInOrderUnderProxy()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");
        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "2");
        pending.AddEvent(HtmxTriggerTiming.Receive, "b", "3");
        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "4");

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo(
                "{\"rs:event\":[" +
                "{\"key\":\"a\",\"value\":1}," +
                "{\"key\":\"a\",\"value\":2}," +
                "{\"key\":\"b\",\"value\":3}," +
                "{\"key\":\"a\",\"value\":4}]}"));
    }

    [Test]
    public void AddEvent_WithoutRepeatedNames_WritesEventsInOrder()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        foreach (var name in new[] { "g", "f", "e", "d", "c", "b", "a" })
            pending.AddEvent(HtmxTriggerTiming.Receive, name, "1");

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"g\":1,\"f\":1,\"e\":1,\"d\":1,\"c\":1,\"b\":1,\"a\":1}"));
    }

    [TestCase(8)]
    [TestCase(9)]
    [TestCase(20)]
    public void AddEvent_WithManyEvents_WritesRepeatedNameUnderProxy(int count)
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        for (var i = 1; i < count; i++)
            pending.AddEvent(HtmxTriggerTiming.Receive, "e" + i, "1");

        pending.AddEvent(HtmxTriggerTiming.Receive, "e1", "2");

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();

        Assert.That(header, Does.StartWith("{\"rs:event\":[{\"key\":\"e1\",\"value\":1},"));
        Assert.That(header, Does.EndWith(",{\"key\":\"e1\",\"value\":2}]}"));
    }

    [TestCase(8)]
    [TestCase(9)]
    [TestCase(20)]
    public void AddEvent_WithManyDistinctEvents_WritesProperties(int count)
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        for (var i = 1; i <= count; i++)
            pending.AddEvent(HtmxTriggerTiming.Receive, "e" + i, "1");

        var expected = "{" + string.Join(",", Enumerable.Range(1, count).Select(i => $"\"e{i}\":1")) + "}";

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo(expected));
    }

    [Test]
    public void AddEvent_TracksTimingsIndependently()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "r", "1");
        pending.AddEvent(HtmxTriggerTiming.AfterSettle, "t", "3");
        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "s", "2");

        Assert.That(pending.GetEvents(HtmxTriggerTiming.Receive), Is.EqualTo([KeyValuePair.Create("r", "1")]));
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSettle), Is.EqualTo([KeyValuePair.Create("t", "3")]));
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSwap), Is.EqualTo([KeyValuePair.Create("s", "2")]));
    }

    [TestCase(HtmxTargetVersion.V1)]
    [TestCase(HtmxTargetVersion.V2)]
    public void AddEvent_PriorVersions_WriteEachTimingToItsOwnHeader(HtmxTargetVersion targetVersion)
    {
        var context = TestHelper.CreateHtmxRequestContext(targetVersion);
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "received", "1");
        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "swapped", "2");
        pending.AddEvent(HtmxTriggerTiming.AfterSettle, "settled", "3");

        Assert.Multiple(() =>
        {
            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
                Is.EqualTo("{\"received\":1}"));

            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.TriggerAfterSwap].ToString(),
                Is.EqualTo("{\"swapped\":2}"));

            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.TriggerAfterSettle].ToString(),
                Is.EqualTo("{\"settled\":3}"));
        });
    }

    [Test]
    public void AddEvent_Htmx4_NormalizesAllTimingsToReceive()
    {
        var context = TestHelper.CreateHtmxRequestContext(HtmxTargetVersion.V4);
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "received", "1");
        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "swapped", "2");
        pending.AddEvent(HtmxTriggerTiming.AfterSettle, "settled", "3");

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.Multiple(() =>
        {
            Assert.That(
                events.Select(e => e.Key),
                Is.EqualTo(["received", "swapped", "settled"]));

            Assert.That(
                pending.GetEvents(HtmxTriggerTiming.AfterSwap),
                Is.SameAs(events));

            Assert.That(
                pending.GetEvents(HtmxTriggerTiming.AfterSettle),
                Is.SameAs(events));

            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
                Is.EqualTo("{\"received\":1,\"swapped\":2,\"settled\":3}"));

            Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSwap), Is.False);
            Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSettle), Is.False);
        });
    }

    [Test]
    public void AddEvent_Htmx4_PreservesDuplicatesAcrossRequestedTimings()
    {
        var context = TestHelper.CreateHtmxRequestContext(HtmxTargetVersion.V4);
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"first\"");
        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "message", "\"second\"");
        pending.AddEvent(HtmxTriggerTiming.AfterSettle, "message", "\"third\"");

        Assert.That(
            pending.GetEvents(HtmxTriggerTiming.Receive),
            Is.EqualTo([
                KeyValuePair.Create("message", "\"first\""),
                KeyValuePair.Create("message", "\"second\""),
                KeyValuePair.Create("message", "\"third\"")]));

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo(
                "{\"rs:event\":[" +
                "{\"key\":\"message\",\"value\":\"first\"}," +
                "{\"key\":\"message\",\"value\":\"second\"}," +
                "{\"key\":\"message\",\"value\":\"third\"}]}"));
    }

    [Test]
    public void GetEvents_ReturnsNull_WhenNothingRegistered()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        Assert.That(pending.GetEvents(HtmxTriggerTiming.Receive), Is.Null);
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSettle), Is.Null);
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSwap), Is.Null);
    }

    [Test]
    public void AddEvent_WritesEventDetailToHeader()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"hello\"");

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger];

        Assert.That(header.ToString(), Is.EqualTo("{\"message\":\"hello\"}"));
    }

    [Test]
    public void AddEvent_PreservesEventNameCase()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "ShowMessage", "1");

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"ShowMessage\":1}"));
    }

    [Test]
    public void AddEvent_PreservesEventNameCase_InReplayedEvents()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "ShowMessage", "1");
        pending.AddEvent(HtmxTriggerTiming.Receive, "ShowMessage", "2");

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"rs:event\":[{\"key\":\"ShowMessage\",\"value\":1},{\"key\":\"ShowMessage\",\"value\":2}]}"));
    }

    [Test]
    public void AddEvent_WritesEventNamesThatDifferOnlyInCaseSeparately()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "Message", "1");
        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "2");

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"Message\":1,\"message\":2}"));
    }

    [Test]
    public void AddEvent_WritesOnlyRegisteredTimings()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "swapped", "true");

        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Trigger), Is.False);
        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSwap), Is.True);
        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSettle), Is.False);
    }

    [Test]
    public void AddEvent_WrapsNullEventDetail()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "e", "null");

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();
        Assert.That(header, Is.EqualTo("{\"e\":{\"value\":null}}"));
    }

    [Test]
    public void AddEvent_WrapsNullEventDetail_InReplayedEvents()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "e", "null");
        pending.AddEvent(HtmxTriggerTiming.Receive, "e", "1");

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();
        Assert.That(header, Is.EqualTo("{\"rs:event\":[{\"key\":\"e\",\"value\":{\"value\":null}},{\"key\":\"e\",\"value\":1}]}"));
    }

    [Test]
    public void GetEvents_KeepsStoredNullEventDetail()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "e", "null");

        Assert.That(
            pending.GetEvents(HtmxTriggerTiming.Receive),
            Is.EqualTo([KeyValuePair.Create("e", "null")]));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void AddEvent_NullOrWhitespaceDetail_ThrowsArgumentException(string? detail)
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        var exception = Assert.Catch<ArgumentException>(() =>
            pending.AddEvent(HtmxTriggerTiming.Receive, "e", detail!));

        Assert.That(exception?.ParamName, Is.EqualTo("detailJson"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void AddEvent_NullOrWhitespaceEventName_ThrowsArgumentException(string? eventName)
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        var exception = Assert.Catch<ArgumentException>(() =>
            pending.AddEvent(HtmxTriggerTiming.Receive, eventName!, "1"));

        Assert.That(exception?.ParamName, Is.EqualTo("eventName"));
    }

    [Test]
    public void GetEvents_ReturnsReadOnlyView()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");

        var events = (ICollection<KeyValuePair<string, string>>)pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.That(events.IsReadOnly, Is.True);
        Assert.Throws<NotSupportedException>(() => events.Add(KeyValuePair.Create("b", "2")));
    }

    [Test]
    public void GetEvents_ReturnsLiveView()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");
        var events = pending.GetEvents(HtmxTriggerTiming.Receive);

        pending.AddEvent(HtmxTriggerTiming.Receive, "b", "2");

        Assert.That(
            events,
            Is.EqualTo([
                KeyValuePair.Create("a", "1"),
                KeyValuePair.Create("b", "2")]));
    }

    [Test]
    public void AddEvent_ResponseStarted_ThrowsInvalidOperationException()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "before", "1");
        TestHelper.StartResponse(context);

        Assert.Throws<InvalidOperationException>(() =>
            pending.AddEvent(HtmxTriggerTiming.Receive, "after", "2"));

        Assert.Multiple(() =>
        {
            Assert.That(
                pending.GetEvents(HtmxTriggerTiming.Receive),
                Is.EqualTo([KeyValuePair.Create("before", "1")]));

            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
                Is.EqualTo("{\"before\":1}"));
        });
    }

    [Test]
    public void AddEvent_AfterHeaderRemoved_DiscardsPreviousEvents()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");
        context.Response.Headers.Remove(HtmxResponseHeaderNames.Trigger);
        pending.AddEvent(HtmxTriggerTiming.Receive, "b", "2");

        Assert.Multiple(() =>
        {
            Assert.That(
                pending.GetEvents(HtmxTriggerTiming.Receive),
                Is.EqualTo([KeyValuePair.Create("b", "2")]));

            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
                Is.EqualTo("{\"b\":2}"));
        });
    }

    [Test]
    public void GetEvents_ReturnsNull_WhenHeaderRemoved()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");
        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "b", "2");
        context.Response.Headers.Remove(HtmxResponseHeaderNames.Trigger);

        Assert.That(pending.GetEvents(HtmxTriggerTiming.Receive), Is.Null);
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSwap), Is.EqualTo([KeyValuePair.Create("b", "2")]));
    }

    [Test]
    public void GetOrCreate_ReturnsSameInstance()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        Assert.That(pending, Is.SameAs(PendingEvents.GetOrCreate(context.Response)));
        Assert.That(PendingEvents.TryGet(context.Response), Is.SameAs(pending));
    }

    [Test]
    public void GetOrCreate_RegistersInHttpContextItems()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        Assert.That(context.Items[typeof(PendingEvents)], Is.SameAs(pending));
    }
}
