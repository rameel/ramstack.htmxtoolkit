using System.Buffers;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Accumulates HTMX events by <see cref="HtmxTriggerTiming" /> for a single request,
/// deferring header serialization until the response is about to start.
/// </summary>
internal sealed class PendingEvents
{
    /// <summary>
    /// The name of the event that carries all events of a header when an event name repeats.
    /// </summary>
    private const string ProxyEventName = "rs:event";

    /// <summary>
    /// The maximum number of events for which repeated names are found using a linear scan.
    /// </summary>
    private const int LinearScanThreshold = 8;

    private readonly HttpResponse _response;
    private readonly HtmxTargetVersion _version;

    private EventList? _receive;
    private EventList? _afterSwap;
    private EventList? _afterSettle;

    /// <summary>
    /// Initializes a new instance of the <see cref="PendingEvents" /> class.
    /// </summary>
    /// <param name="response">The HTTP response to which the events belong.</param>
    private PendingEvents(HttpResponse response) =>
        (_response, _version) = (response, GetTargetVersion(response));

    /// <summary>
    /// Adds the specified event to the end of the pending events for <paramref name="timing" />.
    /// </summary>
    /// <param name="timing">The time at which to trigger the events.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="detailJson">The event detail as JSON.</param>
    public void AddEvent(HtmxTriggerTiming timing, string eventName, string detailJson)
    {
        Argument.ThrowIfNullOrWhiteSpace(eventName);
        Argument.ThrowIfNullOrWhiteSpace(detailJson);

        if (eventName == ProxyEventName)
            throw new ArgumentException(
                $"The event name '{ProxyEventName}' is reserved.",
                nameof(eventName));

        timing = NormalizeTiming(timing);

        var events = timing switch
        {
            HtmxTriggerTiming.Receive => _receive ??= new EventList(),
            HtmxTriggerTiming.AfterSwap => _afterSwap ??= new EventList(),
            _ => _afterSettle ??= new EventList()
        };

        events.Add(eventName, detailJson);
    }

    /// <summary>
    /// Returns the pending events for the specified <paramref name="timing" />
    /// in the order they were added.
    /// </summary>
    /// <param name="timing">The time at which to trigger the events.</param>
    /// <returns>
    /// The pending events, or <see langword="null" /> if none were registered.
    /// </returns>
    /// <remarks>
    /// Returns a read-only live view of the internal accumulator.
    /// </remarks>
    public IReadOnlyList<KeyValuePair<string, string>>? GetEvents(HtmxTriggerTiming timing)
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
    /// Serializes the accumulated events, if any, into the corresponding
    /// <c>HX-Trigger</c> response headers.
    /// </summary>
    public void Flush()
    {
        var buffer = new ArrayBufferWriter<byte>();

        SetHeader(HtmxResponseHeaderNames.Trigger, _receive, buffer);
        SetHeader(HtmxResponseHeaderNames.TriggerAfterSwap, _afterSwap, buffer);
        SetHeader(HtmxResponseHeaderNames.TriggerAfterSettle, _afterSettle, buffer);
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
    /// <remarks>
    /// A JSON object cannot hold repeated names. If an event name repeats, all events
    /// are written in order under the <see cref="ProxyEventName" /> key, and the Toolkit script replays them.
    /// Writing only the repeats there would change the order in which the events are triggered.
    /// </remarks>
    /// <param name="name">The response header name.</param>
    /// <param name="events">The events to serialize.</param>
    /// <param name="buffer">The buffer that receives the serialized header value.</param>
    private void SetHeader(string name, EventList? events, ArrayBufferWriter<byte> buffer)
    {
        if (events is null)
            return;

        buffer.Clear();

        using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Encoder = JsonOptions.Encoder, SkipValidation = true }))
        {
            writer.WriteStartObject();

            if (HasRepeatedNames(events))
            {
                writer.WriteStartArray(ProxyEventName);

                foreach (var (key, value) in events.Storage)
                {
                    writer.WriteStartObject();
                    {
                        writer.WriteString("key", key);
                        writer.WritePropertyName("value");
                        WriteDetail(writer, value);
                    }
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }
            else
            {
                foreach (var (key, value) in events.Storage)
                {
                    writer.WritePropertyName(key);
                    WriteDetail(writer, value);
                }
            }

            writer.WriteEndObject();
        }

        _response.Headers[name] = Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    /// <summary>
    /// Determines whether any event name occurs more than once.
    /// </summary>
    /// <param name="events">The events to inspect.</param>
    /// <returns>
    /// <see langword="true" /> if an event name repeats; otherwise, <see langword="false" />.
    /// </returns>
    private static bool HasRepeatedNames(EventList events)
    {
        // A response usually carries only a few events, and for them a quadratic scan
        // is cheaper than allocating a set. Events can be added in a loop, though,
        // so a set protects against a large number of them.
        var storage = events.Storage;

        if (storage.Count <= LinearScanThreshold)
        {
            for (var i = 1; i < storage.Count; i++)
                for (var j = 0; j < i; j++)
                    if (storage[i].Key == storage[j].Key)
                        return true;

            return false;
        }

        var names = new HashSet<string>(storage.Count, StringComparer.Ordinal);
        foreach (var (key, _) in storage)
            if (!names.Add(key))
                return true;

        return false;
    }

    /// <summary>
    /// Writes the serialized event detail.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="detailJson">The event detail as JSON.</param>
    private static void WriteDetail(Utf8JsonWriter writer, string detailJson)
    {
        // HTMX 1.x and 2.x pass a null detail to listeners as { value: null },
        // while HTMX 4.x fails on it. Write the wrapped form for all versions.
        if (detailJson == "null")
        {
            writer.WriteStartObject();
            writer.WriteNull("value");
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteRawValue(detailJson);
        }
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
    /// Returns the configured HTMX target version, defaulting to HTMX 2.x when toolkit services are unavailable.
    /// </summary>
    /// <param name="response">The response whose request services are inspected.</param>
    /// <returns>
    /// The configured HTMX target version.
    /// </returns>
    private static HtmxTargetVersion GetTargetVersion(HttpResponse response)
    {
        var p = response.HttpContext.RequestServices;
        var options = p.GetService(typeof(IOptions<HtmxToolkitOptions>)) as IOptions<HtmxToolkitOptions>;

        return options?.Value.TargetVersion ?? HtmxTargetVersion.V2;
    }

    #region Inner type: EventList

    /// <summary>
    /// Represents a read-only view of the events for a single timing, in the order they were added.
    /// </summary>
    private sealed class EventList : ReadOnlyCollection<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Gets the list that stores the events.
        /// </summary>
        /// <remarks>
        /// Used internally instead of the inherited members,
        /// which access the list through interface calls.
        /// </remarks>
        public List<KeyValuePair<string, string>> Storage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventList" /> class.
        /// </summary>
        public EventList() : this([])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventList" /> class that wraps the specified list.
        /// </summary>
        /// <param name="storage">The list that stores the events.</param>
        private EventList(List<KeyValuePair<string, string>> storage) : base(storage) =>
            Storage = storage;

        /// <summary>
        /// Adds an event to the end of the list.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="detailJson">The event detail as JSON.</param>
        public void Add(string eventName, string detailJson) =>
            Storage.Add(new KeyValuePair<string, string>(eventName, detailJson));
    }

    #endregion
}
