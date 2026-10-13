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
    /// <see langword="true" /> if the request has the <c>HX-Request</c> header
    /// and is not a history restoration request; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   HTMX treats history restoration as a full page load rather than an HTMX request.
    ///   HTMX 4.x sends it without <c>HX-Request</c>, but HTMX 1.9.x and 2.x send that header by default,
    ///   so the method also checks <see cref="HtmxRequestHeaders.HistoryRestoreRequest" />.
    ///   See <see href="https://github.com/bigskysoftware/htmx/issues/4139#issuecomment-6135080415">issue #4139</see>.
    /// </para>
    /// <para><see cref="HtmxRequestHeaders.Request" /> reports the <c>HX-Request</c> header alone.</para>
    /// </remarks>
    public static bool IsHtmxRequest(this HttpRequest request) =>
        request.IsHtmxRequest(out _);

    /// <summary>
    /// Determines whether the specified HTTP request is an HTMX request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">
    /// When this method returns, contains a strongly typed view of the HTMX request headers.
    /// The value is set even when the method returns <see langword="false" />.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the request has the <c>HX-Request</c> header
    /// and is not a history restoration request; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// See <see cref="IsHtmxRequest(HttpRequest)" />.
    /// </remarks>
    public static bool IsHtmxRequest(this HttpRequest request, out HtmxRequestHeaders headers)
    {
        headers = new HtmxRequestHeaders(request);
        return headers is { Request: true, HistoryRestoreRequest: false };
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
    /// Determines whether the specified HTTP request expects a full page.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the <c>HX-Request-Type</c> header value is <c>"full"</c>,
    /// or the header is absent and the request restores history;
    /// otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// HTMX 4.x sends the header itself; for HTMX 1.9.x and 2.x, the Toolkit script
    /// adds it. See <see cref="HtmxRequestHeaders.RequestType" />.
    /// </remarks>
    public static bool IsHtmxFullRequest(this HttpRequest request)
    {
        // PERF: Compare the header directly instead of using GetHtmxHeaders().RequestType.
        // The parser also recognizes "partial", and the JIT does not reliably eliminate
        // that extra comparison even when the parser is inlined.
        var headers = request.Headers;
        if (headers.TryGetValue(HtmxRequestHeaderNames.RequestType, out var value))
            return value is ["full"];

        return headers.TryGetValue(HtmxRequestHeaderNames.HistoryRestoreRequest, out value) && value is ["true"];
    }

    /// <summary>
    /// Determines whether the specified HTTP request expects an HTML fragment.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the <c>HX-Request-Type</c> header value is <c>"partial"</c>;
    /// otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// HTMX 4.x sends the header itself; for HTMX 1.9.x and 2.x, the Toolkit script
    /// adds it. Without the header, the method returns <see langword="false" />.
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
