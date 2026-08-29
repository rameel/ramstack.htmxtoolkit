namespace Ramstack.HtmxToolkit;

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
    /// Reloads the page when restoring history in HTMX 4. HTMX 1 and 2 treat this as <see cref="Enabled"/>.
    /// </summary>
    Reload
}
