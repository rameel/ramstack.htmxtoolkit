using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxResponseHeadersTests
{
    [Test]
    public void Properties_RoundTrips()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        headers.Location = "/foo";
        headers.PushUrl = "/bar";
        headers.Redirect = "/baz";
        headers.ReplaceUrl = "/qux";
        headers.Retarget = "#target";
        headers.Reselect = "#select";
        headers.Reswap = HtmxSwap.OuterHtml;
        headers.Trigger = new Dictionary<string, object> { ["message"] = "hello" };
        headers.TriggerAfterSwap = new Dictionary<string, object> { ["swapped"] = true };
        headers.TriggerAfterSettle = new Dictionary<string, object> { ["settled"] = 1 };

        Assert.That(headers.Location, Is.EqualTo("/foo"));
        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Location], Is.EqualTo("/foo"));
        Assert.That(headers.PushUrl, Is.EqualTo("/bar"));
        Assert.That(headers.Redirect, Is.EqualTo("/baz"));
        Assert.That(headers.ReplaceUrl, Is.EqualTo("/qux"));
        Assert.That(headers.Retarget, Is.EqualTo("#target"));
        Assert.That(headers.Reselect, Is.EqualTo("#select"));
        Assert.That(headers.Reswap, Is.EqualTo(HtmxSwap.OuterHtml));
        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Reswap], Is.EqualTo("outerHTML"));
        Assert.That(headers.Trigger.Count, Is.EqualTo(1));
        Assert.That(headers.Trigger["message"], Is.EqualTo("hello"));
        Assert.That(headers.TriggerAfterSwap.Count, Is.EqualTo(1));
        Assert.That(headers.TriggerAfterSwap["swapped"], Is.True);
        Assert.That(headers.TriggerAfterSettle.Count, Is.EqualTo(1));
        Assert.That(headers.TriggerAfterSettle["settled"], Is.EqualTo(1));
    }

    [Test]
    public void Refresh_WhenTrue_SetsTrue()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        headers.Refresh = true;

        Assert.That(headers.Refresh, Is.True);
        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Refresh], Is.EqualTo("true"));
    }

    [Test]
    public void Refresh_IsFalse_ByDefault()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        Assert.That(headers.Refresh, Is.False);
    }

    [Test]
    public void Reswap_IsNull_WhenHeaderAbsent()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        Assert.That(headers.Reswap, Is.Null);
    }

    [Test]
    public void Reswap_IsNull_WhenHeaderUnknown()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        context.Response.Headers[HtmxResponseHeaderNames.Reswap] = "bogus";

        Assert.That(headers.Reswap, Is.Null);
    }

    [Test]
    public void ReswapExpression_KeepsFullExpression_WhileReswapParsesOnlyStyle()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        headers.ReswapExpression = "outerHTML show:top";

        Assert.That(headers.Reswap, Is.EqualTo(HtmxSwap.OuterHtml));
        Assert.That(headers.ReswapExpression, Is.EqualTo("outerHTML show:top"));
    }

    [Test]
    public void SettingNull_DoesNotAddHeader()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        headers.Location = null!;

        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Location), Is.False);
    }

    [Test]
    public void Trigger_IsNull_WhenNotSet()
    {
        var context = TestHelper.CreateHttpContext();
        var headers = context.Response.GetHtmxHeaders();

        Assert.That(headers.Trigger, Is.Null);
        Assert.That(headers.TriggerAfterSwap, Is.Null);
        Assert.That(headers.TriggerAfterSettle, Is.Null);
    }

    [Test]
    public void TriggerTimingProperties_Htmx4_AliasReceiveTrigger()
    {
        var context = TestHelper.CreateHtmxRequestContext(HtmxTargetVersion.V4);
        var headers = context.Response.GetHtmxHeaders();
        var events = new Dictionary<string, object> { ["swapped"] = true };

        headers.TriggerAfterSwap = events;

        Assert.Multiple(() =>
        {
            Assert.That(headers.Trigger, Is.EqualTo(events));
            Assert.That(headers.TriggerAfterSwap, Is.EqualTo(events));
            Assert.That(headers.TriggerAfterSettle, Is.EqualTo(events));
        });

        PendingEvents.GetOrCreate(context.Response).Flush();

        Assert.Multiple(() =>
        {
            Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Trigger), Is.True);
            Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSwap), Is.False);
            Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.TriggerAfterSettle), Is.False);
        });
    }
}
