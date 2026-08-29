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
    public void Location_WithContext_SerializesJson()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new AjaxContext
        {
            Source = "button",
            Event = "click",
            Target = "#content",
            Swap = HtmxSwap.OuterHtml,
            Select = "#list"
        }));

        var header = context.Response.Headers[HtmxResponseHeaderNames.Location].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["path"].GetString(), Is.EqualTo("/bar"));
        Assert.That(json["source"].GetString(), Is.EqualTo("button"));
        Assert.That(json["event"].GetString(), Is.EqualTo("click"));
        Assert.That(json["target"].GetString(), Is.EqualTo("#content"));
        Assert.That(json["swap"].GetString(), Is.EqualTo("outerHTML"));
        Assert.That(json["select"].GetString(), Is.EqualTo("#list"));
    }

    [Test]
    public void Location_WithContext_OmitsNullProperties()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new AjaxContext()));

        var header = context.Response.Headers[HtmxResponseHeaderNames.Location].ToString();
        var json = JsonHelper.ParseJson(header);

        Assert.That(json["path"].GetString(), Is.EqualTo("/bar"));
        Assert.That(json.ContainsKey("source"), Is.False);
        Assert.That(json.ContainsKey("swap"), Is.False);
    }

    [Test]
    public void Location_WithContext_SerializesHandlerValuesAndHeaders()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.Location("/bar", new AjaxContext
        {
            Handler = "handleResponse",
            Values = new Dictionary<string, HtmxValues>
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

        Assert.That(json["handler"].GetString(), Is.EqualTo("handleResponse"));
        Assert.That(json["values"].GetProperty("id").GetString(), Is.EqualTo("42"));
        Assert.That(json["values"].GetProperty("tags").GetRawText(), Is.EqualTo("[\"dotnet\",\"web\"]"));
        Assert.That(json["headers"].GetProperty("X-Test").GetString(), Is.EqualTo("abc"));
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
    public void StopPolling_SetsStatusCode286()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.StopPolling());

        Assert.That(context.Response.StatusCode, Is.EqualTo(HtmxResponse.StopPollingStatusCode));
    }

    [Test]
    public void StopPolling_WithFalseCondition_DoesNotChangeStatusCode()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.StopPolling(false));

        Assert.That(context.Response.StatusCode, Is.Not.EqualTo(HtmxResponse.StopPollingStatusCode));
    }

    [Test]
    public void StopPolling_WithTrueCondition_SetsStatusCode286()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.StopPolling(true));

        Assert.That(context.Response.StatusCode, Is.EqualTo(HtmxResponse.StopPollingStatusCode));
    }

    [Test]
    public void TriggerEvent_SetsReceiveTrigger()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("notify", "hello"));

        var events = context.Response.GetHtmxHeaders().Trigger!;
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events["notify"], Is.EqualTo("hello"));
    }

    [Test]
    public void TriggerEvent_WithTiming_SetsAfterSettleTrigger()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvent("done", HtmxTriggerTiming.AfterSettle));

        var events = context.Response.GetHtmxHeaders().TriggerAfterSettle!;

        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events["done"], Is.EqualTo(""));
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
    public void TriggerEvents_SetsMultipleEvents()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx(r => r.TriggerEvents(
            new Dictionary<string, object> { ["a"] = 1, ["b"] = 2 },
            HtmxTriggerTiming.AfterSwap));

        var events = context.Response.GetHtmxHeaders().TriggerAfterSwap!;
        Assert.That(events.Count, Is.EqualTo(2));
        Assert.That(events["a"], Is.EqualTo(1));
        Assert.That(events["b"], Is.EqualTo(2));
    }
}
