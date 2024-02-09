using System.Diagnostics;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents HTTP response to set htmx response headers.
/// </summary>
[DebuggerTypeProxy(typeof(HtmxResponseDebugView))]
public readonly struct HtmxResponse
{
    private readonly HttpResponse _response;

    /// <summary>
    /// The custom status code to stop the polling.
    /// </summary>
    public const int StopPollingStatusCode = 286;

    /// <summary>
    /// Gets the HTMX headers.
    /// </summary>
    public HtmxResponseHeaders Headers => new(_response);

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxResponse"/>.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    internal HtmxResponse(HttpResponse response) =>
        _response = response;

    /// <summary>
    /// Sets the <c>HX-Location</c> header to a client-side redirect that does not do a full page reload.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Location(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Location, value);

    /// <summary>
    /// Sets the <c>HX-Push-Url</c> header to push a new URL into the history stack.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse PushUrl(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.PushUrl, value);

    /// <summary>
    /// Sets the <c>HX-Redirect</c> header to a clent-side redirect to a new location.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Redirect(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Redirect, value);

    /// <summary>
    /// Sets the <c>HX-Refresh</c> header to full refresh of the page.
    /// </summary>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Refresh() =>
        SetHeader(this, HtmxResponseHeaderNames.Refresh, "true");

    /// <summary>
    /// Sets the <c>HX-Replace-Url</c> header to replace the current URL.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse ReplaceUrl(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.ReplaceUrl, value);

    /// <summary>
    /// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Reswap(HtmxSwap value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reswap, value.GetSwapValue());

    /// <summary>
    /// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
    /// </summary>
    /// <param name="value">The header value to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Reswap(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reswap, value);

    /// <summary>
    /// Sets the <c>HX-Retarget</c> header to update the target of the content update
    /// to a different element on the page.
    /// </summary>
    /// <param name="value">The CSS selector to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Retarget(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Retarget, value);

    /// <summary>
    /// Sets the <c>HX-Reselect</c> header to choose which part of the response is used to be swapped in.
    /// </summary>
    /// <param name="value">The CSS selector to set.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse Reselect(string value) =>
        SetHeader(this, HtmxResponseHeaderNames.Reselect, value);

    /// <summary>
    /// Sets one of <c>HX-Trigger</c> headers to trigger a client-side event.
    /// </summary>
    /// <param name="eventName">The event name to trigger.</param>
    /// <param name="trigger">The time at which the event will be triggered. Defaults to <see cref="HtmxTriggerTiming.Receive"/>.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse TriggerEvent(string eventName, HtmxTriggerTiming trigger = HtmxTriggerTiming.Receive) =>
        TriggerEvent(eventName, "", trigger);

    /// <summary>
    /// Sets one of <c>HX-Trigger</c> headers to trigger a client-side event.
    /// </summary>
    /// <param name="eventName">The event name to trigegr.</param>
    /// <param name="detail">The event detail.</param>
    /// <param name="timing">The time at which an event will be triggered. Defaults to <see cref="HtmxTriggerTiming.Receive"/>.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse TriggerEvent(string eventName, object detail, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive)
    {
        return TriggerEventImpl(this, eventName, detail, timing);

        static HtmxResponse TriggerEventImpl(HtmxResponse response, string eventName, object detail, HtmxTriggerTiming timing) =>
            SetEvents(response, new Dictionary<string, object> { [eventName] = detail }, timing);
    }

    /// <summary>
    /// Sets one of <c>HX-Trigger</c> headers to trigger a client-side events.
    /// </summary>
    /// <param name="events">A dictionary containing event names as keys and event details as values.</param>
    /// <param name="timing">The time at which an event will be triggered. Defaults to <see cref="HtmxTriggerTiming.Receive"/>.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse TriggerEvents(IReadOnlyDictionary<string, object> events, HtmxTriggerTiming timing = HtmxTriggerTiming.Receive) =>
        SetEvents(this, events, timing);

    /// <summary>
    /// Sets the special HTTP status code <c>286</c> that is used to stop the polling.
    /// </summary>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse StopPolling() =>
        StopPolling(true);

    /// <summary>
    /// Sets the special HTTP status code <c>286</c> to stop the polling.
    /// </summary>
    /// <param name="condition">A boolean condition indicating whether to stop the polling.</param>
    /// <returns>
    /// The current <see cref="HtmxResponse"/> instance.
    /// </returns>
    public HtmxResponse StopPolling(bool condition)
    {
        if (condition)
            _response.StatusCode = StopPollingStatusCode;

        return this;
    }

    private static HtmxResponse SetHeader(HtmxResponse response, string key, string value)
    {
        response._response.Headers[key] = new StringValues(value);
        return response;
    }

    private static HtmxResponse SetEvents(HtmxResponse response, IReadOnlyDictionary<string, object> events, HtmxTriggerTiming timing)
    {
        var key = timing switch
        {
            HtmxTriggerTiming.Receive => HtmxResponseHeaderNames.Trigger,
            HtmxTriggerTiming.AfterSettle => HtmxResponseHeaderNames.TriggerAfterSettle,
            _ => HtmxResponseHeaderNames.TriggerAfterSwap
        };

        if (response._response.Headers.TryGetValue(key, out var values))
        {
            var current = JsonSerializer.Deserialize<Dictionary<string, object>>(values.ToString())!;

            if (events is Dictionary<string, object> dictionary)
            {
                foreach (var (k, v) in dictionary)
                    current.TryAdd(k, v);
            }
            else
            {
                foreach (var (k, v) in events)
                    current.TryAdd(k, v);
            }

            events = current;
        }

        response._response.Headers[key] = JsonSerializer.Serialize(events, JsonOptions.CamelCase);
        return response;
    }

    #region Inner type: HtmxResponseDebugView

    private sealed class HtmxResponseDebugView(HtmxResponse response)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, string>[] Items => DebugHelpers.GetResponseHeaders(response._response.Headers);
    }

    #endregion
}
