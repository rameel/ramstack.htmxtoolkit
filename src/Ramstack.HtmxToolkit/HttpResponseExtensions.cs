using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for the <see cref="HttpResponse"/> class.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Returns the <see cref="HtmxResponseHeaders"/> that provides well-known htmx headers.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>
    /// The <see cref="HtmxResponseHeaders"/>.
    /// </returns>
    public static HtmxResponseHeaders GetHtmxHeaders(this HttpResponse response) =>
        new(response);

    /// <summary>
    /// Configures the htmx response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The function to configure the htmx response headers.</param>
    public static void Htmx(this HttpResponse response, Action<HtmxResponse> configure)
    {
        if (response.HttpContext.Request.IsHtmxRequest())
            configure(new HtmxResponse(response));
    }

    /// <summary>
    /// Configures the htmx response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The function to configure the htmx response headers.</param>
    /// <param name="state">The value to pass to the <paramref name="configure"/>.</param>
    public static void Htmx<TState>(this HttpResponse response, Action<HtmxResponse, TState> configure, TState state)
    {
        if (response.HttpContext.Request.IsHtmxRequest())
            configure(new HtmxResponse(response), state);
    }
}
