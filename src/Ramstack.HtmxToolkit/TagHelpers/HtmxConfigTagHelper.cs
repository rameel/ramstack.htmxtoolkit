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
/// Represents the <see cref="ITagHelper" /> implementation targeting &lt;meta&gt; elements to set htmx options declaratively.
/// </summary>
[HtmlTargetElement("meta", Attributes = "htmx-config", TagStructure = TagStructure.WithoutEndTag)]
public sealed class HtmxConfigTagHelper(IAntiforgery antiforgery) : TagHelper
{
    /// <summary>
    /// Gets or sets a value indicating whether history is enabled.
    /// Defaults to <c>true</c>. This is mainly useful for testing.
    /// </summary>
    [HtmlAttributeName("history-enabled")]
    public bool? HistoryEnabled { get; set; }

    /// <summary>
    /// Gets or sets the size of the history cache. Defaults to <c>10</c>.
    /// </summary>
    [HtmlAttributeName("history-cache-size")]
    public int? HistoryCacheSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a full page refresh
    /// should be issued on history misses rather than using an AJAX request. Defaults to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("refresh-on-history-miss")]
    public bool? RefreshOnHistoryMiss { get; set; }

    /// <summary>
    /// Gets or sets the default swap style. Defaults to <see cref="HtmxSwap.InnerHtml"/>.
    /// </summary>
    [HtmlAttributeName("default-swap-style")]
    public HtmxSwap? DefaultSwapStyle { get; set; }

    /// <summary>
    /// Gets or sets the default swap delay. Defaults to <c>0</c>.
    /// </summary>
    [HtmlAttributeName("default-swap-delay")]
    public int? DefaultSwapDelay { get; set; }

    /// <summary>
    /// Gets or sets the default settle delay. Defaults to <c>100</c>.
    /// </summary>
    [HtmlAttributeName("default-settle-delay")]
    public int? DefaultSettleDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the indicator styles are loaded. Defaults to <c>true</c>.
    /// </summary>
    [HtmlAttributeName("include-indicator-styles")]
    public bool? IncludeIndicatorStyles { get; set; }

    /// <summary>
    /// Gets or sets the indicator class. Defaults to <c>htmx-indicator</c>.
    /// </summary>
    [HtmlAttributeName("indicator-class")]
    public string? IndicatorClass { get; set; }

    /// <summary>
    /// Gets or sets the request class. Defaults to <c>htmx-request</c>.
    /// </summary>
    [HtmlAttributeName("request-class")]
    public string? RequestClass { get; set; }

    /// <summary>
    /// Gets or sets the added class. Defaults to <c>htmx-added</c>.
    /// </summary>
    [HtmlAttributeName("added-class")]
    public string? AddedClass { get; set; }

    /// <summary>
    /// Gets or sets the settling class. Defaults to <c>htmx-settling</c>.
    /// </summary>
    [HtmlAttributeName("settling-class")]
    public string? SettlingClass { get; set; }

    /// <summary>
    /// Gets or sets the swapping class. Defaults to <c>htmx-swapping</c>.
    /// </summary>
    [HtmlAttributeName("swapping-class")]
    public string? SwappingClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether eval is allowed.
    /// Defaults to <c>true</c>.
    /// </summary>
    [HtmlAttributeName("allow-eval")]
    public bool? AllowEval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether script tags should be processed in new content.
    /// Defaults to <c>true</c>.
    /// </summary>
    [HtmlAttributeName("allow-script-tags")]
    public bool? AllowScriptTags { get; set; }

    /// <summary>
    /// Gets or sets a value meaning that no nonce will be added to inline scripts.
    /// Defaults to <c>""</c>.
    /// </summary>
    [HtmlAttributeName("inline-script-nonce")]
    public string? InlineScriptNonce { get; set; }

    /// <summary>
    /// Gets or sets the attributes to settle during the settling phase.
    /// Defaults to <c>["class", "style", "width", "height"]</c>.
    /// </summary>
    [HtmlAttributeName("attributes-to-settle")]
    public string[]? AttributesToSettle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTML template tags should be used for parsing content.
    /// Defaults to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("use-template-fragments")]
    public bool? UseTemplateFragments { get; set; }

    /// <summary>
    /// Gets or sets the WebSocket reconnect delay. Defaults to <c>full-jitter</c>.
    /// </summary>
    [HtmlAttributeName("ws-reconnect-delay")]
    public string? WsReconnectDelay { get; set; }

    /// <summary>
    /// Gets or sets the <a href="https://developer.mozilla.org/docs/Web/API/WebSocket/binaryType">type of binary data</a>
    /// being received over the WebSocket connection. Defaults to <see cref="HtmxBinaryType.Blob"/>.
    /// </summary>
    [HtmlAttributeName("ws-binary-type")]
    public HtmxBinaryType? WsBinaryType { get; set; }

    /// <summary>
    /// Gets or sets the disable selector.
    /// Defaults to <c>[disable-htmx], [data-disable-htmx]</c>.
    /// HTMX will not process elements with this attribute on it or a parent.
    /// </summary>
    [HtmlAttributeName("disable-selector")]
    public string? DisableSelector { get; set; }

    /// <summary>
    /// Gets or sets the value that allows cross-site <c>Access-Control</c> requests
    /// using credentials such as cookies, authorization headers or TLS client certificates.
    /// Defaults to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("with-credentials")]
    public bool? WithCredentials { get; set; }

    /// <summary>
    /// Gets or sets the number of milliseconds a request can take before automatically being terminated.
    /// Defaults to <c>0</c>.
    /// </summary>
    [HtmlAttributeName("timeout")]
    public int? Timeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the behavior for a boosted link on page transitions.
    /// Defaults to <see cref="HtmxScrollBehavior.Smooth"/>.
    /// </summary>
    [HtmlAttributeName("scroll-behavior")]
    public HtmxScrollBehavior? ScrollBehavior { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the focused element should be scrolled into view.
    /// Defaults to <c>false</c> and can be overridden using the <c>focus-scroll</c> swap modifier.
    /// </summary>
    [HtmlAttributeName("default-focus-scroll")]
    public bool? DefaultFocusScroll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a cache-busting parameter
    /// should be included in GET requests to avoid caching partial responses by the browser.
    /// Defaults to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("get-cache-buster-param")]
    public bool? GetCacheBusterParam { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/API/View_Transitions_API">View Transition API</a>
    /// should be used when swapping in new content.
    /// </summary>
    [HtmlAttributeName("global-view-transitions")]
    public bool? GlobalViewTransitions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX should format requests
    /// with this method by encoding their parameters in the URL, not the request body.
    /// Defaults to <c>["get"]</c>.
    /// Allowed values: <c>"get"</c>, <c>"head"</c>, <c>"post"</c>, <c>"put"</c>,
    /// <c>"delete"</c>, <c>"connect"</c>, <c>"options"</c>, <c>"trace"</c>, <c>"patch"</c>.
    /// </summary>
    [HtmlAttributeName("methods-that-use-url-params")]
    public string[]? MethodsThatUseUrlParams { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether AJAX requests should be allowed only
    /// to the same domain as the current document. Defaults to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("self-requests-only")]
    public bool? SelfRequestsOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should not update the title of the document
    /// when a title tag is found in new content. Defaults to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("ignore-title")]
    public bool? IgnoreTitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the target of a boosted element
    /// is scrolled into the viewport. If <c>hx-target</c> is omitted on a boosted element,
    /// the target defaults to body, causing the page to scroll to the top.
    /// Defaults to <c>true</c>.
    /// </summary>
    [HtmlAttributeName("scroll-into-view-on-boost")]
    public bool? ScrollIntoViewOnBoost { get; set; }

    /// <summary>
    /// Gets or sets a value the cache to store evaluated trigger specifications into,
    /// improving parsing performance at the cost of more memory usage.
    /// You may define a simple object to use a never-clearing cache,
    /// or implement your own system using a proxy object. Defaults to <c>null</c>.
    /// </summary>
    [HtmlAttributeName("trigger-specs-cache")]
    public string? TriggerSpecsCache { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether antiforgery should be included.
    /// Default to <c>false</c>.
    /// </summary>
    [HtmlAttributeName("include-antiforgery-token")]
    public bool IncludeAntiForgeryToken { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext"/>.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("name", "htmx-config");

        var config = new HtmlString(
            JsonSerializer.Serialize(
                new HtmxConfiguration(this, antiforgery),
                JsonOptions.CamelCase));

        output.Attributes.SetAttribute(
            new TagHelperAttribute("content", config, HtmlAttributeValueStyle.SingleQuotes));

        return Task.CompletedTask;
    }

    #region Inner type: HtmxConfiguration

    /// <summary>
    /// Represents a proxy structure for the <see cref="HtmxConfigTagHelper"/> class.
    /// This structure is used to allow the JSON serializer to serialize only the necessary properties.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private readonly struct HtmxConfiguration(HtmxConfigTagHelper helper, IAntiforgery antiforgery)
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
        public string[]? AttributesToSettle => helper.AttributesToSettle;
        public bool? UseTemplateFragments => helper.UseTemplateFragments;
        public string? WsReconnectDelay => helper.WsReconnectDelay;
        public string? WsBinaryType => helper.WsBinaryType?.GetWsBinaryTypeValue();
        public string? DisableSelector => helper.DisableSelector;
        public bool? WithCredentials => helper.WithCredentials;
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
        public AntiForgeryTokenData? AntiForgery => GetAntiForgeryToken();

        private AntiForgeryTokenData? GetAntiForgeryToken() =>
            helper.IncludeAntiForgeryToken
                ? new AntiForgeryTokenData(antiforgery.GetAndStoreTokens(helper.ViewContext.HttpContext))
                : null;
    }

    #endregion

    #region Inner type: AntiForgeryTokenData

    /// <summary>
    /// Represents a proxy structure for the <see cref="AntiforgeryTokenSet"/> class.
    /// This structure is used to allow the JSON serializer to serialize only the necessary properties.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private readonly struct AntiForgeryTokenData(AntiforgeryTokenSet antiforgery)
    {
        public string? HeaderName => antiforgery.HeaderName;
        public string FormFieldName => antiforgery.FormFieldName;
        public string? RequestToken => antiforgery.RequestToken;
    }

    #endregion
}
