namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the Fetch cache mode for an HTMX request.
/// </summary>
/// <remarks>Supported only in HTMX 4.x.</remarks>
public enum HtmxRequestCache
{
    /// <summary>
    /// Uses the browser's normal HTTP cache behavior.
    /// </summary>
    Default,

    /// <summary>
    /// Bypasses the cache and does not store the response in it.
    /// </summary>
    NoStore,

    /// <summary>
    /// Bypasses the cache lookup and updates the cache with the response.
    /// </summary>
    Reload,

    /// <summary>
    /// Revalidates a cached response before using it.
    /// </summary>
    NoCache,

    /// <summary>
    /// Uses a cached response even if stale, or fetches and caches a response on a cache miss.
    /// </summary>
    ForceCache,

    /// <summary>
    /// Uses a cached response even if stale, and fails on a cache miss.
    /// </summary>
    /// <remarks>
    /// Requires the same-origin Fetch mode, which is the HTMX default.
    /// The request fails if <see cref="Configuration.HtmxV4Config.Mode" /> specifies another mode.
    /// </remarks>
    OnlyIfCached
}
