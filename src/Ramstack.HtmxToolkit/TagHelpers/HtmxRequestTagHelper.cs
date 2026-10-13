using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Renders request options as <c>hx-request</c> (HTMX 1.9.x and 2.x) or <c>hx-config</c> (HTMX 4.x).
/// </summary>
/// <remarks>
/// <para>HTMX 1.9.x and 2.x merge-inherit <c>hx-request</c> from parent elements.</para>
/// <para>
///   HTMX 4.x supports the <c>inherited</c> and <c>append</c> modifiers on <c>hx-config</c>
///   and can enable inheritance globally with <see cref="HtmxV4Config.ImplicitInheritance" />.
/// </para>
/// <para>
///   Options that the selected HTMX version does not support are not rendered.
///   If no supported option is set, the tag helper renders no attribute.
/// </para>
/// </remarks>
[HtmlTargetElement(Attributes = RequestInheritedAttributeName)]
[HtmlTargetElement(Attributes = RequestAppendAttributeName)]
[HtmlTargetElement(Attributes = RequestTimeoutAttributeName)]
[HtmlTargetElement(Attributes = RequestCredentialsAttributeName)]
[HtmlTargetElement(Attributes = RequestNoHeadersAttributeName)]
[HtmlTargetElement(Attributes = RequestCacheAttributeName)]
[HtmlTargetElement(Attributes = RequestRedirectAttributeName)]
[HtmlTargetElement(Attributes = RequestReferrerAttributeName)]
[HtmlTargetElement(Attributes = RequestIntegrityAttributeName)]
[HtmlTargetElement(Attributes = RequestValidateAttributeName)]
public sealed class HtmxRequestTagHelper(IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string RequestInheritedAttributeName = "hx-request-inherited";
    private const string RequestAppendAttributeName = "hx-request-append";
    private const string RequestTimeoutAttributeName = "hx-request-timeout";
    private const string RequestCredentialsAttributeName = "hx-request-credentials";
    private const string RequestNoHeadersAttributeName = "hx-request-no-headers";
    private const string RequestCacheAttributeName = "hx-request-cache";
    private const string RequestRedirectAttributeName = "hx-request-redirect";
    private const string RequestReferrerAttributeName = "hx-request-referrer";
    private const string RequestIntegrityAttributeName = "hx-request-integrity";
    private const string RequestValidateAttributeName = "hx-request-validate";

    private readonly HtmxRequestData _request = new();

    /// <summary>
    /// Gets or sets a value indicating whether the generated attribute is explicitly inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   HTMX 1.9.x and 2.x merge-inherit request configuration without a modifier,
    ///   so the property does not change the generated attribute name for them.
    /// </para>
    /// <para>
    ///   For HTMX 4.x, the tag helper emits the <c>inherited</c> modifier
    ///   when this property is <see langword="true" />.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(RequestInheritedAttributeName)]
    public bool Inherited { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the generated attribute is appended to inherited configuration.
    /// </summary>
    /// <remarks>
    /// <para>The property does not change the generated attribute name for HTMX 1.9.x and 2.x.</para>
    /// <para>
    ///   For HTMX 4.x, the tag helper emits the <c>append</c> modifier
    ///   when this property is <see langword="true" />.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(RequestAppendAttributeName)]
    public bool Append { get; set; }

    /// <summary>
    /// Gets or sets the timeout for the request in milliseconds.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, 2.x, and 4.x.</remarks>
    [HtmlAttributeName(RequestTimeoutAttributeName)]
    public int? Timeout
    {
        get => _request.Timeout;
        set => _request.Timeout = value;
    }

    /// <summary>
    /// Gets or sets the credentials mode for the request.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   In HTMX 1.9.x and 2.x, this property maps to the boolean
    ///   <c>credentials</c> option of <c>hx-request</c>.
    ///   <see cref="HtmxRequestCredentials.Include" /> yields <see langword="true" />.
    ///   <see cref="HtmxRequestCredentials.SameOrigin" /> yields <see langword="false" />.
    ///   <see cref="HtmxRequestCredentials.Omit" /> is unsupported, so the option is omitted
    ///   and HTMX uses its default value.
    /// </para>
    /// <para>
    ///   In HTMX 4.x, this property maps to the string <c>credentials</c> option
    ///   of <c>hx-config</c>.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(RequestCredentialsAttributeName)]
    public HtmxRequestCredentials? Credentials
    {
        get => _request.Credentials;
        set => _request.Credentials = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether headers set by HTMX are omitted,
    /// including custom headers supplied through <c>hx-headers</c>.
    /// </summary>
    /// <remarks>
    /// <para>Supported only in HTMX 1.9.x and 2.x.</para>
    /// <para>Headers added by the browser are unaffected.</para>
    /// <para>The Toolkit script also omits the <c>HX-Request-Type</c> header.</para>
    /// </remarks>
    [HtmlAttributeName(RequestNoHeadersAttributeName)]
    public bool? NoHeaders
    {
        get => _request.NoHeaders;
        set => _request.NoHeaders = value;
    }

    /// <summary>
    /// Gets or sets the Fetch cache mode for the request.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName(RequestCacheAttributeName)]
    public HtmxRequestCache? Cache
    {
        get => _request.Cache;
        set => _request.Cache = value;
    }

    /// <summary>
    /// Gets or sets the Fetch redirect mode for the request.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName(RequestRedirectAttributeName)]
    public HtmxRequestRedirect? Redirect
    {
        get => _request.Redirect;
        set => _request.Redirect = value;
    }

    /// <summary>
    /// Gets or sets the Fetch API <c>referrer</c> value for the request.
    /// </summary>
    /// <remarks>
    /// <para>Supported only in HTMX 4.x.</para>
    /// <para>
    ///   Accepts a same-origin URL, an empty string to omit the referrer,
    ///   or <c>about:client</c> to use the default referrer.
    ///   The browser replaces a URL from another origin with the default referrer.
    /// </para>
    /// <para>
    ///   The separate Fetch API <c>referrerPolicy</c> option is not configured by this property.
    ///   Policy values such as <c>no-referrer</c> are treated as relative URLs;
    ///   use an empty string to omit the referrer.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(RequestReferrerAttributeName)]
    public string? Referrer
    {
        get => _request.Referrer;
        set => _request.Referrer = value;
    }

    /// <summary>
    /// Gets or sets the subresource integrity value for the request.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName(RequestIntegrityAttributeName)]
    public string? Integrity
    {
        get => _request.Integrity;
        set => _request.Integrity = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form is validated before submission.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName(RequestValidateAttributeName)]
    public bool? Validate
    {
        get => _request.Validate;
        set => _request.Validate = value;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var json = options.Value.TargetVersion == HtmxTargetVersion.V4
            ? JsonSerializer.Serialize(new HtmxRequestDataV4(_request), HtmxRequestJsonSerializerContext.Default.HtmxRequestDataV4)
            : JsonSerializer.Serialize(new HtmxRequestDataPrior(_request), HtmxRequestJsonSerializerContext.Default.HtmxRequestDataPrior);

        if (json != "{}")
        {
            var name = options.Value.TargetVersion switch
            {
                HtmxTargetVersion.V4 when Inherited && Append => "hx-config:inherited:append",
                HtmxTargetVersion.V4 when Inherited => "hx-config:inherited",
                HtmxTargetVersion.V4 when Append => "hx-config:append",
                HtmxTargetVersion.V4 => "hx-config",
                _ => "hx-request"
            };

            output.Attributes.SetAttribute(
                new TagHelperAttribute(name, new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes));
        }

        return Task.CompletedTask;
    }

    #region Inner type: HtmxRequestData

    /// <summary>
    /// Stores the request options before they are projected for the target HTMX version.
    /// </summary>
    internal sealed class HtmxRequestData
    {
        /// <inheritdoc cref="HtmxRequestTagHelper.Timeout" />
        public int? Timeout { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Credentials" />
        public HtmxRequestCredentials? Credentials { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.NoHeaders" />
        public bool? NoHeaders { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Cache" />
        public HtmxRequestCache? Cache { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Redirect" />
        public HtmxRequestRedirect? Redirect { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Referrer" />
        public string? Referrer { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Integrity" />
        public string? Integrity { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Validate" />
        public bool? Validate { get; set; }
    }

    #endregion

    #region Inner type: HtmxRequestDataPrior

    /// <summary>
    /// Projects request configuration into the <c>hx-request</c> contract used by HTMX 1.9.x and 2.x.
    /// </summary>
    /// <param name="data">The request options to project.</param>
    internal readonly struct HtmxRequestDataPrior(HtmxRequestData data)
    {
        /// <inheritdoc cref="HtmxRequestData.Timeout" />
        public int? Timeout => data.Timeout;

        /// <summary>
        /// Gets the boolean credentials value supported by HTMX 1.9.x and 2.x.
        /// </summary>
        public bool? Credentials => data.Credentials switch
        {
            HtmxRequestCredentials.SameOrigin => false,
            HtmxRequestCredentials.Include => true,
            // TODO: Consider throwing an exception when an HTMX 4-only credentials mode is configured for HTMX 1.9.x or 2.x.
            // HtmxRequestCredentials.Omit => throw new InvalidOperationException(),
            _ => null
        };

        /// <inheritdoc cref="HtmxRequestData.NoHeaders" />
        public bool? NoHeaders => data.NoHeaders;
    }

    #endregion

    #region Inner type: HtmxRequestDataV4

    /// <summary>
    /// Projects request configuration into the <c>hx-config</c> contract used by HTMX 4.x.
    /// </summary>
    /// <param name="data">The request options to project.</param>
    internal readonly struct HtmxRequestDataV4(HtmxRequestData data)
    {
        /// <inheritdoc cref="HtmxRequestData.Timeout" />
        public int? Timeout => data.Timeout;

        /// <summary>
        /// Gets the Fetch API credentials mode supported by HTMX 4.x.
        /// </summary>
        public string? Credentials => data.Credentials switch
        {
            HtmxRequestCredentials.SameOrigin => "same-origin",
            HtmxRequestCredentials.Include => "include",
            HtmxRequestCredentials.Omit => "omit",
            _ => null
        };

        /// <inheritdoc cref="HtmxRequestData.Cache" />
        public string? Cache => data.Cache switch
        {
            HtmxRequestCache.Default => "default",
            HtmxRequestCache.NoStore => "no-store",
            HtmxRequestCache.Reload => "reload",
            HtmxRequestCache.NoCache => "no-cache",
            HtmxRequestCache.ForceCache => "force-cache",
            HtmxRequestCache.OnlyIfCached => "only-if-cached",
            _ => null
        };

        /// <inheritdoc cref="HtmxRequestData.Redirect" />
        public string? Redirect => data.Redirect switch
        {
            HtmxRequestRedirect.Follow => "follow",
            HtmxRequestRedirect.Error => "error",
            HtmxRequestRedirect.Manual => "manual",
            _ => null
        };

        /// <inheritdoc cref="HtmxRequestData.Referrer" />
        public string? Referrer => data.Referrer;

        /// <inheritdoc cref="HtmxRequestData.Integrity" />
        public string? Integrity => data.Integrity;

        /// <inheritdoc cref="HtmxRequestData.Validate" />
        public bool? Validate => data.Validate;
    }

    #endregion
}
