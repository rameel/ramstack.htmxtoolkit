namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Specifies how HTMX history restoration is handled.
/// </summary>
public enum HtmxHistoryMode
{
    /// <summary>
    /// Enables HTMX history handling.
    /// </summary>
    /// <remarks>
    /// In HTMX 4.x, history navigation requests the URL from the server and swaps
    /// the response. Local DOM snapshots require the optional HTMX history-cache extension.
    /// </remarks>
    Enabled,

    /// <summary>
    /// Disables HTMX history support.
    /// </summary>
    Disabled,

    /// <summary>
    /// Reloads the page when restoring history.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    Reload
}
