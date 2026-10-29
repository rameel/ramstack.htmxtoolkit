using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxResponseAttributeTests
{
    [Test]
    public void OnActionExecuting_SetsHeaders_ForHtmxRequest()
    {
        var ctx = TestHelper.CreateHtmxRequestContext();
        var res = new HtmxResponseAttribute
        {
            Refresh = true,
            Reswap = HtmxSwap.OuterHtml,
            Retarget = "#content",
            Reselect = "#list"
        };

        res.OnActionExecuting(CreateActionContext(ctx));

        Assert.That(ctx.Response.Headers[HtmxResponseHeaderNames.Refresh], Is.EqualTo("true"));
        Assert.That(ctx.Response.Headers[HtmxResponseHeaderNames.Reswap], Is.EqualTo("outerHTML"));
        Assert.That(ctx.Response.Headers[HtmxResponseHeaderNames.Retarget], Is.EqualTo("#content"));
        Assert.That(ctx.Response.Headers[HtmxResponseHeaderNames.Reselect], Is.EqualTo("#list"));
    }

    [Test]
    public void OnPageHandlerExecuting_SetsHeaders_ForHtmxRequest()
    {
        var ctx = TestHelper.CreateHtmxRequestContext();
        var res = new HtmxResponseAttribute { Retarget = "#content" };

        res.OnPageHandlerExecuting(CreatePageHandlerContext(ctx));

        Assert.That(ctx.Response.Headers[HtmxResponseHeaderNames.Retarget], Is.EqualTo("#content"));
    }

    [Test]
    public void OnActionExecuting_SetsNoHeaders_ForNonHtmxRequest()
    {
        var ctx = TestHelper.CreateHttpContext();
        var res = new HtmxResponseAttribute { Retarget = "#content" };

        res.OnActionExecuting(CreateActionContext(ctx));

        Assert.That(ctx.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Retarget), Is.False);
    }

    [Test]
    public void OnActionExecuting_SetsNoHeaders_ForHistoryRestoreRequest()
    {
        var ctx = TestHelper.CreateHttpContext(
            (HtmxRequestHeaderNames.Request, "true"),
            (HtmxRequestHeaderNames.HistoryRestoreRequest, "true"));
        var res = new HtmxResponseAttribute { Retarget = "#content" };

        res.OnActionExecuting(CreateActionContext(ctx));

        Assert.That(ctx.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Retarget), Is.False);
    }

    [Test]
    public void Properties_RoundTrips()
    {
        var res = new HtmxResponseAttribute
        {
            Refresh = true,
            Reswap = HtmxSwap.OuterHtml,
            Retarget = "#content",
            Reselect = "#list"
        };

        Assert.That(res.Refresh, Is.True);
        Assert.That(res.Reswap, Is.EqualTo(HtmxSwap.OuterHtml));
        Assert.That(res.Retarget, Is.EqualTo("#content"));
        Assert.That(res.Reselect, Is.EqualTo("#list"));
    }

    [Test]
    public void ReswapExpression_RoundTrips()
    {
        var res = new HtmxResponseAttribute { ReswapExpression = "outerHTML ignoreTitle:true" };

        Assert.That(res.Reswap, Is.EqualTo(HtmxSwap.OuterHtml));
        Assert.That(res.ReswapExpression, Is.EqualTo("outerHTML ignoreTitle:true"));
    }

    private static ActionExecutingContext CreateActionContext(HttpContext context)
    {
        return new ActionExecutingContext(
            new ActionContext(context, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());
    }

    private static PageHandlerExecutingContext CreatePageHandlerContext(HttpContext context)
    {
        return new PageHandlerExecutingContext(
            new PageContext(new ActionContext(context, new RouteData(), new CompiledPageActionDescriptor())),
            new List<IFilterMetadata>(),
            handlerMethod: null,
            new Dictionary<string, object?>(),
            new object());
    }
}
