using System.Diagnostics;
using Microsoft.AspNetCore.Http;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents strongly typed HTMX response headers.
/// </summary>
[DebuggerTypeProxy(typeof(HtmxResponseHeadersDebugView))]
public readonly struct HtmxResponseHeaders
{
    private readonly HttpResponse _response;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxResponseHeaders" /> structure.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    internal HtmxResponseHeaders(HttpResponse response) =>
        _response = response;

    /// <summary>
    /// Gets or sets the value of the <c>HX-Location</c> header, which performs
    /// a client-side redirect without a full-page reload.
    /// </summary>
    /// <remarks>
    /// If the header is not present, the property returns <see langword="null" />.
    /// Assigning <see langword="null" /> or an empty string removes the header.
    /// </remarks>
    public string? Location
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Location);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Location, value);
    }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Push-Url</c> header, which pushes a new URL
    /// onto the browser's history stack.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   If the header is not present, the property returns <see langword="null" />.
    ///   Assigning <see langword="null" /> or an empty string removes the header.
    /// </para>
    /// <para>
    ///   Unlike <see cref="HtmxResponse.PushUrl" />, setting this property does not remove
    ///   the <c>HX-Replace-Url</c> header or the legacy <c>HX-Push</c> header.
    /// </para>
    /// </remarks>
    public string? PushUrl
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.PushUrl);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.PushUrl, value);
    }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Replace-Url</c> header, which replaces the current URL
    /// without pushing a new entry to the browser's history stack.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   If the header is not present, the property returns <see langword="null" />.
    ///   Assigning <see langword="null" /> or an empty string removes the header.
    /// </para>
    /// <para>
    ///   Unlike <see cref="HtmxResponse.ReplaceUrl" />, setting this property does not remove
    ///   the <c>HX-Push-Url</c> header or the legacy <c>HX-Push</c> header.
    /// </para>
    /// </remarks>
    public string? ReplaceUrl
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.ReplaceUrl);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.ReplaceUrl, value);
    }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Redirect</c> header, which performs
    /// a client-side redirect with a full-page reload.
    /// </summary>
    /// <remarks>
    /// If the header is not present, the property returns <see langword="null" />.
    /// Assigning <see langword="null" /> or an empty string removes the header.
    /// </remarks>
    public string? Redirect
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Redirect);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Redirect, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>HX-Refresh</c> header requests a full-page refresh.
    /// </summary>
    /// <remarks>Assigning <see langword="false" /> removes the header.</remarks>
    public bool Refresh
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Refresh) == "true";
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Refresh, value ? "true" : null);
    }

    /// <summary>
    /// Gets or sets the swap style specified by the <c>HX-Reswap</c> header.
    /// </summary>
    /// <remarks>
    /// Reads the style from <see cref="ReswapExpression" />, or returns <see langword="null" />
    /// if the header is not present or the style is not recognized. Assigning a style replaces
    /// the complete header value, including any modifiers. Assigning <see langword="null" /> removes the header.
    /// </remarks>
    public HtmxSwap? Reswap
    {
        get => EnumHelper.ParseHtmxSwap(GetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap));
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap, value?.GetSwapValue());
    }

    /// <summary>
    /// Gets or sets the complete <c>HX-Reswap</c> header value, including any swap modifiers.
    /// </summary>
    /// <remarks>
    /// If the header is not present, the property returns <see langword="null" />.
    /// Assigning <see langword="null" /> or an empty string removes the header.
    /// </remarks>
    public string? ReswapExpression
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap, value);
    }

    /// <summary>
    /// Gets or sets the CSS selector specified by the <c>HX-Retarget</c> header
    /// to change the target of the content update.
    /// </summary>
    /// <remarks>
    /// If the header is not present, the property returns <see langword="null" />.
    /// Assigning <see langword="null" /> or an empty string removes the header.
    /// </remarks>
    public string? Retarget
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Retarget);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Retarget, value);
    }

    /// <summary>
    /// Gets or sets the CSS selector specified by the <c>HX-Reselect</c> header
    /// to determine which part of the response is swapped in.
    /// </summary>
    /// <remarks>
    /// If the header is not present, the property returns <see langword="null" />.
    /// Assigning <see langword="null" /> or an empty string removes the header.
    /// </remarks>
    public string? Reselect
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Reselect);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reselect, value);
    }

    /// <summary>
    /// Gets the client-side events to trigger through the <c>HX-Trigger</c> header.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Each entry pairs an event name with its detail serialized as JSON. Events are listed
    ///   in the order they were added, including repeated names. The list is a read-only live view
    ///   of the events accumulated for the current response, which are written into the header
    ///   immediately before the response starts. If no events have been added,
    ///   the property returns <see langword="null" />.
    /// </para>
    /// <para>
    ///   HTMX 1.9.x and 2.x trigger these events when the response is received,
    ///   whereas HTMX 4.x triggers them when the request completes
    ///   (after the swap and settle steps whenever a swap is performed).
    ///   In HTMX 4.x, the list also includes the events added with other timings,
    ///   because all timings share the <c>HX-Trigger</c> header.
    /// </para>
    /// </remarks>
    public IReadOnlyList<KeyValuePair<string, string>>? Trigger => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.Receive);

    /// <summary>
    /// Gets the client-side events to trigger through the <c>HX-Trigger-After-Swap</c> header after the swap step.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Each entry pairs an event name with its detail serialized as JSON. Events are listed
    ///   in the order they were added, including repeated names. The list is a read-only live view
    ///   of the events accumulated for the current response, which are written into the header
    ///   immediately before the response starts. If no events have been added,
    ///   the property returns <see langword="null" />.
    /// </para>
    /// <para>
    ///   HTMX 4.x has no <c>HX-Trigger-After-Swap</c> header, so these events are emitted through
    ///   <c>HX-Trigger</c> when the request completes (after the swap and settle steps whenever
    ///   a swap is performed). The property therefore returns the same list as <see cref="Trigger" />,
    ///   including the events added with other timings.
    /// </para>
    /// </remarks>
    public IReadOnlyList<KeyValuePair<string, string>>? TriggerAfterSwap => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.AfterSwap);

    /// <summary>
    /// Gets the client-side events to trigger through the <c>HX-Trigger-After-Settle</c> header after the settle step.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Each entry pairs an event name with its detail serialized as JSON. Events are listed
    ///   in the order they were added, including repeated names. The list is a read-only live view
    ///   of the events accumulated for the current response, which are written into the header
    ///   immediately before the response starts. If no events have been added,
    ///   the property returns <see langword="null" />.
    /// </para>
    /// <para>
    ///   HTMX 4.x has no <c>HX-Trigger-After-Settle</c> header, so these events are emitted through
    ///   <c>HX-Trigger</c> when the request completes (after the swap and settle steps whenever
    ///   a swap is performed). The property therefore returns the same list as <see cref="Trigger" />,
    ///   including the events added with other timings.
    /// </para>
    /// </remarks>
    public IReadOnlyList<KeyValuePair<string, string>>? TriggerAfterSettle => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.AfterSettle);

    /// <summary>
    /// Gets the value of the specified header.
    /// </summary>
    /// <param name="headers">The header collection to inspect.</param>
    /// <param name="key">The name of the header.</param>
    /// <returns>
    /// The header value, or <see langword="null" /> if the header is not present.
    /// </returns>
    private static string? GetHeader(IHeaderDictionary headers, string key)
    {
        headers.TryGetValue(key, out var values);
        return values;
    }

    /// <summary>
    /// Sets the specified header, or removes it when <paramref name="value" />
    /// is <see langword="null" /> or empty.
    /// </summary>
    /// <param name="headers">The header collection to update.</param>
    /// <param name="key">The name of the header.</param>
    /// <param name="value">The header value.</param>
    private static void SetHeader(IHeaderDictionary headers, string key, string? value)
    {
        if (string.IsNullOrEmpty(value))
            headers.Remove(key);
        else
            headers[key] = value;
    }

    #region Inner type: HtmxResponseHeadersDebugView

    /// <summary>
    /// Provides a debugger view for <see cref="HtmxResponseHeaders"/>.
    /// </summary>
    /// <param name="headers">The <see cref="HtmxResponseHeaders"/> instance
    /// whose response headers will be displayed.</param>
    private sealed class HtmxResponseHeadersDebugView(HtmxResponseHeaders headers)
    {
        /// <summary>
        /// Gets the collection of HTMX response headers
        /// from the associated <see cref="HtmxResponseHeaders"/> instance
        /// as an array of key-value pairs.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetHeaders(headers._response.Headers);
    }

    #endregion
}
