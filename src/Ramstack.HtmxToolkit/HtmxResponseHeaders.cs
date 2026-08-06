using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents strongly typed HTMX response headers.
/// </summary>
[DebuggerTypeProxy(typeof(HtmxResponseHeadersDebugView))]
public sealed class HtmxResponseHeaders
{
    private readonly HttpResponse _response;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxResponseHeaders"/> structure.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    internal HtmxResponseHeaders(HttpResponse response) =>
        _response = response;

    /// <summary>
    /// Gets or sets the <c>HX-Location</c> header to perform a client-side redirect without a full page reload.
    /// </summary>
    [MaybeNull]
    public string Location
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Location);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Location, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Push-Url</c> header to push a new URL into the browser's history stack.
    /// </summary>
    [MaybeNull]
    public string PushUrl
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.PushUrl);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.PushUrl, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Redirect</c> header to perform a client-side redirect to a new location.
    /// </summary>
    [MaybeNull]
    public string Redirect
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Redirect);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Redirect, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Refresh</c> header to perform a full page refresh.
    /// </summary>
    public bool Refresh
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Refresh) == "true";
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Refresh, value ? "true" : null);
    }

    /// <summary>
    /// Gets or Sets the <c>HX-Replace-Url</c> header to replace the current URL
    /// without pushing a new entry to the browser's history stack.
    /// </summary>
    [MaybeNull]
    public string ReplaceUrl
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.ReplaceUrl);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.ReplaceUrl, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Reswap</c> header to specify how the response will be swapped into the DOM.
    /// </summary>
    [DisallowNull]
    public HtmxSwap? Reswap
    {
        get => EnumHelper.ParseHtmxSwap(GetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap));
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap, value.GetValueOrDefault().GetSwapValue());
    }

    /// <summary>
    /// Gets or sets the <c>HX-Reswap</c> header to specify how the response will be swapped into the DOM.
    /// </summary>
    [MaybeNull]
    public string ReswapExpression
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reswap, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Retarget</c> header that specifies a selector to change
    /// the target of the content update to a different element on the page.
    /// </summary>
    [MaybeNull]
    public string Retarget
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Retarget);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Retarget, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Reselect</c> header that specifies a selector
    /// to choose which part of the response content will be swapped in.
    /// </summary>
    [MaybeNull]
    public string Reselect
    {
        get => GetHeader(_response.Headers, HtmxResponseHeaderNames.Reselect);
        set => SetHeader(_response.Headers, HtmxResponseHeaderNames.Reselect, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Trigger</c> header, which specifies a dictionary of client-side events
    /// to trigger after the server response is processed.
    /// </summary>
    /// <remarks>
    /// Event values are accumulated for the current response and serialized into the header immediately
    /// before the response starts.
    /// </remarks>
    [MaybeNull]
    public IReadOnlyDictionary<string, object> Trigger
    {
        get => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.Receive);
        set => PendingEvents.GetOrCreate(_response).SetEvents(HtmxTriggerTiming.Receive, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Trigger-After-Settle</c> header, which specifies a dictionary of client-side events
    /// to trigger after the htmx request has settled.
    /// </summary>
    /// <remarks>
    /// Event values are accumulated for the current response and serialized into the header immediately
    /// before the response starts.
    /// </remarks>
    [MaybeNull]
    public IReadOnlyDictionary<string, object> TriggerAfterSettle
    {
        get => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.AfterSettle);
        set => PendingEvents.GetOrCreate(_response).SetEvents(HtmxTriggerTiming.AfterSettle, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Trigger-After-Swap</c> header, which specifies a dictionary of client-side events
    /// to trigger after the response content has been swapped into the DOM.
    /// </summary>
    /// <remarks>
    /// Event values are accumulated for the current response and serialized into the header immediately
    /// before the response starts.
    /// </remarks>
    [MaybeNull]
    public IReadOnlyDictionary<string, object> TriggerAfterSwap
    {
        get => PendingEvents.TryGet(_response)?.GetEvents(HtmxTriggerTiming.AfterSwap);
        set => PendingEvents.GetOrCreate(_response).SetEvents(HtmxTriggerTiming.AfterSwap, value);
    }

    private static string? GetHeader(IHeaderDictionary headers, string key)
    {
        headers.TryGetValue(key, out var values);
        return values;
    }

    private static void SetHeader(IHeaderDictionary headers, string key, string? value)
    {
        if (value is not null)
            headers[key] = value;
    }

    #region Inner type: HtmxResponseHeadersDebugView

    private sealed class HtmxResponseHeadersDebugView(HtmxResponseHeaders headers)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetHeaders(headers._response.Headers);
    }

    #endregion
}
