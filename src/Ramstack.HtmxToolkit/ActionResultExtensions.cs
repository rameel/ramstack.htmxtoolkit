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
}
