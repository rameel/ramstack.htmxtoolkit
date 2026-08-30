using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for the <see cref="HttpResponse" /> class.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Returns a strongly typed view of the HTMX response headers.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>
    /// The <see cref="HtmxResponseHeaders" />.
    /// </returns>
    public static HtmxResponseHeaders GetHtmxHeaders(this HttpResponse response) =>
        new(response);

    /// <summary>
    /// Configures the HTMX response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The delegate that configures the HTMX response headers.</param>
    public static void Htmx(this HttpResponse response, Action<HtmxResponse> configure)
    {
        if (response.HttpContext.Request.IsHtmxRequest())
            configure(new HtmxResponse(response));
    }

    /// <summary>
    /// Configures the HTMX response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The delegate that configures the HTMX response headers
    /// using <paramref name="state" />.</param>
    /// <param name="state">The state passed to <paramref name="configure" />.</param>
    public static void Htmx<TState>(this HttpResponse response, Action<HtmxResponse, TState> configure, TState state)
    {
        if (response.HttpContext.Request.IsHtmxRequest())
            configure(new HtmxResponse(response), state);
    }
}
