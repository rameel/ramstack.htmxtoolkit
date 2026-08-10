using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

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
    private readonly IAntiforgery _antiforgery = antiforgery;

    /// <summary>
    /// Gets or sets a value indicating whether history is enabled.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x and 2.x. This is mainly useful for testing.
    /// </remarks>
    [HtmlAttributeName("history-enabled")]
    public bool? HistoryEnabled { get; set; }

    /// <summary>
    /// Gets or sets the size of the history cache. Defaults to <c>10</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("history-cache-size")]
    public int? HistoryCacheSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a full page refresh
    /// should be issued on history misses rather than using an AJAX request.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("refresh-on-history-miss")]
    public bool? RefreshOnHistoryMiss { get; set; }

    /// <summary>
    /// Gets or sets the default swap style.
    /// Defaults to <see cref="HtmxSwap.InnerHtml"/>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("default-swap-style")]
    public HtmxSwap? DefaultSwapStyle { get; set; }

    /// <summary>
    /// Gets or sets the default swap delay. Defaults to <c>0</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("default-swap-delay")]
    public int? DefaultSwapDelay { get; set; }

    /// <summary>
    /// Gets or sets the default settle delay. Defaults to <c>20</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("default-settle-delay")]
    public int? DefaultSettleDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the indicator styles are loaded.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("include-indicator-styles")]
    public bool? IncludeIndicatorStyles { get; set; }

    /// <summary>
    /// Gets or sets the indicator class. Defaults to <c>htmx-indicator</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("indicator-class")]
    public string? IndicatorClass { get; set; }

    /// <summary>
    /// Gets or sets the request class. Defaults to <c>htmx-request</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("request-class")]
    public string? RequestClass { get; set; }

    /// <summary>
    /// Gets or sets the added class. Defaults to <c>htmx-added</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("added-class")]
    public string? AddedClass { get; set; }

    /// <summary>
    /// Gets or sets the swapping class. Defaults to <c>htmx-swapping</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("swapping-class")]
    public string? SwappingClass { get; set; }

    /// <summary>
    /// Gets or sets the settling class. Defaults to <c>htmx-settling</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("settling-class")]
    public string? SettlingClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether eval is allowed.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("allow-eval")]
    public bool? AllowEval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether script tags should be processed in new content.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("allow-script-tags")]
    public bool? AllowScriptTags { get; set; }

    /// <summary>
    /// Gets or sets a value meaning that no nonce will be added to inline scripts.
    /// Defaults to <c>""</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("inline-script-nonce")]
    public string? InlineScriptNonce { get; set; }

    /// <summary>
    /// Gets or sets a value meaning that no nonce will be added to inline styles.
    /// Defaults to <c>""</c>.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x.</remarks>
    [HtmlAttributeName("inline-style-nonce")]
    public string? InlineStyleNonce { get; set; }

    /// <summary>
    /// Gets or sets the attributes to settle during the settling phase.
    /// Defaults to <c>["class", "style", "width", "height"]</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("attributes-to-settle")]
    public string[]? AttributesToSettle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTML template tags should be used for parsing content.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported only in HTMX 1.x. Removed in HTMX 2.x.</remarks>
    [HtmlAttributeName("use-template-fragments")]
    public bool? UseTemplateFragments { get; set; }

    /// <summary>
    /// Gets or sets the WebSocket reconnect delay. Defaults to <c>full-jitter</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("ws-reconnect-delay")]
    public string? WsReconnectDelay { get; set; }

    /// <summary>
    /// Gets or sets the <a href="https://developer.mozilla.org/docs/Web/API/WebSocket/binaryType">type of binary data</a>
    /// being received over the WebSocket connection. Defaults to <see cref="HtmxBinaryType.Blob"/>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("ws-binary-type")]
    public HtmxBinaryType? WsBinaryType { get; set; }

    /// <summary>
    /// Gets or sets the "disable" selector.
    /// Defaults to <c>[disable-htmx], [data-disable-htmx]</c>.
    /// HTMX will not process elements with this attribute on it or a parent.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("disable-selector")]
    public string? DisableSelector { get; set; }

    /// <summary>
    /// Gets or sets the value that allows cross-site <c>Access-Control</c> requests
    /// using credentials such as cookies, authorization headers or TLS client certificates.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("with-credentials")]
    public bool? WithCredentials { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether htmx attribute inheritance is disabled.
    /// If set to <see langword="true" />, the inheritance of attributes is completely disabled
    /// and you can explicitly specify the inheritance with the <c>hx-inherit</c> attribute.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x.</remarks>
    [HtmlAttributeName("disable-inheritance")]
    public bool? DisableInheritance { get; set; }

    /// <summary>
    /// Gets or sets the number of milliseconds a request can take before automatically being terminated.
    /// Defaults to <c>0</c>.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("timeout")]
    public int? Timeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the behavior for a boosted link on page transitions.
    /// Defaults to <see cref="HtmxScrollBehavior.Smooth"/> in HTMX 1.x
    /// and <see cref="HtmxScrollBehavior.Instant"/> in HTMX 2.x.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("scroll-behavior")]
    public HtmxScrollBehavior? ScrollBehavior { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// Defaults to <see langword="false" /> and can be overridden using the <c>focus-scroll</c> swap modifier.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("default-focus-scroll")]
    public bool? DefaultFocusScroll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a cache‑busting parameter
    /// should be included in GET requests to avoid caching partial responses by the browser.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x and 2.x.
    /// In HTMX 2.x, the format changed to <c>org.htmx.cache-buster=targetElementId</c>,
    /// where the target element is appended to the GET request.
    /// </remarks>
    [HtmlAttributeName("get-cache-buster-param")]
    public bool? GetCacheBusterParam { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/API/View_Transitions_API">View Transition API</a>
    /// should be used when swapping in new content.
    /// Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("global-view-transitions")]
    public bool? GlobalViewTransitions { get; set; }

    /// <summary>
    /// Gets or sets a list of HTTP methods that use URL parameters.
    /// Defaults to <c>["get"]</c> in HTMX 1.x and <c>["get", "delete"]</c> in HTMX 2.x.
    /// </summary>
    /// <remarks>
    /// Supported in HTMX 1.x and 2.x.
    /// <para>
    /// Allowed values: <c>"get"</c>, <c>"head"</c>, <c>"post"</c>, <c>"put"</c>,
    /// <c>"delete"</c>, <c>"connect"</c>, <c>"options"</c>, <c>"trace"</c>, <c>"patch"</c>.
    /// </para>
    /// </remarks>
    [HtmlAttributeName("methods-that-use-url-params")]
    public string[]? MethodsThatUseUrlParams { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether AJAX requests should be allowed only
    /// to the same domain as the current document.
    /// Defaults to <see langword="false" /> in HTMX 1.x and <see langword="true"/> in HTMX 2.x.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("self-requests-only")]
    public bool? SelfRequestsOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should not update the title of the document
    /// when a title tag is found in new content. Defaults to <see langword="false" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("ignore-title")]
    public bool? IgnoreTitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the target of a boosted element
    /// is scrolled into the viewport. If <c>hx-target</c> is omitted on a boosted element,
    /// the target defaults to body, causing the page to scroll to the top.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("scroll-into-view-on-boost")]
    public bool? ScrollIntoViewOnBoost { get; set; }

    /// <summary>
    /// Gets or sets the cache to store evaluated trigger specifications into,
    /// improving parsing performance at the cost of more memory usage.
    /// You may define a simple object to use a never-clearing cache or implement your own system
    /// using a proxy object. Defaults to <see langword="null" />.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName("trigger-specs-cache")]
    public string? TriggerSpecsCache { get; set; }

    /// <summary>
    /// Gets or sets the default response handling behavior for HTTP response status codes.
    /// Accepts an array of <see cref="ResponseHandlingEntry"/> objects that define
    /// how htmx should handle responses matching specific status code patterns.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x.</remarks>
    [HtmlAttributeName("response-handling")]
    public IList<ResponseHandlingEntry>? ResponseHandling { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to process OOB swaps
    /// on elements that are nested within the main response element.
    /// Defaults to <see langword="true" />.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x.</remarks>
    [HtmlAttributeName("allow-nested-oob-swaps")]
    public bool? AllowNestedOobSwaps { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to treat history cache miss
    /// full page reload requests as an "HX-Request" by returning the corresponding response header.
    /// Defaults to <see langword="true" />.
    /// This should always be disabled when using the <c>HX-Request</c> header
    /// to optionally return partial responses.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x.</remarks>
    [HtmlAttributeName("history-restore-as-hx-request")]
    public bool? HistoryRestoreAsHxRequest { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to report input validation errors
    /// to the end user and update focus to the first input that fails validation.
    /// Defaults to <see langword="false" />.
    /// This should always be enabled as this matches default browser form submit behavior.
    /// </summary>
    /// <remarks>Supported only in HTMX 2.x.</remarks>
    [HtmlAttributeName("report-validity-of-forms")]
    public bool? ReportValidityOfForms { get; set; }

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

        #if NET8_0_OR_GREATER
        var config = new HtmlString(
            JsonSerializer.Serialize(
                new HtmxConfiguration(this),
                HtmxConfigJsonSerializerContext.Default.HtmxConfiguration));
        #else
        var config = new HtmlString(
            JsonSerializer.Serialize(
                new HtmxConfiguration(this),
                JsonOptions.CamelCase));
        #endif

        output.Attributes.SetAttribute(
            new TagHelperAttribute("content", config, HtmlAttributeValueStyle.SingleQuotes));
    }

    #region Inner type: HtmxConfiguration

    /// <summary>
    /// Represents a proxy structure for the <see cref="HtmxConfigTagHelper"/> class.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal readonly struct HtmxConfiguration(HtmxConfigTagHelper helper)
    {
        public bool? HistoryEnabled => helper.HistoryEnabled;
        public int? HistoryCacheSize => helper.HistoryCacheSize;
        public bool? RefreshOnHistoryMiss => helper.RefreshOnHistoryMiss;
        public string? DefaultSwapStyle => helper.DefaultSwapStyle.GetSwapValue();
        public int? DefaultSwapDelay => helper.DefaultSwapDelay;
        public int? DefaultSettleDelay => helper.DefaultSettleDelay;
        public bool? IncludeIndicatorStyles => helper.IncludeIndicatorStyles;
        public string? IndicatorClass => helper.IndicatorClass;
        public string? RequestClass => helper.RequestClass;
        public string? AddedClass => helper.AddedClass;
        public string? SettlingClass => helper.SettlingClass;
        public string? SwappingClass => helper.SwappingClass;
        public bool? AllowEval => helper.AllowEval;
        public bool? AllowScriptTags => helper.AllowScriptTags;
        public string? InlineScriptNonce => helper.InlineScriptNonce;
        public string? InlineStyleNonce => helper.InlineStyleNonce;
        public string[]? AttributesToSettle => helper.AttributesToSettle;
        public bool? UseTemplateFragments => helper.UseTemplateFragments;
        public string? WsReconnectDelay => helper.WsReconnectDelay;
        public string? WsBinaryType => helper.WsBinaryType?.GetWsBinaryTypeValue();
        public string? DisableSelector => helper.DisableSelector;
        public bool? WithCredentials => helper.WithCredentials;
        public bool? DisableInheritance => helper.DisableInheritance;
        public int? Timeout => helper.Timeout;
        public string? ScrollBehavior => helper.ScrollBehavior?.GetScrollBehaviorValue();
        public bool? DefaultFocusScroll => helper.DefaultFocusScroll;
        public bool? GetCacheBusterParam => helper.GetCacheBusterParam;
        public bool? GlobalViewTransitions => helper.GlobalViewTransitions;
        public string[]? MethodsThatUseUrlParams => helper.MethodsThatUseUrlParams;
        public bool? SelfRequestsOnly => helper.SelfRequestsOnly;
        public bool? IgnoreTitle => helper.IgnoreTitle;
        public bool? ScrollIntoViewOnBoost => helper.ScrollIntoViewOnBoost;
        public string? TriggerSpecsCache => helper.TriggerSpecsCache;
        public IList<ResponseHandlingEntry>? ResponseHandling => helper.ResponseHandling;
        public bool? AllowNestedOobSwaps => helper.AllowNestedOobSwaps;
        public bool? HistoryRestoreAsHxRequest => helper.HistoryRestoreAsHxRequest;
        public bool? ReportValidityOfForms => helper.ReportValidityOfForms;
        public AntiForgeryTokenData? AntiForgery => GetAntiForgeryToken(helper);

        private static AntiForgeryTokenData? GetAntiForgeryToken(HtmxConfigTagHelper h) =>
            h.IncludeAntiForgeryToken
                ? new AntiForgeryTokenData(h._antiforgery.GetAndStoreTokens(h.ViewContext.HttpContext))
                : null;
    }

    #endregion

    #region Inner type: AntiForgeryTokenData

    /// <summary>
    /// Represents a proxy structure for the <see cref="AntiforgeryTokenSet"/> class.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal readonly struct AntiForgeryTokenData(AntiforgeryTokenSet antiforgery)
    {
        public string? HeaderName => antiforgery.HeaderName;
        public string FormFieldName => antiforgery.FormFieldName;
        public string? RequestToken => antiforgery.RequestToken;
    }

    #endregion
}
