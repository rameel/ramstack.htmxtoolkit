using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for the <see cref="HttpRequest"/> class.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Determines whether the specified HTTP request is htmx request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <c>true</c> if the specified HTTP request is htmx request; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request) =>
        request.Headers.ContainsKey(HtmxRequestHeaderNames.Request);

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <c>true</c> if the specified HTTP request is boosted; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request) =>
        request.Headers.TryGetValue(HtmxRequestHeaderNames.Boosted, out var value) && value[0] == "true";

    /// <summary>
    /// Returns the <see cref="HtmxRequestHeaders"/> that provides well-known htmx headers.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// The <see cref="HtmxRequestHeaders"/>.
    /// </returns>
    public static HtmxRequestHeaders GetHtmxHeaders(this HttpRequest request) =>
        new(request);
}
