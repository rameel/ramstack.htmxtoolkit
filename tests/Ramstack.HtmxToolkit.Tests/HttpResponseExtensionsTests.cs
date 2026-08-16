namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HttpResponseExtensionsTests
{
    [Test]
    public void Htmx_RunsConfigure_ForHtmxRequest()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        var invoked = false;

        context.Response.Htmx(r =>
        {
            invoked = true;
            r.Retarget("#content");
        });

        Assert.That(invoked, Is.True);
        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Retarget], Is.EqualTo("#content"));
    }

    [Test]
    public void Htmx_DoesNotRunConfigure_ForNonHtmxRequest()
    {
        var context = TestHelper.CreateHttpContext();
        var invoked = false;

        context.Response.Htmx(r =>
        {
            invoked = true;
            r.Retarget("#content");
        });

        Assert.That(invoked, Is.False);
        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Retarget), Is.False);
    }

    [Test]
    public void Htmx_WithState_PassesStateToConfigure()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        context.Response.Htmx((r, s) => r.Retarget(s), "/target");

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Retarget], Is.EqualTo("/target"));
    }

    [Test]
    public void Htmx_WithState_DoesNotRunConfigure_ForNonHtmxRequest()
    {
        var invoked = false;
        var context = TestHelper.CreateHttpContext();
        context.Response.Htmx((_, _) => invoked = true, 0);

        Assert.That(invoked, Is.False);
    }

    [Test]
    public void GetHtmxHeaders_ReturnsHeadersForResponse()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        var headers = context.Response.GetHtmxHeaders();

        headers.Redirect = "/foo";

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Redirect], Is.EqualTo("/foo"));
    }
}
