using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxRequestAttributeTests
{
    [Test]
    public void Accept_ReturnsFalse_ForNonHtmxRequest()
    {
        var req = new HtmxRequestAttribute();
        var ctx = CreateContext(TestHelper.CreateHttpContext());

        Assert.That(req.Accept(ctx), Is.False);
    }

    [Test]
    public void Accept_ReturnsTrue_ForHtmxRequest_WhenBoostedIsNull()
    {
        var req = new HtmxRequestAttribute { Boosted = null };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext());

        Assert.That(req.Accept(ctx), Is.True);
    }

    [Test]
    public void Accept_ReturnsTrue_ForBoostedRequest_WhenBoostedIsTrue()
    {
        var req = new HtmxRequestAttribute { Boosted = true };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext(boosted: true));

        Assert.That(req.Accept(ctx), Is.True);
    }

    [Test]
    public void Accept_ReturnsFalse_ForNonBoostedRequest_WhenBoostedIsTrue()
    {
        var req = new HtmxRequestAttribute { Boosted = true };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext());

        Assert.That(req.Accept(ctx), Is.False);
    }

    [Test]
    public void Accept_ReturnsTrue_ForNonBoostedRequest_WhenBoostedIsFalse()
    {
        var req = new HtmxRequestAttribute { Boosted = false };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext());

        Assert.That(req.Accept(ctx), Is.True);
    }

    [Test]
    public void Accept_ReturnsFalse_ForBoostedRequest_WhenBoostedIsFalse()
    {
        var req = new HtmxRequestAttribute { Boosted = false };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext(boosted: true));

        Assert.That(req.Accept(ctx), Is.False);
    }

    private static ActionConstraintContext CreateContext(HttpContext httpContext) =>
        new() { RouteContext = new RouteContext(httpContext) };
}
