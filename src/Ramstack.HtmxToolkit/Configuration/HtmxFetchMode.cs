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
    /// An opaque response has status 0, and its headers and body are not accessible.
    /// HTMX cannot read <c>HX-*</c> response headers, does not treat the response as an error,
    /// and swaps the empty body into the target.
    /// </remarks>
    NoCors
}
