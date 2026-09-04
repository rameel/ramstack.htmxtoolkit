using System.Diagnostics;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

using Ramstack.HtmxToolkit.Internal;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an HTTP response whose HTMX response headers can be configured.
/// </summary>
/// <remarks>
/// Like <see cref="HttpContext" /> and <see cref="HttpResponse" /> themselves,
/// this type is not thread-safe. Its members should not be called concurrently
/// from multiple threads for the same request.
/// </remarks>
[DebuggerTypeProxy(typeof(HtmxResponseDebugView))]
public readonly struct HtmxResponse
{
    private readonly HttpResponse _response;

    /// <summary>
    /// Gets the strongly typed HTMX response headers.
    /// </summary>
    public HtmxResponseHeaders Headers => new(_response);

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxResponse" /> structure.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    internal HtmxResponse(HttpResponse response) =>
        _response = response;

    /// <summary>
    /// Sets the <c>HX-Location</c> header to perform a client-side redirect
    /// without a full-page reload.
    /// </summary>
    /// <param name="value">The path or serialized JSON options to assign to the header.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Location(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Location, value);

    /// <summary>
    /// Sets the <c>HX-Location</c> header to perform a client-side redirect
    /// without a full-page reload.
    /// </summary>
    /// <param name="path">The path to request.</param>
    /// <param name="options">The options used to issue the request.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Location(string path, HtmxLocationOptions options)
    {
        return LocationImpl(this, path, options);

        static HtmxResponse LocationImpl(HtmxResponse response, string path, HtmxLocationOptions options)
        {
            options.Path = path;

            var json = JsonSerializer.Serialize(options, HtmxLocationOptionsJsonSerializerContext.Default.HtmxLocationOptions);
            return SetHeader(response, HtmxResponseHeaderNames.Location, json);
        }
    }

    /// <summary>
    /// Sets the <c>HX-Push-Url</c> header to push a new URL onto the browser's history stack.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// The possible values for this header are:
    /// <list type="bullet">
    ///   <item>
    ///     A relative or same-origin absolute URL to be pushed into the location bar,
    ///     as supported by <see href="https://developer.mozilla.org/en-US/docs/Web/API/History/pushState">history.pushState()</see>.
    ///   </item>
    ///   <item>
    ///     <c>"false"</c>, which prevents the browser's history from being updated.
    ///   </item>
    /// </list>
    /// </remarks>
    public HtmxResponse PushUrl(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.PushUrl, value);

    /// <summary>
    /// Sets the <c>HX-Push-Url</c> header to <c>"false"</c> to prevent the browser's
    /// history from being updated.
    /// </summary>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse PreventPushUrl() =>
        SetHeader(this, HtmxResponseHeaderNames.PushUrl, "false");

    /// <summary>
    /// Sets the <c>HX-Redirect</c> header to perform a client-side redirect to a new location.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Redirect(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Redirect, value);

    /// <summary>
    /// Sets the <c>HX-Refresh</c> header to request a full-page refresh.
    /// </summary>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Refresh() =>
        SetHeader(this, HtmxResponseHeaderNames.Refresh, "true");

    /// <summary>
    /// Sets the <c>HX-Replace-Url</c> header to replace the current URL.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// The possible values for this header are:
    /// <list type="bullet">
    ///   <item>
    ///     A URL to replace the current URL in the location bar. This may be relative
    ///     or absolute, as supported by
    ///     <see href="https://developer.mozilla.org/en-US/docs/Web/API/History/replaceState">history.replaceState()</see>,
    ///     but must have the same origin as the current URL.
    ///   </item>
    ///   <item>
    ///     <c>"false"</c>, which prevents the browser's current URL from being updated.
    ///   </item>
    /// </list>
    /// </remarks>
    public HtmxResponse ReplaceUrl(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.ReplaceUrl, value);

    /// <summary>
    /// Sets the <c>HX-Replace-Url</c> header to <c>"false"</c> to prevent the browser's
    /// current URL from being updated.
    /// </summary>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse PreventReplaceUrl() =>
        SetHeader(this, HtmxResponseHeaderNames.ReplaceUrl, "false");

    /// <summary>
    /// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
    /// </summary>
    /// <param name="value">The swap style to assign to the header.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Reswap(HtmxSwap value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reswap, value.GetSwapValue());

    /// <summary>
    /// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Reswap(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reswap, value);

    /// <summary>
    /// Sets the <c>HX-Retarget</c> header to update the target of the content update
    /// to a different element on the page.
    /// </summary>
    /// <param name="value">The CSS selector to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Retarget(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Retarget, value);

    /// <summary>
    /// Sets the <c>HX-Reselect</c> header to select the part of the response to swap in.
    /// </summary>
    /// <param name="value">The CSS selector to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Reselect(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reselect, value);

    /// <summary>
    /// Adds a client-side event to the response header selected by <paramref name="trigger" />.
    /// </summary>
    /// <param name="eventName">The event name to trigger.</param>
    /// <param name="trigger">The event timing. Defaults to <see cref="HtmxTriggerTiming.Receive" />.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// In HTMX 4.x, every <see cref="HtmxTriggerTiming" /> value is emitted through
    /// <c>HX-Trigger</c> and runs when the request completes (after the swap whenever one is performed).
    /// See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </remarks>
    public HtmxResponse TriggerEvent(string eventName, HtmxTriggerTiming trigger = HtmxTriggerTiming.Receive) =>
        TriggerEvent(eventName, "", trigger);

    /// <summary>
    /// Adds a client-side event and its detail to the response header selected by
    /// <paramref name="timing" />.
    /// </summary>
    /// <remarks>
    /// In HTMX 4.x, every <see cref="HtmxTriggerTiming" /> value is emitted through <c>HX-Trigger</c>
    /// and runs when the request completes (after the swap whenever one is performed).
    /// See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </remarks>
    /// <param name="eventName">The event name to trigger.</param>
    /// <param name="detail">The event detail.</param>
    /// <param name="timing">The event timing. Defaults to <see cref="HtmxTriggerTiming.Receive" />.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse TriggerEvent(string eventName, object detail, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive)
    {
        return TriggerEventImpl(this, eventName, detail, timing);

        static HtmxResponse TriggerEventImpl(HtmxResponse response, string eventName, object detail, HtmxTriggerTiming timing) =>
            AddEvents(response, new Dictionary<string, object> { [eventName] = detail }, timing);
    }

    /// <summary>
    /// Adds client-side events to the response header selected by <paramref name="timing" />.
    /// </summary>
    /// <remarks>
    /// In HTMX 4.x, every <see cref="HtmxTriggerTiming" /> value is emitted through <c>HX-Trigger</c>
    /// and runs when the request completes (after the swap whenever one is performed).
    /// See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </remarks>
    /// <param name="events">The event names and their associated details.</param>
    /// <param name="timing">The event timing. Defaults to <see cref="HtmxTriggerTiming.Receive" />.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse TriggerEvents(IReadOnlyDictionary<string, object> events, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive) =>
        AddEvents(this, events, timing);

    /// <summary>
    /// Sets a response header and returns the response wrapper for fluent chaining.
    /// </summary>
    /// <param name="response">The response wrapper to update.</param>
    /// <param name="key">The name of the header.</param>
    /// <param name="value">The header value.</param>
    /// <returns>
    /// The updated response wrapper.
    /// </returns>
    private static HtmxResponse SetHeader(HtmxResponse response, string key, string value)
    {
        response._response.Headers[key] = [with(value)];
        return response;
    }

    /// <summary>
    /// Adds pending client-side events and returns the response wrapper for fluent chaining.
    /// </summary>
    /// <param name="response">The response wrapper to update.</param>
    /// <param name="events">The event names and their associated details.</param>
    /// <param name="timing">The time at which to trigger the events.</param>
    /// <returns>
    /// The updated response wrapper.
    /// </returns>
    private static HtmxResponse AddEvents(HtmxResponse response, IReadOnlyDictionary<string, object> events, HtmxTriggerTiming timing)
    {
        PendingEvents.GetOrCreate(response._response).AddEvents(timing, events);
        return response;
    }

    #region Inner type: HtmxResponseDebugView

    /// <summary>
    /// Provides a debugger view for <see cref="HtmxResponse"/>.
    /// </summary>
    /// <param name="response">The <see cref="HtmxResponse"/> instance
    /// whose response headers will be displayed.</param>
    private sealed class HtmxResponseDebugView(HtmxResponse response)
    {
        /// <summary>
        /// Gets the collection of HTTP response headers
        /// from the associated <see cref="HtmxResponse"/> instance
        /// as an array of key-value pairs.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetHeaders(response._response.Headers);
    }

    #endregion
}
