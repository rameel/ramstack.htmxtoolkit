namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines constants for the well-known names of htmx request headers.
/// For more information, see https://htmx.org/reference/#request_headers
/// </summary>
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
    /// The <c>HX-Prompt</c> header contains the user response to an hx-prompt.
    /// </summary>
    public const string Prompt = "HX-Prompt";

    /// <summary>
    /// The <c>HX-Request</c> header is a general header sent with every htmx request. Always "true".
    /// </summary>
    public const string Request = "HX-Request";

    /// <summary>
    /// The <c>HX-Target</c> header contains the ID of the target element if it exists.
    /// </summary>
    public const string Target = "HX-Target";

    /// <summary>
    /// The <c>HX-Trigger-Name</c> header contains the name of the triggered element if it exists.
    /// </summary>
    public const string TriggerName = "HX-Trigger-Name";

    /// <summary>
    /// The <c>HX-Trigger</c> header contains the ID of the triggered element if it exists.
    /// </summary>
    public const string Trigger = "HX-Trigger";
}
