namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the Fetch redirect mode for an HTMX request.
/// </summary>
/// <remarks>Supported only in HTMX 4.x.</remarks>
public enum HtmxRequestRedirect
{
    /// <summary>
    /// Follows redirects automatically.
    /// This is the Fetch default.
    /// </summary>
    Follow,

    /// <summary>
    /// Fails the request when a redirect is encountered.
    /// </summary>
    /// <remarks>
    /// HTMX raises the <c>htmx:error</c> event and does not swap the response.
    /// </remarks>
    Error,

    /// <summary>
    /// Returns an opaque redirect response instead of following the redirect.
    /// </summary>
    /// <remarks>
    /// The response has status 0, and its headers and body are not accessible.
    /// HTMX cannot read <c>HX-*</c> response headers, does not treat the response as an error,
    /// and swaps the empty body into the target.
    /// </remarks>
    Manual
}
