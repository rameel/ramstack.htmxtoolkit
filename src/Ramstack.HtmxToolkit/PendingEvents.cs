using System.Runtime.InteropServices;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Accumulates htmx events per <see cref="HtmxTriggerTiming"/> for a single request,
/// deferring header serialization until the response is about to start.
/// </summary>
internal sealed class PendingEvents
{
    private const string ProxyEventName = "rs:events";

    private readonly HttpResponse _response;
    private Dictionary<string, object>? _receive;
    private Dictionary<string, object>? _afterSwap;
    private Dictionary<string, object>? _afterSettle;

    /// <summary>
    /// Initializes a new instance of the <see cref="PendingEvents"/> class.
    /// </summary>
    /// <param name="response">The HTTP response to which the events belong.</param>
    private PendingEvents(HttpResponse response) =>
        _response = response;

    /// <summary>
    /// Adds the specified events to the pending set for the given <paramref name="timing"/>.
    /// When a key already exists, the duplicate event is accumulated under a <c>rs:events</c> key and replayed client-side.
    /// </summary>
    /// <param name="timing">The time at which the events will be triggered.</param>
    /// <param name="events">A dictionary containing event names as keys and event details as values.</param>
    public void AddEvents(HtmxTriggerTiming timing, IReadOnlyDictionary<string, object> events)
    {
        var current = timing switch
        {
            HtmxTriggerTiming.Receive => _receive ??= new Dictionary<string, object>(),
            HtmxTriggerTiming.AfterSwap => _afterSwap ??= new Dictionary<string, object>(),
            _ => _afterSettle ??= new Dictionary<string, object>(),
        };

        foreach (var (k, v) in events)
        {
            if (current.TryAdd(k, v))
                continue;

            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(current, ProxyEventName, out _);
            if (value is not List<KeyValuePair<string, object>> collection)
                value = collection = [];

            collection.Add(new KeyValuePair<string, object>(k, v));
        }
    }

    /// <summary>
    /// Returns the pending events for the specified <paramref name="timing"/>.
    /// </summary>
    /// <param name="timing">The time at which the events will be triggered.</param>
    /// <returns>
    /// The pending events, or <see langword="null"/> if none were registered.
    /// </returns>
    public IReadOnlyDictionary<string, object>? GetEvents(HtmxTriggerTiming timing)
    {
        return timing switch
        {
            HtmxTriggerTiming.Receive => _receive,
            HtmxTriggerTiming.AfterSwap => _afterSwap,
            _ => _afterSettle,
        };
    }

    /// <summary>
    /// Replaces the pending events for the specified <paramref name="timing"/>.
    /// </summary>
    /// <param name="timing">The time at which the events will be triggered.</param>
    /// <param name="events">A dictionary containing event names as keys and event details as values.</param>
    public void SetEvents(HtmxTriggerTiming timing, IReadOnlyDictionary<string, object> events)
    {
        var replacement = new Dictionary<string, object>(events);
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
    /// Serializes the accumulated events, if any, into the corresponding <c>HX-Trigger</c> response headers.
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
    /// The pending events accumulator, or <see langword="null"/> if none was registered.
    /// </returns>
    public static PendingEvents? TryGet(HttpResponse response) =>
        response.HttpContext.Items[typeof(PendingEvents)] as PendingEvents;

    /// <summary>
    /// Returns the pending events accumulator, creating and registering it for the response when necessary.
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

    private void SetHeader(string name, Dictionary<string, object>? events)
    {
        if (events is not null)
            _response.Headers[name] = JsonSerializer.Serialize(events, JsonOptions.CamelCase);
    }
}
