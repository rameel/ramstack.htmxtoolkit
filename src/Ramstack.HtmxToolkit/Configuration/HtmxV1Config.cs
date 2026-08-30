using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Represents the configuration for HTMX 1.x.
/// </summary>
public sealed class HtmxV1Config() : HtmxConfig(HtmxTargetVersion.V1)
{
    /// <summary>
    /// Gets or sets a value indicating whether HTMX history support is enabled.
    /// The HTMX default is <see langword="true" />.
    /// </summary>
    public bool? HistoryEnabled
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the size of the history cache.
    /// The HTMX default is <c>10</c>.
    /// </summary>
    public int? HistoryCacheSize
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a full-page refresh should be issued
    /// on history misses rather than using an AJAX request.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? RefreshOnHistoryMiss
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the default swap style.
    /// The HTMX default is <see cref="HtmxSwap.InnerHtml" />.
    /// </summary>
    [JsonConverter(typeof(HtmxSwapJsonConverter))]
    public HtmxSwap? DefaultSwapStyle
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the default swap delay in milliseconds.
    /// The HTMX default is <c>0</c>.
    /// </summary>
    public int? DefaultSwapDelay
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the default settle delay in milliseconds.
    /// The HTMX default is <c>20</c>.
    /// </summary>
    public int? DefaultSettleDelay
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the indicator styles are loaded.
    /// The HTMX default is <see langword="true" />.
    /// </summary>
    public bool? IncludeIndicatorStyles
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the indicator class.
    /// The HTMX default is <c>htmx-indicator</c>.
    /// </summary>
    public string? IndicatorClass
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the request class.
    /// The HTMX default is <c>htmx-request</c>.
    /// </summary>
    public string? RequestClass
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the added class.
    /// The HTMX default is <c>htmx-added</c>.
    /// </summary>
    public string? AddedClass
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the swapping class.
    /// The HTMX default is <c>htmx-swapping</c>.
    /// </summary>
    public string? SwappingClass
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the settling class.
    /// The HTMX default is <c>htmx-settling</c>.
    /// </summary>
    public string? SettlingClass
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the use of <c>eval</c> is allowed.
    /// The HTMX default is <see langword="true" />.
    /// </summary>
    public bool? AllowEval
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether script tags should be processed in new content.
    /// The HTMX default is <see langword="true" />.
    /// </summary>
    public bool? AllowScriptTags
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the nonce added to inline scripts.
    /// The HTMX default is an empty string.
    /// </summary>
    public string? InlineScriptNonce
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the attributes to settle during the settling phase.
    /// The HTMX default is <c>["class", "style", "width", "height"]</c>.
    /// </summary>
    public string[]? AttributesToSettle
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTML template tags are used to parse content.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? UseTemplateFragments
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the WebSocket reconnection delay strategy.
    /// The HTMX default is <c>full-jitter</c>.
    /// </summary>
    public string? WsReconnectDelay
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the type of binary data received over WebSocket connections.
    /// The HTMX default is <see cref="HtmxBinaryType.Blob" />.
    /// </summary>
    [JsonConverter(typeof(HtmxBinaryTypeJsonConverter))]
    public HtmxBinaryType? WsBinaryType
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the selector for elements that HTMX must not process.
    /// The HTMX default is <c>[disable-htmx], [data-disable-htmx]</c>.
    /// </summary>
    public string? DisableSelector
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether credentials are included in cross-origin requests.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? WithCredentials
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the request timeout, in milliseconds.
    /// The HTMX default is <c>0</c>.
    /// </summary>
    public int? Timeout
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether requests are restricted to the current origin.
    /// The HTMX default is <see langword="true" />.
    /// </summary>
    public bool? SelfRequestsOnly
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the scrolling behavior for boosted links.
    /// The HTMX default is <see cref="HtmxScrollBehavior.Smooth" />.
    /// </summary>
    [JsonConverter(typeof(HtmxScrollBehaviorJsonConverter))]
    public HtmxScrollBehavior? ScrollBehavior
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? DefaultFocusScroll
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether GET requests use a cache-busting parameter.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? GetCacheBusterParam
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the View Transition API should be used for swaps.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? GlobalViewTransitions
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the HTTP methods that use URL parameters.
    /// The HTMX default is <c>["get"]</c>.
    /// </summary>
    [JsonConverter(typeof(HttpVerbArrayJsonConverter))]
    public HttpVerb[]? MethodsThatUseUrlParams
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether document titles found in new content are ignored.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? IgnoreTitle
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether boosted targets are scrolled into the viewport.
    /// The HTMX default is <see langword="true" />.
    /// </summary>
    public bool? ScrollIntoViewOnBoost
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether parsed trigger specifications use
    /// a never-clearing cache. The cache is disabled by default.
    /// </summary>
    [JsonPropertyName("triggerSpecsCache")]
    [JsonConverter(typeof(HtmxTriggerSpecsCacheJsonConverter))]
    public bool? TriggerSpecsCacheEnabled
    {
        get;
        set => SetField(ref field, value);
    }

    /// <inheritdoc />
    protected override string Serialize() =>
        JsonSerializer.Serialize(this, HtmxConfigJsonSerializerContext.Default.HtmxV1Config);
}
