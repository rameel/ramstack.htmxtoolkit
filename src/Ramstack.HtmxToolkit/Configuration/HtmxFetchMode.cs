namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Specifies the request mode used by HTMX.
/// </summary>
/// <remarks>
/// <para>In HTMX 4.x, this is passed as the <c>mode</c> option of the Fetch API.</para>
/// <para>
///   HTMX 1.9.x and 2.x use <c>XMLHttpRequest</c> instead of <c>fetch</c>.
///   Use <see cref="HtmxV1Config.SelfRequestsOnly" /> or <see cref="HtmxV2Config.SelfRequestsOnly" />
///   to restrict requests to the current origin.
/// </para>
/// </remarks>
public enum HtmxFetchMode
{
    /// <summary>
    /// Allows requests only to the current origin.
    /// </summary>
    SameOrigin,

    /// <summary>
    /// Allows cross-origin requests using CORS.
    /// </summary>
    Cors,

    /// <summary>
    /// Allows restricted cross-origin requests that produce opaque responses.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The browser allows only the <c>GET</c>, <c>HEAD</c>, and <c>POST</c> methods in this mode.
    ///   For other methods, <c>fetch</c> throws, and HTMX raises the <c>htmx:error</c> event.
    /// </para>
    /// <para>
    ///   The browser also removes request headers that are not CORS-safelisted, even for same-origin requests.
    ///   This includes all <c>HX-*</c> headers, values from <c>hx-headers</c>, and the antiforgery token
    ///   when it is sent in a header. As a result, the server does not recognize the request as an HTMX request.
    /// </para>
    /// <para>
    ///   A cross-origin response is opaque: its status is 0, and its headers and body are not accessible.
    ///   HTMX cannot read <c>HX-*</c> response headers, does not treat the response as an error,
    ///   and swaps the empty body into the target. A same-origin response is not opaque.
    /// </para>
    /// </remarks>
    NoCors
}
