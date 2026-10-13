using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
            var view = new HtmxLocationOptionsJsonView(path, options);
            var json = JsonSerializer.Serialize(view, HtmxLocationOptionsJsonSerializerContext.Default.HtmxLocationOptionsJsonView);

            return SetHeader(response, HtmxResponseHeaderNames.Location, json);
        }
    }

    /// <summary>
    /// Sets the <c>HX-Push-Url</c> header to push a new URL onto the browser's history stack.
    /// </summary>
    /// <param name="url">
    /// A relative or same-origin absolute URL to push into the location bar,
    /// as supported by <see href="https://developer.mozilla.org/en-US/docs/Web/API/History/pushState">history.pushState()</see>.
    /// </param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   Also removes the <c>HX-Replace-Url</c> header and the legacy <c>HX-Push</c> header.
    ///   HTMX 1.x and 2.x check <c>HX-Push</c> before the other history headers.
    ///   When <see cref="PushUrl" />, <see cref="ReplaceUrl" />, and <see cref="PreventHistoryUpdate" />
    ///   are combined, the last call takes effect.
    /// </para>
    /// <para>
    ///   HTMX treats <c>"false"</c> as a control value; use <see cref="PreventHistoryUpdate" /> instead.
    ///   HTMX 4.x also treats <c>"true"</c> as a control value and replaces it with the response URL,
    ///   whereas HTMX 1.x and 2.x push it as the relative URL <c>true</c>.
    /// </para>
    /// </remarks>
    public HtmxResponse PushUrl(string url)
    {
        Argument.ThrowIfNullOrEmpty(url);

        return SetHistoryHeader(this, HtmxResponseHeaderNames.PushUrl, url, HtmxResponseHeaderNames.ReplaceUrl);
    }

    /// <summary>
    /// Sets the <c>HX-Replace-Url</c> header to replace the current URL.
    /// </summary>
    /// <param name="url">
    /// A relative or same-origin absolute URL to replace the current URL in the location bar,
    /// as supported by <see href="https://developer.mozilla.org/en-US/docs/Web/API/History/replaceState">history.replaceState()</see>.
    /// </param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   Also removes the <c>HX-Push-Url</c> header and the legacy <c>HX-Push</c> header.
    ///   HTMX 1.x and 2.x check <c>HX-Push</c> before the other history headers.
    ///   When <see cref="PushUrl" />, <see cref="ReplaceUrl" />, and <see cref="PreventHistoryUpdate" />
    ///   are combined, the last call takes effect.
    /// </para>
    /// <para>
    ///   HTMX treats <c>"false"</c> as a control value; use <see cref="PreventHistoryUpdate" /> instead.
    ///   HTMX 4.x also treats <c>"true"</c> as a control value and replaces it with the response URL,
    ///   whereas HTMX 1.x and 2.x use it as the relative URL <c>true</c>.
    /// </para>
    /// </remarks>
    public HtmxResponse ReplaceUrl(string url)
    {
        Argument.ThrowIfNullOrEmpty(url);

        return SetHistoryHeader(this, HtmxResponseHeaderNames.ReplaceUrl, url, HtmxResponseHeaderNames.PushUrl);
    }

    /// <summary>
    /// Sets the <c>HX-Push-Url</c> header to <c>"false"</c> to prevent the browser's
    /// history from being updated.
    /// </summary>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   Also removes the <c>HX-Replace-Url</c> header and the legacy <c>HX-Push</c> header.
    ///   HTMX 1.x and 2.x check <c>HX-Push</c> before the other history headers.
    ///   When <see cref="PushUrl" />, <see cref="ReplaceUrl" />, and <see cref="PreventHistoryUpdate" />
    ///   are combined, the last call takes effect.
    /// </para>
    /// <para>
    ///   The history is not updated even when the <c>hx-push-url</c> or <c>hx-replace-url</c> attribute
    ///   or boosted navigation requests it.
    /// </para>
    /// </remarks>
    public HtmxResponse PreventHistoryUpdate() =>
        SetHistoryHeader(this, HtmxResponseHeaderNames.PushUrl, "false", HtmxResponseHeaderNames.ReplaceUrl);

    /// <summary>
    /// Sets the <c>HX-Redirect</c> header to perform a client-side redirect with a full-page reload.
    /// </summary>
    /// <param name="value">The URL to redirect to.</param>
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
    /// <param name="value">The swap style, optionally followed by swap modifiers.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Reswap(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reswap, value);

    /// <summary>
    /// Sets the <c>HX-Retarget</c> header to change the target of the content update
    /// to a different element on the page.
    /// </summary>
    /// <param name="value">The CSS selector to assign to the header.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Retarget(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Retarget, value);

    /// <summary>
    /// Sets the <c>HX-Reselect</c> header to select the part of the response to swap in.
    /// </summary>
    /// <param name="value">The CSS selector to assign to the header.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    public HtmxResponse Reselect(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reselect, value);

    /// <summary>
    /// Adds a client-side event to the response header selected by <paramref name="timing" />.
    /// </summary>
    /// <param name="eventName">The event name to trigger.</param>
    /// <param name="timing">The event timing. Defaults to <see cref="HtmxTriggerTiming.Receive" />.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   HTMX 4.x has only the <c>HX-Trigger</c> header, so events with any <see cref="HtmxTriggerTiming" /> value
    ///   are emitted through it. These events fire when the request completes, which is after the swap
    ///   and settle steps whenever a swap is performed.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// <para>
    ///   Events in the same header are triggered in the order they were added. If the same event name
    ///   is added more than once for a header, all events in that header are sent in a form that only
    ///   the Toolkit script (<c>htmx-toolkit.js</c>) can replay; without the script, none of them
    ///   is triggered. In HTMX 4.x, this also applies to a name added with different timings.
    ///   The event name <c>rs:event</c> is reserved for this purpose and cannot be used.
    /// </para>
    /// </remarks>
    public HtmxResponse TriggerEvent(string eventName, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive) =>
        AddToPendingEvent(this, eventName, "{}", timing);

    /// <summary>
    /// Adds a client-side event and its detail to the response header selected by
    /// <paramref name="timing" />.
    /// </summary>
    /// <param name="eventName">The event name to trigger.</param>
    /// <param name="detail">The event detail.</param>
    /// <param name="timing">The event timing. Defaults to <see cref="HtmxTriggerTiming.Receive" />.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   HTMX 4.x has only the <c>HX-Trigger</c> header, so events with any <see cref="HtmxTriggerTiming" /> value
    ///   are emitted through it. These events fire when the request completes, which is after the swap
    ///   and settle steps whenever a swap is performed.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// <para>
    ///   Events in the same header are triggered in the order they were added. If the same event name
    ///   is added more than once for a header, all events in that header are sent in a form that only
    ///   the Toolkit script (<c>htmx-toolkit.js</c>) can replay; without the script, none of them
    ///   is triggered. In HTMX 4.x, this also applies to a name added with different timings.
    ///   The event name <c>rs:event</c> is reserved for this purpose and cannot be used.
    /// </para>
    /// </remarks>
    [RequiresDynamicCode("Event details are serialized using reflection. Use the TriggerEvent overload that accepts JsonTypeInfo<T> for Native AOT applications.")]
    [RequiresUnreferencedCode("Event details are serialized using reflection. Use the TriggerEvent overload that accepts JsonTypeInfo<T> for trimmed applications.")]
    public HtmxResponse TriggerEvent(string eventName, object detail, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive)
    {
        return TriggerEventImpl(this, eventName, detail, timing);

        static HtmxResponse TriggerEventImpl(HtmxResponse response, string eventName, object detail, HtmxTriggerTiming timing) =>
            TriggerEventCore(response, eventName, detail, timing);
    }

    /// <summary>
    /// Adds a client-side event and its detail, serialized with the specified JSON metadata,
    /// to the response header selected by <paramref name="timing" />.
    /// </summary>
    /// <typeparam name="T">The event detail type.</typeparam>
    /// <param name="eventName">The event name to trigger.</param>
    /// <param name="detail">The event detail.</param>
    /// <param name="jsonTypeInfo">
    /// The JSON metadata for the event detail. Use source-generated metadata for trimming and Native AOT.
    /// </param>
    /// <param name="timing">The event timing. Defaults to <see cref="HtmxTriggerTiming.Receive" />.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse" /> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    ///   HTMX 4.x has only the <c>HX-Trigger</c> header, so events with any <see cref="HtmxTriggerTiming" /> value
    ///   are emitted through it. These events fire when the request completes, which is after the swap
    ///   and settle steps whenever a swap is performed.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// <para>
    ///   Events in the same header are triggered in the order they were added. If the same event name
    ///   is added more than once for a header, all events in that header are sent in a form that only
    ///   the Toolkit script (<c>htmx-toolkit.js</c>) can replay; without the script, none of them
    ///   is triggered. In HTMX 4.x, this also applies to a name added with different timings.
    ///   The event name <c>rs:event</c> is reserved for this purpose and cannot be used.
    /// </para>
    /// </remarks>
    public HtmxResponse TriggerEvent<T>(string eventName, T detail, JsonTypeInfo<T> jsonTypeInfo, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive) =>
        TriggerEventCore(this, eventName, detail, jsonTypeInfo, timing);

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
    /// Sets a browser history header and removes the other history headers, including the legacy
    /// <c>HX-Push</c> header, so that the last history call takes effect.
    /// </summary>
    /// <param name="response">The response wrapper to update.</param>
    /// <param name="key">The name of the history header to set.</param>
    /// <param name="value">The header value.</param>
    /// <param name="conflictingKey">The name of the history header to remove.</param>
    /// <returns>
    /// The updated response wrapper.
    /// </returns>
    private static HtmxResponse SetHistoryHeader(HtmxResponse response, string key, string value, string conflictingKey)
    {
        // Remove the legacy "HX-Push" header:
        // - HTMX 1.x and 2.x check it before "HX-Push-Url" and "HX-Replace-Url";
        // - HTMX 4.x ignores it.
        response._response.Headers.Remove("HX-Push");

        response._response.Headers.Remove(conflictingKey);
        return SetHeader(response, key, value);
    }

    [RequiresDynamicCode("Event details are serialized using reflection.")]
    [RequiresUnreferencedCode("Event details are serialized using reflection.")]
    private static HtmxResponse TriggerEventCore(HtmxResponse response, string eventName, object detail, HtmxTriggerTiming timing) =>
        AddToPendingEvent(response, eventName, JsonSerializer.Serialize(detail, JsonOptions.CamelCase), timing);

    private static HtmxResponse TriggerEventCore<T>(HtmxResponse response, string eventName, T detail, JsonTypeInfo<T> jsonTypeInfo, HtmxTriggerTiming timing) =>
        AddToPendingEvent(response, eventName, SerializeEventDetail(detail, jsonTypeInfo), timing);

    private static string SerializeEventDetail<T>(T detail, JsonTypeInfo<T> jsonTypeInfo)
    {
        Argument.ThrowIfNull(jsonTypeInfo);

        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Encoder = JsonOptions.Encoder, SkipValidation = true }))
            JsonSerializer.Serialize(writer, detail, jsonTypeInfo);

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static HtmxResponse AddToPendingEvent(HtmxResponse response, string eventName, string detailJson, HtmxTriggerTiming timing)
    {
        PendingEvents.GetOrCreate(response._response).AddEvent(timing, eventName, detailJson);
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
        /// Gets the collection of HTMX response headers
        /// from the associated <see cref="HtmxResponse"/> instance
        /// as an array of key-value pairs.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetHeaders(response._response.Headers);
    }

    #endregion
}
