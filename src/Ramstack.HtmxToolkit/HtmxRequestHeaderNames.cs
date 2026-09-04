namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines constants for the well-known names of HTMX request headers.
/// </summary>
/// <remarks>
/// For more information, see <see href="https://htmx.org/reference/#request_headers">HTMX Request Headers Reference</see>.
/// </remarks>
public static class HtmxRequestHeaderNames
{
    /// <summary>
    /// The <c>HX-Boosted</c> header indicates whether the request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    public const string Boosted = "HX-Boosted";

    /// <summary>
    /// The <c>HX-Current-URL</c> header contains the current URL of the browser.
    /// </summary>
    public const string CurrentUrl = "HX-Current-URL";

    /// <summary>
    /// The <c>HX-History-Restore-Request</c> header indicates whether the request
    /// is for history restoration after a miss in the local history cache.
    /// </summary>
    public const string HistoryRestoreRequest = "HX-History-Restore-Request";

    /// <summary>
    /// The <c>HX-Prompt</c> header contains the user's response to an <c>hx-prompt</c>.
    /// </summary>
    /// <remarks>
    /// Supported only in HTMX 1.x and 2.x. HTMX 4.x removed <c>hx-prompt</c>
    /// and does not send this header.
    /// </remarks>
    public const string Prompt = "HX-Prompt";

    /// <summary>
    /// The <c>HX-Request</c> header indicates that the request was issued by HTMX.
    /// Its value is always <c>"true"</c>.
    /// </summary>
    public const string Request = "HX-Request";

    /// <summary>
    /// The <c>HX-Target</c> header identifies the target element, if present.
    /// </summary>
    /// <remarks>
    /// <para>In HTMX 1.x and 2.x, the value is the ID of the target element.</para>
    /// <para>In HTMX 4.x, the value is in <c>tag#id</c> format, for example <c>div#results</c>.</para>
    /// </remarks>
    public const string Target = "HX-Target";

    /// <summary>
    /// The <c>HX-Trigger-Name</c> header contains the name of the triggered element, if present.
    /// </summary>
    /// <remarks>
    /// Supported only in HTMX 1.x and 2.x. HTMX 4.x identifies the source element with <c>HX-Source</c> instead.
    /// </remarks>
    public const string TriggerName = "HX-Trigger-Name";

    /// <summary>
    /// The <c>HX-Trigger</c> header contains the ID of the triggered element, if present.
    /// </summary>
    /// <remarks>
    /// Supported only in HTMX 1.x and 2.x. HTMX 4.x identifies the source element with <c>HX-Source</c> instead.
    /// </remarks>
    public const string Trigger = "HX-Trigger";
}
