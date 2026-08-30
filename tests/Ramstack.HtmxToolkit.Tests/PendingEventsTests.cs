using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class PendingEventsTests
{
    [Test]
    public void AddEvents_StoresDistinctKeys()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(
            HtmxTriggerTiming.Receive,
            CreateDictionary(("a", 1), ("b", 2)));

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events["a"], Is.EqualTo(1));
        Assert.That(events["b"], Is.EqualTo(2));
    }

    [Test]
    public void AddEvents_AccumulatesDuplicateKeys_UnderProxy()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(
            HtmxTriggerTiming.Receive,
            CreateDictionary(("message", "first")));

        pending.AddEvents(
            HtmxTriggerTiming.Receive,
            CreateDictionary(("message", "second")));

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events["message"], Is.EqualTo("first"));
        Assert.That(events["rs:events"], Is.EqualTo(new[] { KeyValuePair.Create("message", "second") }));
    }

    [Test]
    public void AddEvents_TracksTimingsIndependently()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(HtmxTriggerTiming.Receive, CreateDictionary(("r", 1)));
        pending.AddEvents(HtmxTriggerTiming.AfterSettle, CreateDictionary(("t", 3)));
        pending.AddEvents(HtmxTriggerTiming.AfterSwap, CreateDictionary(("s", 2)));

        Assert.That(pending.GetEvents(HtmxTriggerTiming.Receive), Is.EqualTo(CreateDictionary(("r", 1))));
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSettle), Is.EqualTo(CreateDictionary(("t", 3))));
        Assert.That(pending.GetEvents(HtmxTriggerTiming.AfterSwap), Is.EqualTo(CreateDictionary(("s", 2))));
    }

    [TestCase(HtmxTargetVersion.V1)]
    [TestCase(HtmxTargetVersion.V2)]
    public void AddEvents_LegacyVersions_WriteEachTimingToItsOwnHeader(HtmxTargetVersion targetVersion)
    {
        var context = TestHelper.CreateHtmxRequestContext(targetVersion);
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(HtmxTriggerTiming.Receive, CreateDictionary(("received", 1)));
        pending.AddEvents(HtmxTriggerTiming.AfterSwap, CreateDictionary(("swapped", 2)));
        pending.AddEvents(HtmxTriggerTiming.AfterSettle, CreateDictionary(("settled", 3)));
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
    public void AddEvents_Htmx4_NormalizesAllTimingsToReceive()
    {
        var context = TestHelper.CreateHtmxRequestContext(HtmxTargetVersion.V4);
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(HtmxTriggerTiming.Receive, CreateDictionary(("received", 1)));
        pending.AddEvents(HtmxTriggerTiming.AfterSwap, CreateDictionary(("swapped", 2)));
        pending.AddEvents(HtmxTriggerTiming.AfterSettle, CreateDictionary(("settled", 3)));

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
    public void AddEvents_Htmx4_PreservesDuplicatesAcrossRequestedTimings()
    {
        var context = TestHelper.CreateHtmxRequestContext(HtmxTargetVersion.V4);
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(HtmxTriggerTiming.Receive, CreateDictionary(("message", "first")));
        pending.AddEvents(HtmxTriggerTiming.AfterSwap, CreateDictionary(("message", "second")));
        pending.AddEvents(HtmxTriggerTiming.AfterSettle, CreateDictionary(("message", "third")));

        var events = pending.GetEvents(HtmxTriggerTiming.Receive)!;

        Assert.Multiple(() =>
        {
            Assert.That(events["message"], Is.EqualTo("first"));
            Assert.That(events["rs:events"], Is.EqualTo(new[]
            {
                KeyValuePair.Create<string, object>("message", "second"),
                KeyValuePair.Create<string, object>("message", "third")
            }));
        });
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
    public void SetEvents_ReplacesExisting()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(HtmxTriggerTiming.Receive, CreateDictionary(("old", 1)));
        pending.SetEvents(HtmxTriggerTiming.Receive, CreateDictionary(("new", 2)));

        Assert.That(
            pending.GetEvents(HtmxTriggerTiming.Receive),
            Is.EqualTo(CreateDictionary(("new", 2))));
    }

    [Test]
    public void Flush_WritesCamelCaseJsonToHeaders()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(
            HtmxTriggerTiming.Receive,
            CreateDictionary(("message", "hello")));

        pending.Flush();

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger];

        Assert.That(header.ToString(), Is.EqualTo("{\"message\":\"hello\"}"));
    }

    [Test]
    public void Flush_WritesOnlyRegisteredTimings()
    {
        var context = TestHelper.CreateHttpContext();
        var pending = PendingEvents.GetOrCreate(context.Response);

        pending.AddEvents(
            HtmxTriggerTiming.AfterSwap,
            CreateDictionary(("swapped", true)));

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

        pending.AddEvents(HtmxTriggerTiming.Receive, CreateDictionary(("e", null!)));
        pending.Flush();

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();
        Assert.That(header, Is.EqualTo("{\"e\":null}"));
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

    private static Dictionary<string, object> CreateDictionary(params (string, object)[] parameters)
    {
        var dictionary = new Dictionary<string, object>();
        foreach (var (k, v) in parameters)
            dictionary[k] = v;

        return dictionary;
    }
}
