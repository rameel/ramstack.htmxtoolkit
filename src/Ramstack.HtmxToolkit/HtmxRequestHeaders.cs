using System.Diagnostics;

using Microsoft.AspNetCore.Http;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents strongly typed HTMX request headers.
/// </summary>
[DebuggerTypeProxy(typeof(HtmxRequestHeadersDebugView))]
public readonly struct HtmxRequestHeaders
{
    private readonly IHeaderDictionary _headers;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxRequestHeaders"/>.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    internal HtmxRequestHeaders(HttpRequest request) =>
        _headers = request.Headers;

    /// <summary>
    /// Gets a value indicating whether the request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.Boosted"/></remarks>
    public bool Boosted => GetBoolean(_headers, HtmxRequestHeaderNames.Boosted);

    /// <summary>
    /// Gets the current URL of the browser.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.CurrentUrl"/></remarks>
    public string? CurrentUrl => GetString(_headers, HtmxRequestHeaderNames.CurrentUrl);

    /// <summary>
    /// Gets a value indicating whether the request is for history restoration after a miss in the local history cache.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.HistoryRestoreRequest"/></remarks>
    public bool HistoryRestoreRequest => GetBoolean(_headers, HtmxRequestHeaderNames.HistoryRestoreRequest);

    /// <summary>
    /// Gets the user response to an hx-prompt on the client.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.Prompt"/></remarks>
    public string? Prompt => GetString(_headers, HtmxRequestHeaderNames.Prompt);

    /// <summary>
    /// Gets a value indicating whether the current request is htmx request.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.Request"/></remarks>
    public bool Request => GetBoolean(_headers, HtmxRequestHeaderNames.Request);

    /// <summary>
    /// Gets the ID of the target element if it exists.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.Target"/></remarks>
    public string? Target => GetString(_headers, HtmxRequestHeaderNames.Target);

    /// <summary>
    /// Gets the name of the triggered element if it exists.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.TriggerName"/></remarks>
    public string? TriggerName => GetString(_headers, HtmxRequestHeaderNames.TriggerName);

    /// <summary>
    /// Gets the ID of the triggered element if it exists as indicated by the <c>HX-Trigger</c> header.
    /// </summary>
    /// <remarks><see cref="HtmxRequestHeaderNames.Trigger"/></remarks>
    public string? Trigger => GetString(_headers, HtmxRequestHeaderNames.Trigger);

    private static bool GetBoolean(IHeaderDictionary dictionary, string key) =>
        dictionary.TryGetValue(key, out var value) && value[0] == "true";

    private static string? GetString(IHeaderDictionary dictionary, string key)
    {
        dictionary.TryGetValue(key, out var value);
        return value;
    }

    #region Inner type: HtmxRequestHeadersDebugView

    private sealed class HtmxRequestHeadersDebugView(HtmxRequestHeaders headers)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetRequestHeaders(headers._headers);
    }

    #endregion
}
