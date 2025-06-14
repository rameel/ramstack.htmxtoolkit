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
    /// <see langword="true" /> if the specified HTTP request is htmx request;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request) =>
        request.Headers.ContainsKey(HtmxRequestHeaderNames.Request);

    /// <summary>
    /// Determines whether the specified HTTP request is htmx request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this method returns, contains the <see cref="HtmxRequestHeaders"/>
    /// that provides well-known htmx headers.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is htmx request; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request, out HtmxRequestHeaders headers)
    {
        headers = new HtmxRequestHeaders(request);
        return request.IsHtmxRequest();
    }

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is boosted; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request) =>
        request.Headers.TryGetValue(HtmxRequestHeaderNames.Boosted, out var value) && value is ["true"];

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this method returns, contains the <see cref="HtmxRequestHeaders"/>
    /// that provides well-known htmx headers.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is boosted; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request, out HtmxRequestHeaders headers)
    {
        headers = new HtmxRequestHeaders(request);
        return request.IsHtmxBoosted();
    }

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
