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
    /// <summary>
    /// Gets or sets a value that determines whether to return a partial view for htmx requests.
    /// </summary>
    public bool ReturnPartial { get; set; }

    /// <inheritdoc />
    public Task ExecuteResultAsync(ActionContext context)
    {
        if (context.HttpContext.Request.IsHtmxRequest())
        {
            if (ReturnPartial)
                result = result.ToPartialViewResult();

            var response = new HtmxResponse(context.HttpContext.Response);
            configure(response, state);
        }

        return result.ExecuteResultAsync(context);
    }

    /// <summary>
    /// Converts the <see cref="ViewResult"/> to the <see cref="PartialViewResult"/> for htmx requests.
    /// </summary>
    /// <returns>
    /// The <see cref="HtmxResult{TState}"/>.
    /// </returns>
    public HtmxResult<TState> ToPartialView()
    {
        ReturnPartial = true;
        return this;
    }
}
