using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Represents the configuration for HTMX 4.x.
/// </summary>
public sealed class HtmxV4Config() : HtmxConfig(HtmxTargetVersion.V4)
{
    /// <summary>
    /// Gets or sets a value indicating whether all HTMX events are logged to the console.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? LogAll
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the secondary attribute prefix recognized alongside <c>hx-*</c>.
    /// The HTMX default is <c>data-hx-</c>.
    /// </summary>
    public string? Prefix
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the character used instead of <c>:</c> in attribute names.
    /// The HTMX default is <c>undefined</c>.
    /// </summary>
    public string? MetaCharacter
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets how HTMX history restoration is handled.
    /// The HTMX default is <see cref="HtmxHistoryMode.Enabled" />.
    /// </summary>
    [JsonConverter(typeof(HtmxHistoryModeJsonConverter))]
    public HtmxHistoryMode? History
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the default swap style.
    /// The HTMX default is <see cref="HtmxSwap.InnerHtml" />.
    /// </summary>
    [JsonConverter(typeof(HtmxSwapJsonConverter))]
    public HtmxSwap? DefaultSwap
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the main swap is performed
    /// when the response contained only out-of-band elements.
    /// <c>&lt;hx-partial&gt;</c> content always prevents the main swap.
    /// The HTMX default is <see langword="false" /> and can be overridden
    /// using the <c>swapEmpty</c> modifier on <c>hx-swap</c>.
    /// </summary>
    [JsonPropertyName("allowEmptySwapAfterOOB")]
    public bool? AllowEmptySwapAfterOob
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the default settle delay in milliseconds.
    /// The HTMX default is <c>1</c>.
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
    [JsonPropertyName("includeIndicatorCSS")]
    public bool? IncludeIndicatorCss
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
    /// Gets or sets the nonce added to inline scripts.
    /// The HTMX default is <c>undefined</c>, which means that no nonce is added.
    /// </summary>
    public string? InlineScriptNonce
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a comma-separated list of extensions that HTMX is allowed to load.
    /// The HTMX default is an empty string.
    /// </summary>
    public string? Extensions
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX attributes are inherited implicitly.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? ImplicitInheritance
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the default request timeout in milliseconds.
    /// The HTMX default is <c>60000</c>.
    /// </summary>
    public int? DefaultTimeout
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the request mode passed to the Fetch API.
    /// The HTMX default is <c>same-origin</c>.
    /// </summary>
    [JsonConverter(typeof(HtmxFetchModeJsonConverter))]
    public HtmxFetchMode? Mode
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// The HTMX default is <see langword="false" /> and can be overridden
    /// using the <c>focus-scroll</c> swap modifier.
    /// </summary>
    public bool? DefaultFocusScroll
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/View_Transitions_API">View Transition API</see>
    /// should be used when swapping in new content.
    /// The HTMX default is <see langword="false" />.
    /// </summary>
    public bool? Transitions
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the attribute name prefixes to preserve during morphing.
    /// The HTMX default is <c>["data-htmx-powered"]</c>.
    /// </summary>
    public string[]? MorphIgnore
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the selector for elements to skip during morphing.
    /// The HTMX default is <c>[hx-morph-skip]</c>.
    /// </summary>
    public string? MorphSkip
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the selector for elements whose children should not be morphed.
    /// The HTMX default is <c>[hx-morph-skip-children]</c>.
    /// </summary>
    public string? MorphSkipChildren
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of siblings scanned while matching elements during morphing.
    /// The HTMX default is <c>10</c>.
    /// </summary>
    public int? MorphScanLimit
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the response status codes or patterns for which HTMX does not perform a swap.
    /// The HTMX default is <c>[204, 304]</c>.
    /// </summary>
    /// <remarks>
    /// Although HTMX declares this option as a number array, it converts each entry to a string
    /// at runtime and supports wildcard patterns such as <c>"4xx"</c> and <c>"44x"</c>.
    /// </remarks>
    public string[]? NoSwap
    {
        get;
        set => SetField(ref field, value);
    }

    /// <inheritdoc />
    protected override string Serialize() =>
        JsonSerializer.Serialize(this, HtmxConfigJsonSerializerContext.Default.HtmxV4Config);
}
