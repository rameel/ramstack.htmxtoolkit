using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for <see cref="IActionResult"/> to configure htmx response headers.
/// </summary>
public static class ActionResultExtensions
{
    /// <summary>
    /// Configures htmx response headers for the specified <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to configure.</param>
    /// <param name="configure">A delegate to configure the htmx response headers.</param>
    /// <returns>
    /// An <see cref="HtmxResult"/> that wraps the original result with htmx configuration.
    /// </returns>
    public static HtmxResult Htmx(this IActionResult result, Action<HtmxResponse> configure) =>
        new(result, configure);

    /// <summary>
    /// Configures htmx response headers for the specified <see cref="IActionResult"/> using a state object.
    /// </summary>
    /// <typeparam name="TState">The type of the state object passed to the configuration delegate.</typeparam>
    /// <param name="result">The <see cref="IActionResult"/> to configure.</param>
    /// <param name="configure">A delegate to configure the htmx response headers using the state object.</param>
    /// <param name="state">The state object passed to the <paramref name="configure"/> delegate.</param>
    /// <returns>
    /// An <see cref="HtmxResult{TState}"/> that wraps the original result with htmx configuration.
    /// </returns>
    public static HtmxResult<TState> Htmx<TState>(this IActionResult result, Action<HtmxResponse, TState> configure, TState state) =>
        new(result, configure, state);
}
