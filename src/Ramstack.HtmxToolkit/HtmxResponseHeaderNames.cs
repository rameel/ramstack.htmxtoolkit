namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines constants for the well-known names of htmx response headers.
/// For more information, see https://htmx.org/reference/#response_headers
/// </summary>
public static class HtmxResponseHeaderNames
{
    /// <summary>
    /// The <c>HX-Location</c> header is used to a client-side redirect that does not do a full page reload.
    /// </summary>
    public const string Location = "HX-Location";

    /// <summary>
    /// The <c>HX-Push-Url</c> header is used to push a new URL into the history stack.
    /// </summary>
    public const string PushUrl = "HX-Push-Url";

    /// <summary>
    /// The <c>HX-Redirect</c> header is used to client-side redirect to a new location.
    /// </summary>
    public const string Redirect = "HX-Redirect";

    /// <summary>
    /// The <c>HX-Refresh</c> header is used to full refresh of the page.
    /// </summary>
    public const string Refresh = "HX-Refresh";

    /// <summary>
    /// The <c>HX-Replace-Url</c> header is used to replace the current URL.
    /// </summary>
    public const string ReplaceUrl = "HX-Replace-Url";

    /// <summary>
    /// The <c>HX-Reswap</c> header allows to specify how the response will be swapped.
    /// </summary>
    public const string Reswap = "HX-Reswap";

    /// <summary>
    /// The <c>HX-Retarget</c> header sets a CSS selector to update
    /// the target of the content update to a different element on the page.
    /// </summary>
    public const string Retarget = "HX-Retarget";

    /// <summary>
    /// The <c>HX-Reselect</c> header sets a CSS selector to choose
    /// which part of the response is used to be swapped in.
    /// </summary>
    public const string Reselect = "HX-Reselect";

    /// <summary>
    /// The <c>HX-Trigger</c> header is used to trigger an event on the client side
    /// after the server response is processed.
    /// </summary>
    public const string Trigger = "HX-Trigger";

    /// <summary>
    /// The <c>HX-Trigger-After-Settle</c> header is used to trigger an event
    /// on the client side after the htmx request has settled.
    /// </summary>
    public const string TriggerAfterSettle = "HX-Trigger-After-Settle";

    /// <summary>
    /// The <c>HX-Trigger-After-Swap</c> header is used to trigger an event
    /// on the client side after the htmx request has been swapped.
    /// </summary>
    public const string TriggerAfterSwap = "HX-Trigger-After-Swap";
}
