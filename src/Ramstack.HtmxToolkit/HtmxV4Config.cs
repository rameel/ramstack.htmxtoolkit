using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents the configuration for HTMX 4.x.
/// </summary>
public sealed class HtmxV4Config() : HtmxConfig(HtmxTargetVersion.V4)
{
    /// <summary>
    /// Gets or sets a value indicating whether all HTMX events are logged to the console.
    /// Defaults to <see langword="false" />.
    /// </summary>
    public bool? LogAll { get; set; }

    /// <summary>
    /// Gets or sets the secondary attribute prefix recognized alongside <c>hx-</c>.
    /// Defaults to <c>data-hx-</c>.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets the character used instead of <c>:</c> in attribute names.
    /// </summary>
    public string? MetaCharacter { get; set; }

    /// <summary>
    /// Gets or sets how HTMX history restoration is handled.
    /// Defaults to <see cref="HtmxHistoryMode.Enabled"/>.
    /// </summary>
    [JsonConverter(typeof(HtmxHistoryModeJsonConverter))]
    public HtmxHistoryMode? History { get; set; }

    /// <summary>
    /// Gets or sets the default swap style.
    /// Defaults to <see cref="HtmxSwap.InnerHtml"/>.
    /// </summary>
    [JsonConverter(typeof(HtmxSwapJsonConverter))]
    public HtmxSwap? DefaultSwap { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an empty response body should replace the main swap target.
    /// </summary>
    public bool? DefaultSwapEmpty { get; set; }

    /// <summary>
    /// Gets or sets the default settle delay in milliseconds.
    /// Defaults to <c>1</c>.
    /// </summary>
    public int? DefaultSettleDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the indicator styles are loaded.
    /// Defaults to <see langword="true" />.
    /// </summary>
    [JsonPropertyName("includeIndicatorCSS")]
    public bool? IncludeIndicatorCss { get; set; }

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
    /// Gets or sets a value meaning that no nonce will be added to inline scripts.
    /// Defaults to <c>""</c>.
    /// </summary>
    public string? InlineScriptNonce { get; set; }

    /// <summary>
    /// Gets or sets a comma-separated list of extensions that HTMX is allowed to load.
    /// Defaults to an empty string.
    /// </summary>
    public string? Extensions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX attributes are inherited implicitly.
    /// Defaults to <see langword="true" />.
    /// </summary>
    public bool? ImplicitInheritance { get; set; }

    /// <summary>
    /// Gets or sets the default request timeout in milliseconds.
    /// Defaults to <c>60000</c>.
    /// </summary>
    public int? DefaultTimeout { get; set; }

    /// <summary>
    /// Gets or sets the request mode passed to the Fetch API.
    /// Defaults to <c>same-origin</c>.
    /// </summary>
    [JsonConverter(typeof(HtmxFetchModeJsonConverter))]
    public HtmxFetchMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// Defaults to <see langword="false" /> and can be overridden using the <c>focus-scroll</c> swap modifier.
    /// </summary>
    public bool? DefaultFocusScroll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/API/View_Transitions_API">View Transition API</a>
    /// should be used when swapping in new content. Defaults to <see langword="false" />.
    /// </summary>
    public bool? Transitions { get; set; }

    /// <summary>
    /// Gets or sets the attribute name prefixes to preserve during morphing.
    /// Defaults to <c>["data-htmx-powered"]</c>.
    /// </summary>
    public string[]? MorphIgnore { get; set; }

    /// <summary>
    /// Gets or sets the selector for elements to skip during morphing.
    /// Defaults to <c>[hx-morph-skip]</c>.
    /// </summary>
    public string? MorphSkip { get; set; }

    /// <summary>
    /// Gets or sets the selector for elements whose children should not be morphed.
    /// Defaults to <c>[hx-morph-skip-children]</c>.
    /// </summary>
    public string? MorphSkipChildren { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of siblings scanned while matching elements during morphing.
    /// Defaults to <c>10</c>.
    /// </summary>
    public int? MorphScanLimit { get; set; }

    /// <summary>
    /// Gets or sets the response status codes or patterns for which HTMX does not perform a swap.
    /// Defaults to <c>[204, 304]</c>.
    /// </summary>
    /// <remarks>
    /// Although HTMX declares this option as a number array, it converts each entry to a string
    /// at runtime and supports wildcard patterns such as <c>"4xx"</c> and <c>"44x"</c>.
    /// </remarks>
    public string[]? NoSwap { get; set; }

    /// <inheritdoc />
    internal override string ToJson() =>
        JsonSerializer.Serialize(this, HtmxConfigJsonSerializerContext.Default.HtmxV4Config);
}
