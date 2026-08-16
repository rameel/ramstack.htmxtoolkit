using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxResultTests
{
    [Test]
    public async Task ExecuteResultAsync_ConfiguresHeaders_ForHtmxRequest()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        var recording = new RecordingResult();
        var result = new HtmxResult(recording, r => r.Retarget("#content"));

        await result.ExecuteResultAsync(CreateActionContext(context));

        Assert.That(recording.Executed, Is.True);
        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Retarget], Is.EqualTo("#content"));
    }

    [Test]
    public async Task ExecuteResultAsync_SkipsConfiguration_ForNonHtmxRequest()
    {
        var context = TestHelper.CreateHttpContext();
        var recording = new RecordingResult();
        var result = new HtmxResult(recording, r => r.Retarget("#content"));

        await result.ExecuteResultAsync(CreateActionContext(context));

        Assert.That(recording.Executed, Is.True);
        Assert.That(context.Response.Headers.ContainsKey(HtmxResponseHeaderNames.Retarget), Is.False);
    }

    [Test]
    public async Task ExecuteResultAsync_PassesStateToConfigure()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        var recording = new RecordingResult();
        var result = new HtmxResult<string>(recording, (r, s) => r.Redirect(s), "/foo");

        await result.ExecuteResultAsync(CreateActionContext(context));

        Assert.That(context.Response.Headers[HtmxResponseHeaderNames.Redirect], Is.EqualTo("/foo"));
    }

    [Test]
    public async Task HtmxExtension_WithState_CallbackReceivesProvidedState()
    {
        var context = TestHelper.CreateHtmxRequestContext();
        var recording = new RecordingResult();
        var result = recording.Htmx((r, s) => r.Redirect(s), "/foo");

        await result.ExecuteResultAsync(CreateActionContext(context));

        Assert.That(
            context.Response.Headers[HtmxResponseHeaderNames.Redirect],
            Is.EqualTo("/foo"));
    }

    private static ActionContext CreateActionContext(HttpContext httpContext) =>
        new(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

    private sealed class RecordingResult : IActionResult
    {
        public bool Executed { get; private set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            Executed = true;
            return Task.CompletedTask;
        }
    }
}
