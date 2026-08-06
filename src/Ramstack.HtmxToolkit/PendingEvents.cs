using System.Text.Json;

using Microsoft.AspNetCore.Http;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Accumulates htmx events per <see cref="HtmxTriggerTiming"/> for a single request,
/// deferring header serialization until the response is about to start.
/// </summary>
internal sealed class PendingEvents(HttpResponse response)
{
    private Dictionary<string, object>? _receive;
    private Dictionary<string, object>? _afterSettle;
    private Dictionary<string, object>? _afterSwap;

    /// <summary>
    /// Adds the specified events to the pending set for the given <paramref name="timing"/>.
    /// Keys already present are preserved.
    /// </summary>
    /// <param name="timing">The time at which the events will be triggered.</param>
    /// <param name="events">A dictionary containing event names as keys and event details as values.</param>
    public void AddEvents(HtmxTriggerTiming timing, IReadOnlyDictionary<string, object> events)
    {
        var current = timing switch
        {
            HtmxTriggerTiming.Receive => _receive ??= new Dictionary<string, object>(),
            HtmxTriggerTiming.AfterSettle => _afterSettle ??= new Dictionary<string, object>(),
            _ => _afterSwap ??= new Dictionary<string, object>()
        };

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
            HtmxTriggerTiming.AfterSettle => _afterSettle,
            _ => _afterSwap
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
            case HtmxTriggerTiming.AfterSettle:
                _afterSettle = replacement;
                break;
            default:
                _afterSwap = replacement;
                break;
        }
    }

    /// <summary>
    /// Serializes the accumulated events, if any, into the corresponding <c>HX-Trigger</c> response headers.
    /// </summary>
    public void Flush()
    {
        SetHeader(HtmxResponseHeaderNames.Trigger, _receive);
        SetHeader(HtmxResponseHeaderNames.TriggerAfterSettle, _afterSettle);
        SetHeader(HtmxResponseHeaderNames.TriggerAfterSwap, _afterSwap);
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
            response.Headers[name] = JsonSerializer.Serialize(events, JsonOptions.CamelCase);
    }
}
