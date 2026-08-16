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
    public void IsHtmxRequest_ReturnsTrue_RegardlessOfHeaderValue()
    {
        var context = TestHelper.CreateHttpContext((HtmxRequestHeaderNames.Request, ""));
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
    public void IsHtmxBoosted_ReturnsFalse_WhenHeaderAbsent()
    {
        var context = TestHelper.CreateHttpContext((HtmxRequestHeaderNames.Request, "true"));
        Assert.That(context.Request.IsHtmxBoosted(), Is.False);
    }

    [Test]
    public void IsHtmxBoosted_ReturnsTrue_WhenHeaderIsTrue()
    {
        var context = TestHelper.CreateHttpContext((HtmxRequestHeaderNames.Boosted, "true"));
        Assert.That(context.Request.IsHtmxBoosted(), Is.True);
    }

    [Test]
    public void IsHtmxBoosted_ReturnsFalse_WhenHeaderIsFalse()
    {
        var context = TestHelper.CreateHttpContext((HtmxRequestHeaderNames.Boosted, "false"));
        Assert.That(context.Request.IsHtmxBoosted(), Is.False);
    }

    [Test]
    public void IsHtmxBoosted_WithOutParameter_ReturnsHeaders()
    {
        var context = TestHelper.CreateHttpContext((HtmxRequestHeaderNames.Boosted, "true"));

        Assert.That(context.Request.IsHtmxBoosted(out var headers), Is.True);
        Assert.That(headers.Boosted, Is.True);
    }
}
