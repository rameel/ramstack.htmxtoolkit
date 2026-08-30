using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult" /> that when executed configures HTMX response headers
/// before executing the wrapped result.
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
