using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an <see cref="IActionResult"/> that is used to configure the htmx response headers
/// along with the provided <see cref="IActionResult"/> which is used to produce the response.
/// </summary>
public sealed class HtmxResult(IActionResult result, Action<HtmxResponse> configure) : IActionResult
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
            configure(response);
        }

        return result.ExecuteResultAsync(context);
    }

    /// <summary>
    /// Converts the <see cref="ViewResult"/> to the <see cref="PartialViewResult"/> for htmx requests.
    /// </summary>
    /// <returns>
    /// The <see cref="HtmxResult{TState}"/>.
    /// </returns>
    public HtmxResult ToPartialView()
    {
        ReturnPartial = true;
        return this;
    }
}
