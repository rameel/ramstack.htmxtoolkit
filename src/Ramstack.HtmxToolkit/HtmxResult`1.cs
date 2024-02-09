using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult"/> that is used to configure the htmx response headers
/// along with the provided <see cref="IActionResult"/> which is used to produce the response.
/// </summary>
/// <typeparam name="TState">The type of the value to pass to <paramref name="configure"/>. Used to reduce memory allocations.</typeparam>
/// <param name="result">The <see cref="IActionResult"/> to execute as a response result.</param>
/// <param name="configure">The function to configure the htmx response headers.</param>
/// <param name="state">The value to pass to <paramref name="configure"/>.</param>
public sealed class HtmxResult<TState>(IActionResult result, Action<HtmxResponse, TState> configure, TState state) : IActionResult
{
    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context)
    {
        context.HttpContext.Response.Htmx(configure, state);
        return result.ExecuteResultAsync(context);
    }
}
