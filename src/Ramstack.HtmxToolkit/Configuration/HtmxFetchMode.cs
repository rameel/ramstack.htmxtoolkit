namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Specifies the request mode used by HTMX.
/// </summary>
/// <remarks>
/// <para>In HTMX 4.x this is passed as the <c>mode</c> option of the Fetch API.</para>
/// <para>
///   In HTMX 1.x and 2.x, the equivalent setting is the <c>selfRequestsOnly</c>
///   boolean configuration option, for which <see cref="SameOrigin" /> corresponds to
///   <see langword="true" /> and any other value corresponds to <see langword="false" />.
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
    /// Opaque responses cannot normally be swapped by HTMX.
    /// </summary>
    NoCors
}
