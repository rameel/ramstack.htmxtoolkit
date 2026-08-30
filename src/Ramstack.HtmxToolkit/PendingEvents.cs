using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Accumulates HTMX events by <see cref="HtmxTriggerTiming" /> for a single request,
/// deferring header serialization until the response is about to start.
/// </summary>
internal sealed class PendingEvents
{
    private const string ProxyEventName = "rs:events";

    private readonly HttpResponse _response;
    private readonly HtmxTargetVersion _version;

    private SmallDictionary<string, object>? _receive;
    private SmallDictionary<string, object>? _afterSwap;
    private SmallDictionary<string, object>? _afterSettle;

    /// <summary>
    /// Initializes a new instance of the <see cref="PendingEvents" /> class.
    /// </summary>
    /// <param name="response">The HTTP response to which the events belong.</param>
    private PendingEvents(HttpResponse response) =>
        (_response, _version) = (response, GetTargetVersion(response));

    /// <summary>
    /// Adds the specified events to the pending set for <paramref name="timing" />.
    /// When an event name already exists, the duplicate is stored under the
    /// <c>rs:events</c> key for client-side replay.
    /// </summary>
    /// <param name="timing">The time at which to trigger the events.</param>
    /// <param name="events">The event names and their associated details.</param>
    public void AddEvents(HtmxTriggerTiming timing, IReadOnlyDictionary<string, object> events)
    {
        timing = NormalizeTiming(timing);

        var current = timing switch
        {
            HtmxTriggerTiming.Receive => _receive ??= new SmallDictionary<string, object>(StringComparer.Ordinal),
            HtmxTriggerTiming.AfterSwap => _afterSwap ??= new SmallDictionary<string, object>(StringComparer.Ordinal),
            _ => _afterSettle ??= new SmallDictionary<string, object>(StringComparer.Ordinal)
        };

        foreach (var (k, v) in events)
        {
            if (current.TryAdd(k, v))
                continue;

            if (!current.TryGetValue(ProxyEventName, out var value) || value is not List<KeyValuePair<string, object>> collection)
            {
                collection = [];
                current[ProxyEventName] = collection;
            }

            collection.Add(new KeyValuePair<string, object>(k, v));
        }
    }

    /// <summary>
    /// Returns the pending events for the specified <paramref name="timing" />.
    /// </summary>
    /// <param name="timing">The time at which to trigger the events.</param>
    /// <returns>
    /// The pending events, or <see langword="null" /> if none were registered.
    /// </returns>
    public IReadOnlyDictionary<string, object>? GetEvents(HtmxTriggerTiming timing)
    {
        timing = NormalizeTiming(timing);

        return timing switch
        {
            HtmxTriggerTiming.Receive => _receive,
            HtmxTriggerTiming.AfterSwap => _afterSwap,
            _ => _afterSettle
        };
    }

    /// <summary>
    /// Replaces the pending events for the specified <paramref name="timing" />.
    /// </summary>
    /// <param name="timing">The time at which to trigger the events.</param>
    /// <param name="events">The replacement event names and their associated details.</param>
    public void SetEvents(HtmxTriggerTiming timing, IReadOnlyDictionary<string, object> events)
    {
        timing = NormalizeTiming(timing);

        var replacement = new SmallDictionary<string, object>(events, StringComparer.Ordinal);
        switch (timing)
        {
            case HtmxTriggerTiming.Receive:
                _receive = replacement;
                break;
            case HtmxTriggerTiming.AfterSwap:
                _afterSwap = replacement;
                break;
            default:
                _afterSettle = replacement;
                break;
        }
    }

    /// <summary>
    /// Serializes the accumulated events, if any, into the corresponding
    /// <c>HX-Trigger</c> response headers.
    /// </summary>
    public void Flush()
    {
        SetHeader(HtmxResponseHeaderNames.Trigger, _receive);
        SetHeader(HtmxResponseHeaderNames.TriggerAfterSwap, _afterSwap);
        SetHeader(HtmxResponseHeaderNames.TriggerAfterSettle, _afterSettle);
    }

    /// <summary>
    /// Returns the events accumulator previously registered for the response.
    /// </summary>
    /// <param name="response">The HTTP response that owns the events.</param>
    /// <returns>
    /// The pending events accumulator, or <see langword="null" /> if none was registered.
    /// </returns>
    public static PendingEvents? TryGet(HttpResponse response) =>
        response.HttpContext.Items[typeof(PendingEvents)] as PendingEvents;

    /// <summary>
    /// Returns the pending events accumulator, creating and registering it
    /// for the response when necessary.
    /// </summary>
    /// <param name="response">The HTTP response that owns the events.</param>
    /// <returns>
    /// The pending events accumulator for the response.
    /// </returns>
    public static PendingEvents GetOrCreate(HttpResponse response)
    {
        var context = response.HttpContext;
        if (TryGet(response) is { } pending)
            return pending;

        pending = new PendingEvents(response);
        context.Items[typeof(PendingEvents)] = pending;

        response.OnStarting(static o =>
        {
            var state = (PendingEvents)o;
            state.Flush();
            return Task.CompletedTask;
        }, pending);

        return pending;
    }

    /// <summary>
    /// Serializes the specified events into a response header
    /// when the collection is not <see langword="null" />.
    /// </summary>
    /// <param name="name">The response header name.</param>
    /// <param name="events">The events to serialize.</param>
    private void SetHeader(string name, SmallDictionary<string, object>? events)
    {
        if (events is not null)
            _response.Headers[name] = JsonSerializer.Serialize(events, JsonOptions.CamelCase);
    }

    /// <summary>
    /// Normalizes unsupported HTMX 4.x trigger timings to the primary trigger header.
    /// </summary>
    /// <param name="timing">The requested event timing.</param>
    /// <returns>
    /// The timing supported by the configured HTMX version.
    /// </returns>
    private HtmxTriggerTiming NormalizeTiming(HtmxTriggerTiming timing) =>
        _version == HtmxTargetVersion.V4
            ? HtmxTriggerTiming.Receive
            : timing;

    /// <summary>
    /// Returns the configured HTMX target version, defaulting to HTMX 2.x
    /// when toolkit services are unavailable.
    /// </summary>
    /// <param name="response">The response whose request services are inspected.</param>
    /// <returns>
    /// The configured HTMX target version.
    /// </returns>
    private static HtmxTargetVersion GetTargetVersion(HttpResponse response)
    {
        var p = response.HttpContext.RequestServices;
        var options = p?.GetService(typeof(IOptions<HtmxToolkitOptions>)) as IOptions<HtmxToolkitOptions>;

        return options?.Value.TargetVersion ?? HtmxTargetVersion.V2;
    }
}
