using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents the <see cref="ITagHelper" /> implementation that applies to &lt;meta&gt; element
/// to declaratively define htmx options.
/// </summary>
[HtmlTargetElement("meta", Attributes = "htmx-config", TagStructure = TagStructure.WithoutEndTag)]
[HtmlTargetElement("htmx-config", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class HtmxConfigTagHelper(IAntiforgery antiforgery) : TagHelper
{
    private readonly HtmxConfigData _config = new();

    /// <summary>
    /// Gets or sets a value indicating whether htmx logs all events to the console.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("log-all")]
    public bool? LogAll
    {
        get => _config.LogAll;
        set => _config.LogAll = value;
    }

    /// <summary>
    /// Gets or sets the secondary attribute prefix recognized alongside <c>hx-</c>.
    /// Defaults to <c>data-hx-</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("prefix")]
    public string? Prefix
    {
        get => _config.Prefix;
        set => _config.Prefix = value;
    }

    /// <summary>
    /// Gets or sets the character used instead of <c>:</c> in attribute names.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("meta-character")]
    public string? MetaCharacter
    {
        get => _config.MetaCharacter;
        set => _config.MetaCharacter = value;
    }

    /// <summary>
    /// Gets or sets how HTMX history restoration is handled.
    /// Defaults to <see cref="HtmxHistoryMode.Enabled"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   In HTMX 1.x and 2.x this maps to the <c>historyEnabled</c> boolean configuration option,
    ///   where <see cref="HtmxHistoryMode.Disabled"/> yields <see langword="false" />
    ///   and any other value yields <see langword="true" />.
    /// </para>
    /// <para>
    ///   In HTMX 4.x this maps to the <c>history</c> configuration option.
    /// </para>
    /// <para>
    ///   For compatibility, the generated JSON includes both <c>historyEnabled</c> and <c>history</c>.
    /// </para>
    /// </remarks>
    [HtmlAttributeName("history")]
    public HtmxHistoryMode? History
    {
        get => _config.History;
        set => _config.History = value;
    }

    /// <summary>
    /// Gets or sets the size of the history cache. Defaults to <c>10</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("history-cache-size")]
    public int? HistoryCacheSize
    {
        get => _config.HistoryCacheSize;
        set => _config.HistoryCacheSize = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether a full page refresh
    /// should be issued on history misses rather than using an AJAX request.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("refresh-on-history-miss")]
    public bool? RefreshOnHistoryMiss
    {
        get => _config.RefreshOnHistoryMiss;
        set => _config.RefreshOnHistoryMiss = value;
    }

    /// <summary>
    /// Gets or sets the default swap style.
    /// Defaults to <see cref="HtmxSwap.InnerHtml"/>.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x, 2.x, and 4.x. HTMX 4.x uses the compatible <c>defaultSwap</c> key.
    /// For compatibility, the generated JSON includes both <c>defaultSwapStyle</c> and <c>defaultSwap</c>.
    /// </remarks>
    [HtmlAttributeName("default-swap-style")]
    public HtmxSwap? DefaultSwapStyle
    {
        // NOTE: The getter exists primarily for debugging; performance is not a concern here.
        get => EnumHelper.ParseHtmxSwap(_config.DefaultSwapStyle);
        set => _config.DefaultSwapStyle = value.GetSwapValue();
    }

    /// <summary>
    /// Gets or sets a value indicating whether an empty response body should replace the main swap target.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("default-swap-empty")]
    public bool? DefaultSwapEmpty
    {
        get => _config.DefaultSwapEmpty;
        set => _config.DefaultSwapEmpty = value;
    }

    /// <summary>
    /// Gets or sets the default swap delay. Defaults to <c>0</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("default-swap-delay")]
    public int? DefaultSwapDelay
    {
        get => _config.DefaultSwapDelay;
        set => _config.DefaultSwapDelay = value;
    }

    /// <summary>
    /// Gets or sets the default settle delay. Defaults to <c>20</c> in HTMX 1.x and 2.x,
    /// and <c>1</c> in HTMX 4.x.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x, 2.x, and 4.x.</remarks>
    [HtmlAttributeName("default-settle-delay")]
    public int? DefaultSettleDelay
    {
        get => _config.DefaultSettleDelay;
        set => _config.DefaultSettleDelay = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the indicator styles are loaded.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x, 2.x, and 4.x. HTMX 4.x uses the compatible <c>includeIndicatorCSS</c> key.
    /// For compatibility, the generated JSON includes both <c>includeIndicatorStyles</c> and <c>includeIndicatorCSS</c>.
    /// </remarks>
    [HtmlAttributeName("include-indicator-styles")]
    public bool? IncludeIndicatorStyles
    {
        get => _config.IncludeIndicatorStyles;
        set => _config.IncludeIndicatorStyles = value;
    }

    /// <summary>
    /// Gets or sets the indicator class. Defaults to <c>htmx-indicator</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x, 2.x, and 4.x.</remarks>
    [HtmlAttributeName("indicator-class")]
    public string? IndicatorClass
    {
        get => _config.IndicatorClass;
        set => _config.IndicatorClass = value;
    }

    /// <summary>
    /// Gets or sets the request class. Defaults to <c>htmx-request</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x, 2.x, and 4.x.</remarks>
    [HtmlAttributeName("request-class")]
    public string? RequestClass
    {
        get => _config.RequestClass;
        set => _config.RequestClass = value;
    }

    /// <summary>
    /// Gets or sets the added class. Defaults to <c>htmx-added</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("added-class")]
    public string? AddedClass
    {
        get => _config.AddedClass;
        set => _config.AddedClass = value;
    }

    /// <summary>
    /// Gets or sets the swapping class. Defaults to <c>htmx-swapping</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("swapping-class")]
    public string? SwappingClass
    {
        get => _config.SwappingClass;
        set => _config.SwappingClass = value;
    }

    /// <summary>
    /// Gets or sets the settling class. Defaults to <c>htmx-settling</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("settling-class")]
    public string? SettlingClass
    {
        get => _config.SettlingClass;
        set => _config.SettlingClass = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether eval is allowed.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("allow-eval")]
    public bool? AllowEval
    {
        get => _config.AllowEval;
        set => _config.AllowEval = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether script tags should be processed in new content.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("allow-script-tags")]
    public bool? AllowScriptTags
    {
        get => _config.AllowScriptTags;
        set => _config.AllowScriptTags = value;
    }

    /// <summary>
    /// Gets or sets a value meaning that no nonce will be added to inline scripts.
    /// Defaults to <c>""</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x, 2.x, and 4.x.</remarks>
    [HtmlAttributeName("inline-script-nonce")]
    public string? InlineScriptNonce
    {
        get => _config.InlineScriptNonce;
        set => _config.InlineScriptNonce = value;
    }

    /// <summary>
    /// Gets or sets a comma-separated list of extensions that htmx is allowed to load.
    /// Defaults to an empty string.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("extensions")]
    public string? Extensions
    {
        get => _config.Extensions;
        set => _config.Extensions = value;
    }

    /// <summary>
    /// Gets or sets a value meaning that no nonce will be added to inline styles.
    /// Defaults to <c>""</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("inline-style-nonce")]
    public string? InlineStyleNonce
    {
        get => _config.InlineStyleNonce;
        set => _config.InlineStyleNonce = value;
    }

    /// <summary>
    /// Gets or sets the attributes to settle during the settling phase.
    /// Defaults to <c>["class", "style", "width", "height"]</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("attributes-to-settle")]
    public string[]? AttributesToSettle
    {
        get => _config.AttributesToSettle;
        set => _config.AttributesToSettle = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTML template tags should be used for parsing content.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported only in HTMX 1.x. Removed in HTMX 2.x and 4.x.</remarks>
    [HtmlAttributeName("use-template-fragments")]
    public bool? UseTemplateFragments
    {
        get => _config.UseTemplateFragments;
        set => _config.UseTemplateFragments = value;
    }

    /// <summary>
    /// Gets or sets the WebSocket reconnect delay. Defaults to <c>full-jitter</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("ws-reconnect-delay")]
    public string? WsReconnectDelay
    {
        get => _config.WsReconnectDelay;
        set => _config.WsReconnectDelay = value;
    }

    /// <summary>
    /// Gets or sets the <a href="https://developer.mozilla.org/docs/Web/API/WebSocket/binaryType">type of binary data</a>
    /// being received over the WebSocket connection. Defaults to <see cref="HtmxBinaryType.Blob"/>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("ws-binary-type")]
    public HtmxBinaryType? WsBinaryType
    {
        // NOTE: The getter exists primarily for debugging; performance is not a concern here.
        get => Enum.TryParse<HtmxBinaryType>(_config.WsBinaryType ?? "", true, out var v) ? v : null;
        set => _config.WsBinaryType = value?.GetWsBinaryTypeValue();
    }

    /// <summary>
    /// Gets or sets the "disable" selector.
    /// Defaults to <c>[disable-htmx], [data-disable-htmx]</c>.
    /// HTMX will not process elements with this attribute on it or a parent.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("disable-selector")]
    public string? DisableSelector
    {
        get => _config.DisableSelector;
        set => _config.DisableSelector = value;
    }

    /// <summary>
    /// Gets or sets the value that allows cross-site <c>Access-Control</c> requests
    /// using credentials such as cookies, authorization headers or TLS client certificates.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("with-credentials")]
    public bool? WithCredentials
    {
        get => _config.WithCredentials;
        set => _config.WithCredentials = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether htmx attribute inheritance is disabled.
    /// If set to <see langword="true" />, the inheritance of attributes is completely disabled
    /// and you can explicitly specify the inheritance with the <c>hx-inherit</c> attribute.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// <para>Supported in HTMX 2.x and 4.x; not supported in HTMX 1.x.</para>
    /// <para>In HTMX 2.x this maps to the <c>disableInheritance</c> configuration option.</para>
    /// <para>
    ///   In HTMX 4.x this maps to the inverse of the <c>implicitInheritance</c>
    ///   configuration option.
    /// </para>
    /// <para>
    ///   For compatibility, the generated JSON includes both <c>disableInheritance</c> and its inverse,
    ///   <c>implicitInheritance</c>.
    /// </para>
    /// </remarks>
    [HtmlAttributeName("disable-inheritance")]
    public bool? DisableInheritance
    {
        get => _config.DisableInheritance;
        set => _config.DisableInheritance = value;
    }

    /// <summary>
    /// Gets or sets the number of milliseconds a request can take before automatically being terminated.
    /// Defaults to <c>0</c> in HTMX 1.x and 2.x, and <c>60000</c> in HTMX 4.x.
    /// </summary>
    /// <remarks>
    /// <para>Supported in HTMX 1.x and 2.x as the <c>timeout</c> configuration option.</para>
    /// <para>In HTMX 4.x this maps to the <c>defaultTimeout</c> configuration option.</para>
    /// <para>For compatibility, the generated JSON includes both <c>timeout</c> and <c>defaultTimeout</c>.</para>
    /// </remarks>
    [HtmlAttributeName("timeout")]
    public int? DefaultTimeout
    {
        get => _config.DefaultTimeout;
        set => _config.DefaultTimeout = value;
    }

    /// <summary>
    /// Gets or sets the request mode passed to the Fetch API.
    /// Defaults to <c>same-origin</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   In HTMX 1.x and 2.x this maps to the <c>selfRequestsOnly</c> boolean configuration option,
    ///   where <see cref="HtmxFetchMode.SameOrigin"/> yields <see langword="true" />
    ///   and any other value yields <see langword="false" />.
    /// </para>
    /// <para>
    ///   In HTMX 4.x this maps to the <c>mode</c> configuration option.
    /// </para>
    /// <para>
    ///   For compatibility, the generated JSON includes both <c>mode</c> and the derived
    ///   <c>selfRequestsOnly</c> value.
    /// </para>
    /// </remarks>
    [HtmlAttributeName("mode")]
    public HtmxFetchMode? Mode
    {
        // NOTE: The getter exists primarily for debugging; performance is not a concern here.
        get => EnumHelper.ParseHtmxFetchMode(_config.Mode);
        set => _config.Mode = value?.GetFetchModeValue();
    }

    /// <summary>
    /// Gets or sets a value indicating the behavior for a boosted link on page transitions.
    /// Defaults to <see cref="HtmxScrollBehavior.Smooth"/> in HTMX 1.x
    /// and <see cref="HtmxScrollBehavior.Instant"/> in HTMX 2.x.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("scroll-behavior")]
    public HtmxScrollBehavior? ScrollBehavior
    {
        // NOTE: The getter exists primarily for debugging; performance is not a concern here.
        get => Enum.TryParse<HtmxScrollBehavior>(_config.ScrollBehavior ?? "", true, out var v) ? v : null;
        set => _config.ScrollBehavior = value?.GetScrollBehaviorValue();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// Defaults to <see langword="false" /> and can be overridden using the <c>focus-scroll</c> swap modifier.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x, 2.x, and 4.x.</remarks>
    [HtmlAttributeName("default-focus-scroll")]
    public bool? DefaultFocusScroll
    {
        get => _config.DefaultFocusScroll;
        set => _config.DefaultFocusScroll = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether a cache‑busting parameter
    /// should be included in GET requests to avoid caching partial responses by the browser.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.
    /// In HTMX 2.x, the format changed to <c>org.htmx.cache-buster=targetElementId</c>,
    /// where the target element is appended to the GET request.
    /// </remarks>
    [HtmlAttributeName("get-cache-buster-param")]
    public bool? GetCacheBusterParam
    {
        get => _config.GetCacheBusterParam;
        set => _config.GetCacheBusterParam = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/API/View_Transitions_API">View Transition API</a>
    /// should be used when swapping in new content.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x, 2.x, and 4.x. HTMX 4.x uses the compatible <c>transitions</c> key.
    /// For compatibility, the generated JSON includes both <c>globalViewTransitions</c> and <c>transitions</c>.
    /// </remarks>
    [HtmlAttributeName("global-view-transitions")]
    public bool? GlobalViewTransitions
    {
        get => _config.GlobalViewTransitions;
        set => _config.GlobalViewTransitions = value;
    }

    /// <summary>
    /// Gets or sets the attribute name prefixes to preserve during morphing.
    /// Defaults to <c>["data-htmx-powered"]</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("morph-ignore")]
    public string[]? MorphIgnore
    {
        get => _config.MorphIgnore;
        set => _config.MorphIgnore = value;
    }

    /// <summary>
    /// Gets or sets the selector for elements to skip during morphing.
    /// Defaults to <c>[hx-morph-skip]</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("morph-skip")]
    public string? MorphSkip
    {
        get => _config.MorphSkip;
        set => _config.MorphSkip = value;
    }

    /// <summary>
    /// Gets or sets the selector for elements whose children should not be morphed.
    /// Defaults to <c>[hx-morph-skip-children]</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("morph-skip-children")]
    public string? MorphSkipChildren
    {
        get => _config.MorphSkipChildren;
        set => _config.MorphSkipChildren = value;
    }

    /// <summary>
    /// Gets or sets the maximum number of siblings scanned while matching elements during morphing.
    /// Defaults to <c>10</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName("morph-scan-limit")]
    public int? MorphScanLimit
    {
        get => _config.MorphScanLimit;
        set => _config.MorphScanLimit = value;
    }

    /// <summary>
    /// Gets or sets the response status codes or patterns for which htmx does not perform a swap.
    /// Defaults to <c>[204, 304]</c>.
    /// </summary>
    /// <remarks>
    /// <para>Supported only in HTMX 4.x.</para>
    /// <para>
    ///   Although the HTMX TypeScript declaration types this as <c>number[]</c>, at runtime htmx
    ///   converts each entry to a string and matches it against the status code as well as
    ///   wildcard patterns such as <c>"4xx"</c> or <c>"44x"</c>, so both forms are accepted.
    /// </para>
    /// </remarks>
    [HtmlAttributeName("no-swap")]
    public string[]? NoSwap
    {
        get => _config.NoSwap;
        set => _config.NoSwap = value;
    }

    /// <summary>
    /// Gets or sets a list of HTTP methods that use URL parameters.
    /// Defaults to <c>["get"]</c> in HTMX 1.x and <c>["get", "delete"]</c> in HTMX 2.x.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("methods-that-use-url-params")]
    public HttpVerb[]? MethodsThatUseUrlParams
    {
        get => _config.MethodsThatUseUrlParams.GetValueOrDefault().Values;
        set => _config.MethodsThatUseUrlParams = new HttpVerbArray(value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should not update the title of the document
    /// when a title tag is found in new content. Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("ignore-title")]
    public bool? IgnoreTitle
    {
        get => _config.IgnoreTitle;
        set => _config.IgnoreTitle = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the target of a boosted element
    /// is scrolled into the viewport. If <c>hx-target</c> is omitted on a boosted element,
    /// the target defaults to body, causing the page to scroll to the top.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("scroll-into-view-on-boost")]
    public bool? ScrollIntoViewOnBoost
    {
        get => _config.ScrollIntoViewOnBoost;
        set => _config.ScrollIntoViewOnBoost = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should use a never-clearing cache
    /// for evaluated trigger specifications, improving parsing performance
    /// at the cost of more memory usage. Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// <para>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</para>
    /// <para>
    ///   When <see langword="true" />, serialized as the <c>triggerSpecsCache</c> configuration option
    ///   set to an empty object (<c>{}</c>). When <see langword="false" />, serialized as <see langword="null" />.
    /// </para>
    /// </remarks>
    [HtmlAttributeName("trigger-specs-cache-enabled")]
    public bool? TriggerSpecsCacheEnabled
    {
        get => _config.TriggerSpecsCache;
        set => _config.TriggerSpecsCache = value;
    }

    /// <summary>
    /// Gets or sets the default response handling behavior for HTTP response status codes.
    /// Accepts an array of <see cref="ResponseHandlingConfig"/> objects that define
    /// how htmx should handle responses matching specific status code patterns.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x. Replaced by <c>hx-status</c> and <c>noSwap</c> in HTMX 4.x.</remarks>
    [HtmlAttributeName("response-handling")]
    public IList<ResponseHandlingConfig>? ResponseHandling
    {
        get => _config.ResponseHandling;
        set => _config.ResponseHandling = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to process OOB swaps
    /// on elements that are nested within the main response element.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("allow-nested-oob-swaps")]
    public bool? AllowNestedOobSwaps
    {
        get => _config.AllowNestedOobSwaps;
        set => _config.AllowNestedOobSwaps = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to treat history cache miss
    /// full page reload requests as an "HX-Request" by returning the corresponding response header.
    /// Defaults to <see langword="true" />.
    /// This should always be disabled when using the <c>HX-Request</c> header
    /// to optionally return partial responses.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("history-restore-as-hx-request")]
    public bool? HistoryRestoreAsHxRequest
    {
        get => _config.HistoryRestoreAsHxRequest;
        set => _config.HistoryRestoreAsHxRequest = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to report input validation errors
    /// to the end user and update focus to the first input that fails validation.
    /// Defaults to <see langword="false" />.
    /// This should always be enabled as this matches default browser form submit behavior.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x. Removed in HTMX 4.x.</remarks>
    [HtmlAttributeName("report-validity-of-forms")]
    public bool? ReportValidityOfForms
    {
        get => _config.ReportValidityOfForms;
        set => _config.ReportValidityOfForms = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether an antiforgery token should be included.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>This is a custom extension, not part of the htmx configuration.</remarks>
    [HtmlAttributeName("include-antiforgery-token")]
    public bool IncludeAntiForgeryToken { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext"/>.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (output.TagName == "meta")
            output.Attributes.RemoveAll("htmx-config");

        output.TagName = "meta";
        output.TagMode = TagMode.SelfClosing;
        output.Attributes.SetAttribute("name", "htmx-config");

        context.Items[typeof(HtmxConfigTagHelper)] = this;
        await output.GetChildContentAsync();

        if (IncludeAntiForgeryToken)
            _config.AntiForgery = antiforgery.GetAndStoreTokens(ViewContext.HttpContext);

        var info = HtmxConfigJsonSerializerContext.Default.HtmxConfigData;
        var config = new HtmlString(JsonSerializer.Serialize(_config, info));

        output.Attributes.SetAttribute(
            new TagHelperAttribute("content", config, HtmlAttributeValueStyle.SingleQuotes));
    }

    #region Inner type: HtmxConfigData

    /// <summary>
    /// Represents the serializable configuration data for the <see cref="HtmxConfigTagHelper"/> class.
    /// </summary>
    internal sealed class HtmxConfigData
    {
        public bool? LogAll { get; set; }
        public string? Prefix { get; set; }
        public string? MetaCharacter { get; set; }

        /// <summary>
        /// Gets the legacy <c>historyEnabled</c> value derived from <see cref="History"/>.
        /// Updated automatically whenever <see cref="History"/> is assigned.
        /// </summary>
        public bool? HistoryEnabled { get; private set; }

        [JsonConverter(typeof(HtmxHistoryModeJsonConverter))]
        public HtmxHistoryMode? History
        {
            get;
            set
            {
                field = value;
                HistoryEnabled = value switch
                {
                    null => null,
                    HtmxHistoryMode.Disabled => false,
                    _ => true
                };
            }
        }

        public int? HistoryCacheSize { get; set; }
        public bool? RefreshOnHistoryMiss { get; set; }
        public string? DefaultSwapStyle { get; set; }
        public string? DefaultSwap => DefaultSwapStyle;
        public bool? DefaultSwapEmpty { get; set; }
        public int? DefaultSwapDelay { get; set; }
        public int? DefaultSettleDelay { get; set; }
        public bool? IncludeIndicatorStyles { get; set; }
        [JsonPropertyName("includeIndicatorCSS")]
        public bool? IncludeIndicatorCss => IncludeIndicatorStyles;
        public string? IndicatorClass { get; set; }
        public string? RequestClass { get; set; }
        public string? AddedClass { get; set; }
        public string? SwappingClass { get; set; }
        public string? SettlingClass { get; set; }
        public bool? AllowEval { get; set; }
        public bool? AllowScriptTags { get; set; }
        public string? InlineScriptNonce { get; set; }
        public string? Extensions { get; set; }
        public string? InlineStyleNonce { get; set; }
        public string[]? AttributesToSettle { get; set; }
        public bool? UseTemplateFragments { get; set; }
        public string? WsReconnectDelay { get; set; }
        public string? WsBinaryType { get; set; }
        public string? DisableSelector { get; set; }
        public bool? WithCredentials { get; set; }
        public bool? DisableInheritance { get; set; }
        public bool? ImplicitInheritance => DisableInheritance is bool value ? !value : null;
        public int? DefaultTimeout { get; set; }
        public int? Timeout => DefaultTimeout;
        public string? Mode { get; set; }
        public string? ScrollBehavior { get; set; }
        public bool? DefaultFocusScroll { get; set; }
        public bool? GetCacheBusterParam { get; set; }
        public bool? GlobalViewTransitions { get; set; }
        public bool? Transitions => GlobalViewTransitions;
        public string[]? MorphIgnore { get; set; }
        public string? MorphSkip { get; set; }
        public string? MorphSkipChildren { get; set; }
        public int? MorphScanLimit { get; set; }
        public string[]? NoSwap { get; set; }
        public HttpVerbArray? MethodsThatUseUrlParams { get; set; }
        public bool? SelfRequestsOnly => Mode is not null ? Mode == "same-origin" : null;
        public bool? IgnoreTitle { get; set; }
        public bool? ScrollIntoViewOnBoost { get; set; }
        [JsonConverter(typeof(HtmxTriggerSpecsCacheJsonConverter))]
        public bool? TriggerSpecsCache { get; set; }
        public IList<ResponseHandlingConfig>? ResponseHandling { get; set; }
        public bool? AllowNestedOobSwaps { get; set; }
        public bool? HistoryRestoreAsHxRequest { get; set; }
        public bool? ReportValidityOfForms { get; set; }
        public AntiforgeryTokenSet? AntiForgery { get; set; }
    }

    #endregion
}
