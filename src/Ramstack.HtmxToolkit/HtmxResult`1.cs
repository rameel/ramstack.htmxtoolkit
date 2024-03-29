using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult"/> that is used to configure the htmx response headers.
/// </summary>
/// <typeparam name="TState">The type of the value to pass to <paramref name="configure"/>. Used to reduce memory allocations.</typeparam>
/// <param name="result">The <see cref="IActionResult"/> to produce the response result.</param>
/// <param name="configure">The function to configure the htmx response headers.</param>
/// <param name="state">The value to pass to <paramref name="configure"/>.</param>
public sealed class HtmxResult<TState>(IActionResult result, Action<HtmxResponse, TState> configure, TState state) : IActionResult
{
    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context)
    {
        if (context.HttpContext.Request.IsHtmxRequest())
        {
            var response = new HtmxResponse(context.HttpContext.Response);
            configure(response, state);
        }

        return result.ExecuteResultAsync(context);
    }
}
