using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents configuration options for HTMX 1.x.
/// </summary>
public sealed class HtmxV1Options() : HtmxOptions(HtmxTargetVersion.V1)
{
    /// <summary>
    /// Gets or sets a value indicating whether HTMX history support is enabled.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool? HistoryEnabled { get; set; }

    /// <summary>
    /// Gets or sets the size of the history cache.
    /// Defaults to <c>10</c>.
    /// </summary>
    public int? HistoryCacheSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a full page refresh should be issued
    /// on history misses rather than using an AJAX request.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? RefreshOnHistoryMiss { get; set; }

    /// <summary>
    /// Gets or sets the default swap style. Defaults to <see cref="HtmxSwap.InnerHtml"/>.
    /// </summary>
    [JsonConverter(typeof(HtmxSwapJsonConverter))]
    public HtmxSwap? DefaultSwapStyle { get; set; }

    /// <summary>
    /// Gets or sets the default swap delay in milliseconds.
    /// Defaults to <c>0</c>.
    /// </summary>
    public int? DefaultSwapDelay { get; set; }

    /// <summary>
    /// Gets or sets the default settle delay in milliseconds.
    /// Defaults to <c>20</c>.
    /// </summary>
    public int? DefaultSettleDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the indicator styles are loaded.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool? IncludeIndicatorStyles { get; set; }

    /// <summary>
    /// Gets or sets the indicator class.
    /// Defaults to <c>htmx-indicator</c>.
    /// </summary>
    public string? IndicatorClass { get; set; }

    /// <summary>
    /// Gets or sets the request class.
    /// Defaults to <c>htmx-request</c>.
    /// </summary>
    public string? RequestClass { get; set; }

    /// <summary>
    /// Gets or sets the added class.
    /// Defaults to <c>htmx-added</c>.
    /// </summary>
    public string? AddedClass { get; set; }

    /// <summary>
    /// Gets or sets the swapping class.
    /// Defaults to <c>htmx-swapping</c>.
    /// </summary>
    public string? SwappingClass { get; set; }

    /// <summary>
    /// Gets or sets the settling class.
    /// Defaults to <c>htmx-settling</c>.
    /// </summary>
    public string? SettlingClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether eval is allowed.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool? AllowEval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether script tags should be processed in new content.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool? AllowScriptTags { get; set; }

    /// <summary>
    /// Gets or sets the nonce added to inline scripts.
    /// Defaults to an empty string.
    /// </summary>
    public string? InlineScriptNonce { get; set; }

    /// <summary>
    /// Gets or sets the attributes to settle during the settling phase.
    /// Defaults to <c>["class", "style", "width", "height"]</c>.
    /// </summary>
    public string[]? AttributesToSettle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTML template tags should be used for parsing content.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? UseTemplateFragments { get; set; }

    /// <summary>
    /// Gets or sets the WebSocket reconnect delay. Defaults to <c>full-jitter</c>.
    /// </summary>
    public string? WsReconnectDelay { get; set; }

    /// <summary>
    /// Gets or sets the type of binary data received over WebSocket connections.
    /// Defaults to <see cref="HtmxBinaryType.Blob"/>.
    /// </summary>
    [JsonConverter(typeof(HtmxBinaryTypeJsonConverter))]
    public HtmxBinaryType? WsBinaryType { get; set; }

    /// <summary>
    /// Gets or sets the selector for elements that HTMX must not process.
    /// Defaults to <c>[disable-htmx], [data-disable-htmx]</c>.
    /// </summary>
    public string? DisableSelector { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether cross-site requests include credentials.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? WithCredentials { get; set; }

    /// <summary>
    /// Gets or sets the number of milliseconds a request can take before being terminated.
    /// Defaults to <c>0</c>.
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether requests are restricted to the current origin.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool? SelfRequestsOnly { get; set; }

    /// <summary>
    /// Gets or sets the scrolling behavior for boosted links.
    /// Defaults to <see cref="HtmxScrollBehavior.Smooth"/>.
    /// </summary>
    [JsonConverter(typeof(HtmxScrollBehaviorJsonConverter))]
    public HtmxScrollBehavior? ScrollBehavior { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? DefaultFocusScroll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a cache-busting parameter should be included in GET requests.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? GetCacheBusterParam { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the View Transition API should be used for swaps.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? GlobalViewTransitions { get; set; }

    /// <summary>
    /// Gets or sets the HTTP methods that use URL parameters.
    /// Defaults to <c>["get"]</c>.
    /// </summary>
    [JsonConverter(typeof(HttpVerbArrayJsonConverter))]
    public HttpVerb[]? MethodsThatUseUrlParams { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether document titles found in new content are ignored.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool? IgnoreTitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether boosted targets are scrolled into the viewport.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool? ScrollIntoViewOnBoost { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX uses a never-clearing cache for parsed trigger specifications.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    [JsonPropertyName("triggerSpecsCache")]
    [JsonConverter(typeof(HtmxTriggerSpecsCacheJsonConverter))]
    public bool? TriggerSpecsCacheEnabled { get; set; }
}
