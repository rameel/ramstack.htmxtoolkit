using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxResponseTests
{
    [Test]
    public void Location_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/foo"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Location], Is.EqualTo("/foo"));
    }

    [Test]
    public void Location_WithOptions_SerializesJson()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new HtmxLocationOptions
        {
            Source = "button",
            Target = "#content",
            Swap = HtmxSwap.OuterHtml,
            Select = "#list"
        }));

        var header = context.Response.Headers[HtmxResponseHeaderNames.Location].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["path"].GetString(), Is.EqualTo("/bar"));
        Assert.That(json["source"].GetString(), Is.EqualTo("button"));
        Assert.That(json["target"].GetString(), Is.EqualTo("#content"));
        Assert.That(json["swap"].GetString(), Is.EqualTo("outerHTML"));
        Assert.That(json["select"].GetString(), Is.EqualTo("#list"));
    }

    [Test]
    public void Location_WithOptions_OmitsNullProperties()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new HtmxLocationOptions()));

        var header = context.Response.Headers[HtmxResponseHeaderNames.Location].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["path"].GetString(), Is.EqualTo("/bar"));
        Assert.That(json.ContainsKey("source"), Is.False);
        Assert.That(json.ContainsKey("swap"), Is.False);
    }

    [Test]
    public void Location_WithOptions_SerializesValuesAndHeaders()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new HtmxLocationOptions
        {
            Values = new Dictionary<string, HtmxFieldValues>
            {
                ["id"] = "42",
                ["tags"] = ["dotnet", "web"]
            },
            Headers = new Dictionary<string, string>
            {
                ["X-Test"] = "abc"
            }
        }));

        var header = context.Response.Headers[HtmxResponseHeaderNames.Location].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["values"].GetProperty("id").GetString(), Is.EqualTo("42"));
        Assert.That(json["values"].GetProperty("tags").GetRawText(), Is.EqualTo("[\"dotnet\",\"web\"]"));
        Assert.That(json["headers"].GetProperty("X-Test").GetString(), Is.EqualTo("abc"));
    }

    [Test]
    public void Location_WithOptions_SerializesHistoryAndOutOfBandOptions()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new HtmxLocationOptions
        {
            SelectOob = "#alerts",
            Push = "false",
            Replace = "/replaced"
        }));

        var header = context.Response.Headers[HtmxResponseHeaderNames.Location].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["selectOOB"].GetString(), Is.EqualTo("#alerts"));
        Assert.That(json["push"].GetString(), Is.EqualTo("false"));
        Assert.That(json["replace"].GetString(), Is.EqualTo("/replaced"));
    }

    [Test]
    public void PushUrl_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.PushUrl("/foo"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.PushUrl], Is.EqualTo("/foo"));
    }

    [Test]
    public void PreventPushUrl_SetsFalse()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.PreventPushUrl());

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.PushUrl], Is.EqualTo("false"));
    }

    [Test]
    public void Redirect_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Redirect("/foo"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Redirect], Is.EqualTo("/foo"));
    }

    [Test]
    public void Refresh_SetsTrue()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Refresh());

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Refresh], Is.EqualTo("true"));
    }

    [Test]
    public void ReplaceUrl_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.ReplaceUrl("/foo"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.ReplaceUrl], Is.EqualTo("/foo"));
    }

    [Test]
    public void PreventReplaceUrl_SetsFalse()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.PreventReplaceUrl());

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.ReplaceUrl], Is.EqualTo("false"));
    }

    [Test]
    public void Reswap_Enum_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Reswap(HtmxSwap.BeforeBegin));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Reswap], Is.EqualTo("beforebegin"));
    }

    [Test]
    public void Reswap_String_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Reswap("innerHTML show:top"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Reswap], Is.EqualTo("innerHTML show:top"));
    }

    [Test]
    public void Retarget_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Retarget("#content"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Retarget], Is.EqualTo("#content"));
    }

    [Test]
    public void Reselect_SetsHeader()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Reselect("#list"));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Reselect], Is.EqualTo("#list"));
    }

    [Test]
    public void TriggerEvent_SetsReceiveTrigger()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("notify", "hello"));

        var events = context.Response.GetHtmxHeaders().Trigger!;
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events["notify"], Is.EqualTo("\"hello\""));
    }

    [Test]
    public void TriggerEvent_WithTiming_SetsAfterSettleTrigger()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("done", HtmxTriggerTiming.AfterSettle));

        var events = context.Response.GetHtmxHeaders().TriggerAfterSettle!;

        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events["done"], Is.EqualTo("{}"));
    }

    [Test]
    public void TriggerEvent_WithComplexDetail_SerializesJson()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("message", new { Id = 42, Text = "hi" }));

        PendingEvents.GetOrCreate(context.Response).Flush();

        var header = context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["message"].GetProperty("id").GetInt32(), Is.EqualTo(42));
        Assert.That(json["message"].GetProperty("text").GetString(), Is.EqualTo("hi"));
    }

    [Test]
    public void TriggerEvent_WithIndentedJsonTypeInfo_SerializesImmediatelyAsCompactJson()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        var detail = new TriggerEventDetail { Text = "before" };

        context.Response.Htmx(static (htmx, state) =>
        {
            var info = TriggerEventJsonSerializerContext.Default.TriggerEventDetail;
            htmx.TriggerEvent("ModelChanged", state, info);
        }, detail);

        detail.Text = "after";

        var events = context.Response.GetHtmxHeaders().Trigger!;
        PendingEvents.GetOrCreate(context.Response).Flush();

        Assert.Multiple(() =>
        {
            Assert.That(events["ModelChanged"], Is.EqualTo("{\"text\":\"before\"}"));
            Assert.That(
                context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
                Is.EqualTo("{\"modelChanged\":{\"text\":\"before\"}}"));
        });
    }

    [Test]
    public void TriggerEvent_NullObjectDetail_SerializesAsNull()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("e", (object?)null!));

        PendingEvents.GetOrCreate(context.Response).Flush();

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"e\":null}"));
    }

    [Test]
    public void TriggerEvent_EmptyStringDetail_SerializesAsEmptyString()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("e", ""));

        PendingEvents.GetOrCreate(context.Response).Flush();

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Trigger].ToString(),
            Is.EqualTo("{\"e\":\"\"}"));
    }

    [Test]
    public void TriggerEvent_NullJsonTypeInfo_ThrowsArgumentNullException()
    {
        var context = TestHelper.CreateHtmxRequestContext();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            context.Response.Htmx(static htmx => htmx.TriggerEvent("e", new TriggerEventDetail(), null!)));

        Assert.That(exception?.ParamName, Is.EqualTo("jsonTypeInfo"));
    }

    [Test]
    public void TriggerEvent_ReservedProxyEventName_ThrowsArgumentException()
    {
        var context = TestHelper.CreateHtmxRequestContext();

        var exception = Assert.Throws<ArgumentException>(() =>
            context.Response.Htmx(static htmx => htmx.TriggerEvent("rs:event")));

        Assert.That(exception?.ParamName, Is.EqualTo("eventName"));
    }

    [Test]
    public void TriggerEvent_Htmx4_AddsEveryTimingToReceiveTrigger()
    {
        var context = TestHelper.CreateHtmxRequestContext(HtmxTargetVersion.V4);

        context.Response.Htmx(r => r
            .TriggerEvent("received")
            .TriggerEvent("swapped", HtmxTriggerTiming.AfterSwap)
            .TriggerEvent("settled", HtmxTriggerTiming.AfterSettle));

        var headers = context.Response.GetHtmxHeaders();
        var events = headers.Trigger!;

        Assert.Multiple(() =>
        {
            Assert.That(events.Keys, Is.EqualTo(["received", "swapped", "settled"]));
            Assert.That(headers.TriggerAfterSwap, Is.SameAs(events));
            Assert.That(headers.TriggerAfterSettle, Is.SameAs(events));
        });
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(TriggerEventDetail))]
internal partial class TriggerEventJsonSerializerContext : JsonSerializerContext;

internal sealed class TriggerEventDetail
{
    public string Text { get; set; } = "";
}
