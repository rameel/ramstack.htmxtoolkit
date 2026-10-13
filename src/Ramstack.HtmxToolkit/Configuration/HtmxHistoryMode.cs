namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Specifies how HTMX history restoration is handled.
/// </summary>
/// <remarks>
/// Used by <see cref="HtmxV4Config.History" />. HTMX 1.x and 2.x use
/// <see cref="HtmxV1Config.HistoryEnabled" /> and <see cref="HtmxV2Config.HistoryEnabled" /> instead.
/// </remarks>
public enum HtmxHistoryMode
{
    /// <summary>
    /// Enables HTMX history support.
    /// </summary>
    /// <remarks>
    /// History navigation requests the URL from the server and swaps the response.
    /// Local DOM snapshots require the optional HTMX <c>hx-history-cache</c> extension.
    /// </remarks>
    Enabled,

    /// <summary>
    /// Disables HTMX history support.
    /// </summary>
    Disabled,

    /// <summary>
    /// Reloads the page when restoring history.
    /// </summary>
    Reload
}
