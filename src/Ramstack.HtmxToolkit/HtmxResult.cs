using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult"/> that is used to configure the htmx response headers
/// along with the provided <see cref="IActionResult"/> which is used to produce the response.
/// </summary>
public sealed class HtmxResult(IActionResult result, Action<HtmxResponse> configure) : IActionResult
{
    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context)
    {
        if (context.HttpContext.Request.IsHtmxRequest())
        {
            var response = new HtmxResponse(context.HttpContext.Response);
            configure(response);
        }

        return result.ExecuteResultAsync(context);
    }
}
