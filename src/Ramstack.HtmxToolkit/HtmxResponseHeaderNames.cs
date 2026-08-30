namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines constants for the well-known names of HTMX response headers.
/// </summary>
/// <remarks>
/// For more information, see <see href="https://htmx.org/reference/#response_headers">HTMX Response Headers Reference</see>.
/// </remarks>
public static class HtmxResponseHeaderNames
{
    /// <summary>
    /// The <c>HX-Location</c> header performs a client-side redirect without a full-page reload.
    /// </summary>
    public const string Location = "HX-Location";

    /// <summary>
    /// The <c>HX-Push-Url</c> header pushes a new URL onto the browser's history stack.
    /// </summary>
    public const string PushUrl = "HX-Push-Url";

    /// <summary>
    /// The <c>HX-Redirect</c> header performs a client-side redirect to a new location.
    /// </summary>
    public const string Redirect = "HX-Redirect";

    /// <summary>
    /// The <c>HX-Refresh</c> header performs a full-page refresh when its value is <c>"true"</c>.
    /// </summary>
    public const string Refresh = "HX-Refresh";

    /// <summary>
    /// The <c>HX-Replace-Url</c> header replaces the current URL
    /// without pushing a new entry to the browser's history stack.
    /// </summary>
    public const string ReplaceUrl = "HX-Replace-Url";

    /// <summary>
    /// The <c>HX-Reswap</c> header specifies how the response is swapped into the DOM.
    /// </summary>
    public const string Reswap = "HX-Reswap";

    /// <summary>
    /// The <c>HX-Retarget</c> header specifies a CSS selector
    /// that changes the target of the content update.
    /// </summary>
    public const string Retarget = "HX-Retarget";

    /// <summary>
    /// The <c>HX-Reselect</c> header specifies a CSS selector that determines
    /// which part of the response is swapped in.
    /// </summary>
    public const string Reselect = "HX-Reselect";

    /// <summary>
    /// The <c>HX-Trigger</c> header triggers client-side events.
    /// </summary>
    public const string Trigger = "HX-Trigger";

    /// <summary>
    /// The <c>HX-Trigger-After-Settle</c> header triggers client-side events after the settle step.
    /// </summary>
    /// <remarks>Supported only in HTMX 1.x and 2.x.</remarks>
    public const string TriggerAfterSettle = "HX-Trigger-After-Settle";

    /// <summary>
    /// The <c>HX-Trigger-After-Swap</c> header triggers client-side events after the swap step.
    /// </summary>
    /// <remarks>Supported only in HTMX 1.x and 2.x.</remarks>
    public const string TriggerAfterSwap = "HX-Trigger-After-Swap";
}
