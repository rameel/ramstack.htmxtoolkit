using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult" /> that when executed configures HTMX response headers
/// before executing the wrapped result.
/// </summary>
/// <typeparam name="TState">The type of state passed to <paramref name="configure" />.</typeparam>
/// <param name="result">The action result to execute.</param>
/// <param name="configure">The delegate that configures the HTMX response headers
/// using <paramref name="state" />.</param>
/// <param name="state">The state passed to <paramref name="configure" />.</param>
/// <remarks>
/// The state parameter enables callers to avoid closure allocations.
/// </remarks>
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
