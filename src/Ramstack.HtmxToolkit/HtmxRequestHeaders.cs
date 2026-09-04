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
    /// Initializes a new instance of the <see cref="HtmxRequestHeaders" /> structure.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    internal HtmxRequestHeaders(HttpRequest request) =>
        _headers = request.Headers;

    /// <summary>
    /// Gets a value indicating whether the request was made
    /// using AJAX instead of a normal navigation.
    /// </summary>
    /// <remarks>
    /// The header name is <see cref="HtmxRequestHeaderNames.Boosted" />.
    /// </remarks>
    public bool Boosted => GetBoolean(_headers, HtmxRequestHeaderNames.Boosted);

    /// <summary>
    /// Gets the current URL of the browser.
    /// </summary>
    /// <remarks>
    /// The header name is <see cref="HtmxRequestHeaderNames.CurrentUrl" />.
    /// </remarks>
    public string? CurrentUrl => GetString(_headers, HtmxRequestHeaderNames.CurrentUrl);

    /// <summary>
    /// Gets a value indicating whether the request restores history
    /// after a miss in the local history cache.
    /// </summary>
    /// <remarks>
    /// The header name is <see cref="HtmxRequestHeaderNames.HistoryRestoreRequest" />.
    /// </remarks>
    public bool HistoryRestoreRequest => GetBoolean(_headers, HtmxRequestHeaderNames.HistoryRestoreRequest);

    /// <summary>
    /// Gets the user's response to an <c>hx-prompt</c> on the client.
    /// </summary>
    /// <remarks>
    /// <para>The header name is <see cref="HtmxRequestHeaderNames.Prompt" />.</para>
    /// <para>
    ///   Supported only in HTMX 1.x and 2.x; HTMX 4.x removed <c>hx-prompt</c>
    ///   and does not send this header.
    /// </para>
    /// </remarks>
    public string? Prompt => GetString(_headers, HtmxRequestHeaderNames.Prompt);

    /// <summary>
    /// Gets a value indicating whether the current request is an HTMX request.
    /// </summary>
    /// <remarks>
    /// The header name is <see cref="HtmxRequestHeaderNames.Request" />.
    /// </remarks>
    public bool Request => GetBoolean(_headers, HtmxRequestHeaderNames.Request);

    /// <summary>
    /// Gets the identifier of the target element, if present.
    /// </summary>
    /// <remarks>
    /// <para>The header name is <see cref="HtmxRequestHeaderNames.Target" />.</para>
    /// <para>In HTMX 1.x and 2.x, the value is the ID of the target element.</para>
    /// <para>In HTMX 4.x, the value is in <c>tag#id</c> format, for example <c>div#results</c>.</para>
    /// </remarks>
    public string? Target => GetString(_headers, HtmxRequestHeaderNames.Target);

    /// <summary>
    /// Gets the name of the triggered element, if present.
    /// </summary>
    /// <remarks>
    /// <para>The header name is <see cref="HtmxRequestHeaderNames.TriggerName" />.</para>
    /// <para>
    ///   Supported only in HTMX 1.x and 2.x; HTMX 4.x identifies the source element
    ///   with <c>HX-Source</c> instead.
    /// </para>
    /// </remarks>
    public string? TriggerName => GetString(_headers, HtmxRequestHeaderNames.TriggerName);

    /// <summary>
    /// Gets the ID of the triggered element, if present.
    /// </summary>
    /// <remarks>
    /// <para>The header name is <see cref="HtmxRequestHeaderNames.Trigger" />.</para>
    /// <para>
    ///   Supported only in HTMX 1.x and 2.x; HTMX 4.x identifies the source element  with <c>HX-Source</c> instead.
    /// </para>
    /// </remarks>
    public string? Trigger => GetString(_headers, HtmxRequestHeaderNames.Trigger);

    /// <summary>
    /// Determines whether the specified header has the value <c>"true"</c>.
    /// </summary>
    /// <param name="dictionary">The header collection to inspect.</param>
    /// <param name="key">The name of the header.</param>
    /// <returns>
    /// <see langword="true" /> if the header value is <c>"true"</c>; otherwise, <see langword="false" />.
    /// </returns>
    private static bool GetBoolean(IHeaderDictionary dictionary, string key) =>
        dictionary.TryGetValue(key, out var value) && value[0] == "true";

    /// <summary>
    /// Gets the value of the specified header.
    /// </summary>
    /// <param name="dictionary">The header collection to inspect.</param>
    /// <param name="key">The name of the header.</param>
    /// <returns>
    /// The header value, or <see langword="null" /> if the header is not present.
    /// </returns>
    private static string? GetString(IHeaderDictionary dictionary, string key)
    {
        dictionary.TryGetValue(key, out var value);
        return value;
    }

    #region Inner type: HtmxRequestHeadersDebugView

    /// <summary>
    /// Provides a debugger view for <see cref="HtmxRequestHeaders"/>.
    /// </summary>
    /// <param name="headers">The <see cref="HtmxRequestHeaders"/> instance
    /// whose headers will be displayed.</param>
    private sealed class HtmxRequestHeadersDebugView(HtmxRequestHeaders headers)
    {
        /// <summary>
        /// Gets the collection of all HTTP headers stored
        /// in the associated <see cref="HtmxRequestHeaders"/> instance
        /// as an array of key-value pairs.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetHeaders(headers._headers);
    }

    #endregion
}
