using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxRequestAttributeTests
{
    [TestCase(false)]
    [TestCase(true)]
    public void Accept_ReturnsTrue_ForHtmxRequest_WhenFiltersAreNotSet(bool boosted)
    {
        var req = new HtmxRequestAttribute();
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext(boosted));

        Assert.That(req.Accept(ctx), Is.True);
    }

    [TestCase(HtmxBoostedFilter.Any, null, true)]
    [TestCase(HtmxBoostedFilter.Any, "false", true)]
    [TestCase(HtmxBoostedFilter.Any, "true", true)]
    [TestCase(HtmxBoostedFilter.Boosted, null, false)]
    [TestCase(HtmxBoostedFilter.Boosted, "false", false)]
    [TestCase(HtmxBoostedFilter.Boosted, "true", true)]
    [TestCase(HtmxBoostedFilter.NonBoosted, null, true)]
    [TestCase(HtmxBoostedFilter.NonBoosted, "false", true)]
    [TestCase(HtmxBoostedFilter.NonBoosted, "true", false)]
    public void Accept_UsesBoostedFilter(HtmxBoostedFilter filter, string? boostedHeader, bool expected)
    {
        var req = new HtmxRequestAttribute { BoostedFilter = filter };
        var ctx = TestHelper.CreateHtmxRequestContext();

        if (boostedHeader is not null)
            ctx.Request.Headers[HtmxRequestHeaderNames.Boosted] = boostedHeader;

        Assert.That(
            req.Accept(CreateContext(ctx)),
            Is.EqualTo(expected));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Accept_ReturnsFalse_ForUnknownBoostedFilter(bool boosted)
    {
        var req = new HtmxRequestAttribute { BoostedFilter = (HtmxBoostedFilter)999 };
        var ctx = CreateContext(TestHelper.CreateHtmxRequestContext(boosted));

        Assert.That(req.Accept(ctx), Is.False);
    }

    [TestCase(HtmxRequestType.Unspecified, null, true)]
    [TestCase(HtmxRequestType.Unspecified, "full", true)]
    [TestCase(HtmxRequestType.Unspecified, "partial", true)]
    [TestCase(HtmxRequestType.Unspecified, "unknown", true)]
    [TestCase(HtmxRequestType.Full, "full", true)]
    [TestCase(HtmxRequestType.Full, "partial", false)]
    [TestCase(HtmxRequestType.Full, null, false)]
    [TestCase(HtmxRequestType.Full, "", false)]
    [TestCase(HtmxRequestType.Full, "unknown", false)]
    [TestCase(HtmxRequestType.Partial, "partial", true)]
    [TestCase(HtmxRequestType.Partial, "full", false)]
    [TestCase(HtmxRequestType.Partial, null, false)]
    [TestCase(HtmxRequestType.Partial, "", false)]
    [TestCase(HtmxRequestType.Partial, "unknown", false)]
    [TestCase((HtmxRequestType)999, "full", false)]
    [TestCase((HtmxRequestType)999, null, false)]
    public void Accept_UsesRequestType(HtmxRequestType requestType, string? requestTypeHeader, bool expected)
    {
        var req = new HtmxRequestAttribute { RequestType = requestType };
        var ctx = TestHelper.CreateHtmxRequestContext();

        if (requestTypeHeader is not null)
            ctx.Request.Headers[HtmxRequestHeaderNames.RequestType] = requestTypeHeader;

        Assert.That(req.Accept(CreateContext(ctx)), Is.EqualTo(expected));
    }

    [TestCase(HtmxRequestType.Full, HtmxBoostedFilter.Boosted, "full", true, true)]
    [TestCase(HtmxRequestType.Full, HtmxBoostedFilter.Boosted, "full", false, false)]
    [TestCase(HtmxRequestType.Full, HtmxBoostedFilter.Boosted, "partial", true, false)]
    [TestCase(HtmxRequestType.Full, HtmxBoostedFilter.Boosted, null, true, false)]
    [TestCase(HtmxRequestType.Full, HtmxBoostedFilter.NonBoosted, "full", false, true)]
    [TestCase(HtmxRequestType.Full, HtmxBoostedFilter.NonBoosted, "full", true, false)]
    [TestCase(HtmxRequestType.Partial, HtmxBoostedFilter.Boosted, "partial", true, true)]
    [TestCase(HtmxRequestType.Partial, HtmxBoostedFilter.NonBoosted, "partial", false, true)]
    [TestCase(HtmxRequestType.Partial, HtmxBoostedFilter.NonBoosted, "full", false, false)]
    [TestCase(HtmxRequestType.Partial, HtmxBoostedFilter.NonBoosted, null, false, false)]
    [TestCase(HtmxRequestType.Full, (HtmxBoostedFilter)999, "full", true, false)]
    public void Accept_RequiresBothFilters(HtmxRequestType requestType, HtmxBoostedFilter boostedFilter, string? requestTypeHeader, bool boosted, bool expected)
    {
        var req = new HtmxRequestAttribute { RequestType = requestType, BoostedFilter = boostedFilter };
        var ctx = TestHelper.CreateHtmxRequestContext(boosted);

        if (requestTypeHeader is not null)
            ctx.Request.Headers[HtmxRequestHeaderNames.RequestType] = requestTypeHeader;

        Assert.That(req.Accept(CreateContext(ctx)), Is.EqualTo(expected));
    }

    private static ActionConstraintContext CreateContext(HttpContext httpContext) =>
        new() { RouteContext = new RouteContext(httpContext) };
}
