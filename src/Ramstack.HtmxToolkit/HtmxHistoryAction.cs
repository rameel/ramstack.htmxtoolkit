namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies how an <c>HX-Location</c> request updates the browser history.
/// </summary>
/// <remarks>Response headers from the loaded page can override the requested history action.</remarks>
public enum HtmxHistoryAction
{
    /// <summary>
    /// Adds a new entry to the browser history.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    Push,

    /// <summary>
    /// Replaces the current entry in the browser history.
    /// </summary>
    /// <remarks>Supported in HTMX 2.0.9 and later 2.x releases, and in HTMX 4.x.</remarks>
    Replace,

    /// <summary>
    /// Requests that the browser history is not updated.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 2.0.8 and later 2.x releases, and in HTMX 4.x.
    /// In HTMX 2.x, a boosted element used as the request source can still cause a history entry to be added.
    /// </remarks>
    None
}
