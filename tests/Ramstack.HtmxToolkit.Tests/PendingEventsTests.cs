using System.Text.Json;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class PendingEventsTests
{
    [Test]
    public void AddEvent_StoresDistinctKeys()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");
        pending.AddEvent(HtmxTriggerTiming.Receive, "b", "2");

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events["a"], Is.EqualTo("1"));
        Assert.That(events["b"], Is.EqualTo("2"));
    }

    [Test]
    public void AddEvent_AccumulatesDuplicateKeys_UnderProxy()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"first\"");
        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"second\"");

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events["message"], Is.EqualTo("\"first\""));
        Assert.That(events["rs:event"], Is.EqualTo([KeyValuePair.Create("message", "\"second\"")]));

        pending.Flush();

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"message\":\"first\",\"rs:event\":[{\"key\":\"message\",\"value\":\"second\"}]}"));
    }

    [Test]
    public void AddEvent_TracksTimingsIndependently()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "r", "1");
        pending.AddEvent(HtmxTriggerTiming.AfterSettle, "t", "3");
        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "s", "2");

        Assert.That(pending.GetEvents(HtmxTriggerTiming.Receive), Is.EqualTo(CreateDictionary(("r", "1"))));
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSettle), Is.EqualTo(CreateDictionary(("t", "3"))));
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSwap), Is.EqualTo(CreateDictionary(("s", "2"))));
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
        pending.Flush();

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
        pending.Flush();

        Assert.Multiple(() =>
        {
            Assert.That(events.Keys, Is.EqualTo(["received", "swapped", "settled"]));
            Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSwap), Is.SameAs(events));
            Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSettle), Is.SameAs(events));

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

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.Multiple(() =>
        {
            Assert.That(events["message"], Is.EqualTo("\"first\""));
            Assert.That(
                events["rs:event"],
                Is.EqualTo(
                [
                    KeyValuePair.Create("message", "\"second\""),
                    KeyValuePair.Create("message", "\"third\"")
                ]));
        });

        pending.Flush();

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"message\":\"first\",\"rs:event\":[{\"key\":\"message\",\"value\":\"second\"},{\"key\":\"message\",\"value\":\"third\"}]}"));
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
    public void Flush_WritesCamelCaseJsonToHeaders()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "message", "\"hello\"");

        pending.Flush();

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger];

        Assert.That(header.ToString(), Is.EqualTo("{\"message\":\"hello\"}"));
    }

    [Test]
    public void Flush_WritesOnlyRegisteredTimings()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.AfterSwap, "swapped", "true");

        pending.Flush();

        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Trigger), Is.False);
        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSwap), Is.True);
        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSettle), Is.False);
    }

    [Test]
    public void Flush_SerializesNullEventDetail_AsNull()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "e", "null");
        pending.Flush();

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();
        Assert.That(header, Is.EqualTo("{\"e\":null}"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void AddEvent_NullOrWhitespaceDetail_NormalizesToEmptyObject(string? detail)
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "e", detail);
        pending.Flush();

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();
        Assert.That(header, Is.EqualTo("{\"e\":{}}"));
    }

    [Test]
    public void AddEvent_NullEventName_ThrowsArgumentNullException()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        Assert.Throws<ArgumentNullException>(() =>
            pending.AddEvent(HtmxTriggerTiming.Receive, null!, "1"));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void AddEvent_EmptyOrWhitespaceEventName_ThrowsArgumentException(string eventName)
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        var exception = Assert.Throws<ArgumentException>(() =>
            pending.AddEvent(HtmxTriggerTiming.Receive, eventName, "1"));

        Assert.That(exception?.ParamName, Is.EqualTo("eventName"));
    }

    [Test]
    public void GetEvents_MutatingReturnedView_BreaksFlush()
    {
        // The returned dictionary is a live inspection-only view: writing a value
        // that is not a serialized JSON fragment corrupts the pending events.
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "a", "1");

        var mutable = (IDictionary<string, object>)pending.GetEvents(HtmxTriggerTiming.Receive)!;
        mutable["a"] = 2;

        Assert.Throws<InvalidCastException>(pending.Flush);
    }

    [Test]
    public void Flush_RejectsInvalidJsonDetail()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvent(HtmxTriggerTiming.Receive, "e", "not-json");

        Assert.Catch<JsonException>(pending.Flush);
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

    private static Dictionary<string, string> CreateDictionary(params (string, string)[] parameters)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var (k, v) in parameters)
            dictionary[k] = v;

        return dictionary;
    }
}
