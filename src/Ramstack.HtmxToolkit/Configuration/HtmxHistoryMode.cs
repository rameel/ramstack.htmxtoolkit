namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Specifies how HTMX history restoration is handled.
/// </summary>
public enum HtmxHistoryMode
{
    /// <summary>
    /// Enables history snapshots and restoration.
    /// </summary>
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
