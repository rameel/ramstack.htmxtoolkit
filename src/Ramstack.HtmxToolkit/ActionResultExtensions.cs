using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for <see cref="IActionResult"/> to configure HTMX response headers.
/// </summary>
public static class ActionResultExtensions
{
    /// <summary>
    /// Configures HTMX response headers for the specified <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to configure.</param>
    /// <param name="configure">A delegate to configure the HTMX response headers.</param>
    /// <returns>
    /// An <see cref="HtmxResult"/> that wraps the original result with HTMX configuration.
    /// </returns>
    public static HtmxResult Htmx(this IActionResult result, Action<HtmxResponse> configure) =>
        new(result, configure);

    /// <summary>
    /// Configures HTMX response headers for the specified <see cref="IActionResult"/> using a state object.
    /// </summary>
    /// <typeparam name="TState">The type of the state object passed to the configuration delegate.</typeparam>
    /// <param name="result">The <see cref="IActionResult"/> to configure.</param>
    /// <param name="configure">A delegate to configure the HTMX response headers using the state object.</param>
    /// <param name="state">The state object passed to the <paramref name="configure"/> delegate.</param>
    /// <returns>
    /// An <see cref="HtmxResult{TState}"/> that wraps the original result with HTMX configuration.
    /// </returns>
    public static HtmxResult<TState> Htmx<TState>(this IActionResult result, Action<HtmxResponse, TState> configure, TState state) =>
        new(result, configure, state);
}
