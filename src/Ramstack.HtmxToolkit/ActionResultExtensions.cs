using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for <see cref="IActionResult"/> to configure htmx response headers.
/// </summary>
public static class ActionResultExtensions
{
    /// <summary>
    /// Configures the htmx response headers for the provided <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to configure.</param>
    /// <param name="configure">The function to configure the htmx response headers.</param>
    /// <returns>
    /// A <see cref="HtmxResult"/>.
    /// </returns>
    public static HtmxResult Htmx(this IActionResult result, Action<HtmxResponse> configure) =>
        new(result, configure);

    /// <summary>
    /// Configures the htmx response headers for the provided <see cref="IActionResult"/> with a state object.
    /// </summary>
    /// <typeparam name="TState">The type of the value to pass to the function.</typeparam>
    /// <param name="result">The <see cref="IActionResult"/> to configure.</param>
    /// <param name="configure">The function to configure the htmx response headers.</param>
    /// <param name="state">The value to pass to the <paramref name="configure"/>.</param>
    /// <returns>
    /// A <see cref="HtmxResult{TState}"/>.
    /// </returns>
    public static HtmxResult<TState> Htmx<TState>(this IActionResult result, Action<HtmxResponse, TState> configure, TState state) =>
        new(result, configure, state);

    /// <summary>
    /// Converts the specified <see cref="IActionResult"/> to a partial view if it is a <see cref="ViewResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to convert.</param>
    /// <returns>
    /// If the specified <paramref name="result"/> is a <see cref="ViewResult"/>, returns a <see cref="PartialViewResult"/>;
    /// otherwise, returns the original <paramref name="result"/>.
    /// </returns>
    internal static IActionResult ToPartialViewResult(this IActionResult result)
    {
        if (result is ViewResult view)
        {
            return new PartialViewResult
            {
                ViewName = view.ViewName,
                ViewData = view.ViewData,
                TempData = view.TempData,
                ContentType = view.ContentType,
                ViewEngine = view.ViewEngine,
                StatusCode = view.StatusCode
            };
        }

        return result;
    }
}
