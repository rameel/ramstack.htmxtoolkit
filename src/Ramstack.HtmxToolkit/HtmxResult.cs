using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult"/> that is used to configure the htmx response headers
/// along with the provided <see cref="IActionResult"/> which is used to produce the response.
/// </summary>
/// <param name="result">The <see cref="IActionResult"/> to execute as a response result.</param>
/// <param name="configure">The function to configure the htmx response headers.</param>
public sealed class HtmxResult(IActionResult result, Action<HtmxResponse> configure) : IActionResult
{
    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.Htmx(configure);
        return result.ExecuteResultAsync(context);
    }
}
