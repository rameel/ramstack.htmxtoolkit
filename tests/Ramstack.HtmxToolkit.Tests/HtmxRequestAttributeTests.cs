using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxRequestAttributeTests
{
    [TestCase(false)]
    [TestCase(true)]
    public void Accept_ReturnsTrue_ForHtmxRequest_WhenKindIsNotSet(bool boosted)
    {
        var req = new HtmxRequestAttribute();
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext(boosted));

        Assert.That(req.Accept(ctx), Is.True);
    }

    [TestCase(HtmxRequestKind.Any, null, true)]
    [TestCase(HtmxRequestKind.Any, "false", true)]
    [TestCase(HtmxRequestKind.Any, "true", true)]
    [TestCase(HtmxRequestKind.Boosted, null, false)]
    [TestCase(HtmxRequestKind.Boosted, "false", false)]
    [TestCase(HtmxRequestKind.Boosted, "true", true)]
    [TestCase(HtmxRequestKind.NonBoosted, null, true)]
    [TestCase(HtmxRequestKind.NonBoosted, "false", true)]
    [TestCase(HtmxRequestKind.NonBoosted, "true", false)]
    public void Accept_UsesKind_ForHtmxRequest(HtmxRequestKind kind, string? boostedHeader, bool expected)
    {
        var req = new HtmxRequestAttribute { Kind = kind };
        var ctx = TestHelper.CreateHtmxRequestContext();

        if (boostedHeader is not null)
            ctx.Request.Headers[HtmxRequestHeaderNames.Boosted] = boostedHeader;

        Assert.That(
            req.Accept(CreateContext(ctx)),
            Is.EqualTo(expected));
    }

    [TestCase(HtmxRequestKind.Any, false)]
    [TestCase(HtmxRequestKind.Any, true)]
    [TestCase(HtmxRequestKind.Boosted, false)]
    [TestCase(HtmxRequestKind.Boosted, true)]
    [TestCase(HtmxRequestKind.NonBoosted, false)]
    [TestCase(HtmxRequestKind.NonBoosted, true)]
    public void Accept_ReturnsFalse_ForNonHtmxRequest(HtmxRequestKind kind, bool boosted)
    {
        var req = new HtmxRequestAttribute { Kind = kind };
        var ctx = TestHelper.CreateHttpContext();

        if (boosted)
            ctx.Request.Headers[HtmxRequestHeaderNames.Boosted] = "true";

        Assert.That(req.Accept(CreateContext(ctx)), Is.False);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Accept_ReturnsFalse_ForUnknownKind(bool boosted)
    {
        var req = new HtmxRequestAttribute { Kind = (HtmxRequestKind)999 };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext(boosted));

        Assert.That(req.Accept(ctx), Is.False);
    }

    private static ActionConstraintContext CreateContext(HttpContext httpContext) =>
        new() { RouteContext = new RouteContext(httpContext) };
}
