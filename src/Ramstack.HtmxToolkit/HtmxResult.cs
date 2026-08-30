using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Wraps an <see cref="IActionResult" /> and configures HTMX response headers
/// before executing it for an HTMX request.
/// </summary>
/// <param name="result">The action result to execute.</param>
/// <param name="configure">The delegate that configures the HTMX response headers.</param>
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
