using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for the <see cref="HttpRequest" /> class.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Determines whether the specified HTTP request is an HTMX request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is an HTMX request;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request) =>
        request.GetHtmxHeaders().Request;

    /// <summary>
    /// Determines whether the specified HTTP request is an HTMX request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this method returns, contains the <see cref="HtmxRequestHeaders" />
    /// that provides access to well-known HTMX headers.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is an HTMX request;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request, out HtmxRequestHeaders headers)
    {
        headers = new HtmxRequestHeaders(request);
        return headers.Request;
    }

    /// <summary>
    /// Determines whether the request came from a link or form enhanced with <c>hx-boost</c>.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is boosted;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request) =>
        request.GetHtmxHeaders().Boosted;

    /// <summary>
    /// Determines whether the specified HTTP request has an <c>HX-Request-Type</c> value of <c>"full"</c>.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the header value is <c>"full"</c>;
    /// otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// Returns <see langword="false" /> when the header is absent,
    /// including HTMX 1.x and 2.x requests without this header.
    /// </remarks>
    public static bool IsHtmxFullRequest(this HttpRequest request) =>
        // PERF: Compare the header directly instead of using GetHtmxHeaders().RequestType.
        // The parser also recognizes "partial", and the JIT does not reliably eliminate
        // that extra comparison even when the parser is inlined.
        request.Headers.TryGetValue(HtmxRequestHeaderNames.RequestType, out var value) && value is ["full"];

    /// <summary>
    /// Determines whether the specified HTTP request has an <c>HX-Request-Type</c> value of <c>"partial"</c>.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the header value is <c>"partial"</c>;
    /// otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// Returns <see langword="false" /> when the header is absent,
    /// including HTMX 1.x and 2.x requests without this header.
    /// </remarks>
    public static bool IsHtmxPartialRequest(this HttpRequest request) =>
        // PERF: See IsHtmxFullRequest.
        request.Headers.TryGetValue(HtmxRequestHeaderNames.RequestType, out var value) && value is ["partial"];

    /// <summary>
    /// Returns a strongly typed view of the HTMX request headers.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// The <see cref="HtmxRequestHeaders" />.
    /// </returns>
    public static HtmxRequestHeaders GetHtmxHeaders(this HttpRequest request) =>
        new(request);
}
