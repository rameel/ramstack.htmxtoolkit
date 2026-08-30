using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    [MaybeNull]
    public string Location
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Location);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Location, value);
    }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Push-Url</c> header, which pushes a new URL
    /// onto the browser's history stack.
    /// </summary>
    [MaybeNull]
    public string PushUrl
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.PushUrl);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.PushUrl, value);
    }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Redirect</c> header, which performs
    /// a client-side redirect to a new location.
    /// </summary>
    [MaybeNull]
    public string Redirect
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Redirect);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Redirect, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>HX-Refresh</c> header requests a full-page refresh.
    /// </summary>
    public bool Refresh
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Refresh) == "true";
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Refresh, value ? "true" : null);
    }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Replace-Url</c> header, which replaces the current URL
    /// without pushing a new entry to the browser's history stack.
    /// </summary>
    [MaybeNull]
    public string ReplaceUrl
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.ReplaceUrl);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.ReplaceUrl, value);
    }

    /// <summary>
    /// Gets or sets the swap style specified by the <c>HX-Reswap</c> header.
    /// </summary>
    [DisallowNull]
    public HtmxSwap? Reswap
    {
        get => EnumHelper.ParseHtmxSwap(GetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap));
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap, value.GetValueOrDefault().GetSwapValue());
    }

    /// <summary>
    /// Gets or sets the complete <c>HX-Reswap</c> header value, including any swap modifiers.
    /// </summary>
    [MaybeNull]
    public string ReswapExpression
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap, value);
    }

    /// <summary>
    /// Gets or sets the CSS selector specified by the <c>HX-Retarget</c> header
    /// to change the target of the content update.
    /// </summary>
    [MaybeNull]
    public string Retarget
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Retarget);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Retarget, value);
    }

    /// <summary>
    /// Gets or sets the CSS selector specified by the <c>HX-Reselect</c> header
    /// to determine which part of the response is swapped in.
    /// </summary>
    [MaybeNull]
    public string Reselect
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Reselect);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reselect, value);
    }

    /// <summary>
    /// Gets or sets the client-side events to trigger through the <c>HX-Trigger</c> header.
    /// </summary>
    /// <remarks>
    /// Event values are accumulated for the current response and serialized
    /// into the header immediately before the response starts.
    /// </remarks>
    [MaybeNull]
    public IReadOnlyDictionary<string, object> Trigger
    {
        get => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.Receive);
        set => PendingEvents.GetOrCreate(_response).SetEvents(HtmxTriggerTiming.Receive, value);
    }

    /// <summary>
    /// Gets or sets the client-side events to trigger through
    /// the <c>HX-Trigger-After-Swap</c> header after the swap step.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Event values are accumulated for the current response and serialized
    ///   into the header immediately before the response starts.
    /// </para>
    /// <para>This header is supported only in HTMX 1.x and 2.x.</para>
    /// </remarks>
    [MaybeNull]
    public IReadOnlyDictionary<string, object> TriggerAfterSwap
    {
        get => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.AfterSwap);
        set => PendingEvents.GetOrCreate(_response).SetEvents(HtmxTriggerTiming.AfterSwap, value);
    }

    /// <summary>
    /// Gets or sets the client-side events to trigger through
    /// the <c>HX-Trigger-After-Settle</c> header after the settle step.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Event values are accumulated for the current response and serialized
    ///   into the header immediately before the response starts.
    /// </para>
    /// <para>This header is supported only in HTMX 1.x and 2.x.</para>
    /// </remarks>
    [MaybeNull]
    public IReadOnlyDictionary<string, object> TriggerAfterSettle
    {
        get => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.AfterSettle);
        set => PendingEvents.GetOrCreate(_response).SetEvents(HtmxTriggerTiming.AfterSettle, value);
    }

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
    /// Sets the specified header when <paramref name="value" /> is not <see langword="null" />.
    /// </summary>
    /// <param name="headers">The header collection to update.</param>
    /// <param name="key">The name of the header.</param>
    /// <param name="value">The header value.</param>
    private static void SetHeader(IHeaderDictionary headers, string key, string? value)
    {
        if (value is not null)
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
        /// Gets the collection of HTTP response headers
        /// from the associated <see cref="HtmxResponseHeaders"/> instance
        /// as an array of key-value pairs.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetHeaders(headers._response.Headers);
    }

    #endregion
}
