namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HttpRequestExtensionsTests
{
    [Test]
    public void IsHtmxRequest_ReturnsFalse_WhenHeaderAbsent()
    {
        var context = TestHelper.CreateHttpContext();
        Assert.That(context.Request.IsHtmxRequest(), Is.False);
    }

    [Test]
    public void IsHtmxRequest_ReturnsTrue_WhenHeaderPresent()
    {
        var context = TestHelper.CreateHttpContext((HtmxRequestHeaderNames.Request, "true"));
        Assert.That(context.Request.IsHtmxRequest(), Is.True);
    }

    [Test]
    public void IsHtmxRequest_WithOutParameter_ReturnsHeaders()
    {
        var context = TestHelper.CreateHttpContext(
            (HtmxRequestHeaderNames.Request, "true"),
            (HtmxRequestHeaderNames.Target, "foo"));

        Assert.That(context.Request.IsHtmxRequest(out var headers), Is.True);
        Assert.That(headers.Target, Is.EqualTo("foo"));
    }

    [Test]
    public void IsHtmxBoosted_ReturnsExpectedValue()
    {
        var context = TestHelper.CreateHttpContext();
        var request = context.Request;

        Assert.That(request.IsHtmxBoosted(), Is.False);

        request.Headers[HtmxRequestHeaderNames.Boosted] = "false";
        Assert.That(request.IsHtmxFullRequest(), Is.False);

        request.Headers[HtmxRequestHeaderNames.Boosted] = "true";
        Assert.That(request.IsHtmxFullRequest(), Is.True);
    }

    [Test]
    public void IsHtmxFullRequest_ReturnsExpectedValue()
    {
        var context = TestHelper.CreateHttpContext();
        var request = context.Request;

        Assert.That(request.IsHtmxFullRequest(), Is.False);

        request.Headers[HtmxRequestHeaderNames.RequestType] = "full";
        Assert.That(request.IsHtmxFullRequest(), Is.True);
    }

    [Test]
    public void IsHtmxPartialRequest_ReturnsExpectedValue()
    {
        var context = TestHelper.CreateHttpContext();
        var request = context.Request;

        Assert.That(request.IsHtmxPartialRequest(), Is.False);

        request.Headers[HtmxRequestHeaderNames.RequestType] = "full";
        Assert.That(request.IsHtmxPartialRequest(), Is.True);
    }
}
