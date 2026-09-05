using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Applies version-specific HTMX request configuration to matching elements.
/// </summary>
/// <remarks>
/// <para>HTMX 1.x and 2.x use the merge-inherited <c>hx-request</c> attribute.</para>
/// <para>
///   HTMX 4.x uses <c>hx-config</c> and requires either the explicit inheritance modifier
///   or the global <see cref="HtmxV4Config.ImplicitInheritance" /> option for inheritance.
/// </para>
/// </remarks>
[HtmlTargetElement(Attributes = RequestInheritedAttributeName)]
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
    /// <para>HTMX 1.x and 2.x merge-inherit request configuration without a modifier.</para>
    /// <para>HTMX 4.x emits the <c>inherited</c> modifier when this property is <see langword="true" />.</para>
    /// </remarks>
    [HtmlAttributeName(RequestInheritedAttributeName)]
    public bool Inherited { get; set; }

    /// <summary>
    /// Gets or sets the timeout for the request in milliseconds.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x, 2.x, and 4.x.</remarks>
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
    ///   In HTMX 1.x and 2.x, this property maps to the boolean
    ///   <c>credentials</c> option of <c>hx-request</c>.
    ///   <see cref="HtmxRequestCredentials.Include" /> yields <see langword="true" />.
    ///   <see cref="HtmxRequestCredentials.SameOrigin" /> yields <see langword="false" />.
    /// </para>
    /// <para>
    ///   In HTMX 4.x, this property maps to the string <c>credentials</c> option
    ///   of <c>hx-config</c>.
    /// </para>
    /// <para>
    ///   <see cref="HtmxRequestCredentials.Omit" /> is unsupported in HTMX 1.x and 2.x,
    ///   so the option is omitted and HTMX uses its default value.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(RequestCredentialsAttributeName)]
    public HtmxRequestCredentials? Credentials
    {
        get => _request.Credentials;
        set => _request.Credentials = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX-specific request headers are omitted.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x. Removed in HTMX 4.x.</remarks>
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
    public string? Cache
    {
        get => _request.Cache;
        set => _request.Cache = value;
    }

    /// <summary>
    /// Gets or sets the Fetch redirect mode for the request.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    [HtmlAttributeName(RequestRedirectAttributeName)]
    public string? Redirect
    {
        get => _request.Redirect;
        set => _request.Redirect = value;
    }

    /// <summary>
    /// Gets or sets the referrer URL or referrer policy for the request.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
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
            var name = "hx-request";

            if (options.Value.TargetVersion == HtmxTargetVersion.V4)
            {
                if (Inherited)
                {
                    if (options.Value.HtmxConfig is HtmxV4Config config)
                        name = string.IsNullOrEmpty(config.MetaCharacter)
                            ? "hx-config:inherited"
                            : $"hx-config{config.MetaCharacter}inherited";
                }
                else
                {
                    name = "hx-config";
                }
            }

            output.Attributes.SetAttribute(
                new TagHelperAttribute(name, new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes));
        }

        return Task.CompletedTask;
    }

    #region Inner type: HtmxRequestData

    /// <summary>
    /// Stores the request configuration shared by all supported HTMX versions.
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
        public string? Cache { get; set; }

        /// <inheritdoc cref="HtmxRequestTagHelper.Redirect" />
        public string? Redirect { get; set; }

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
    /// Projects request configuration into the <c>hx-request</c> contract used by HTMX 1.x and 2.x.
    /// </summary>
    /// <param name="data">The shared request configuration.</param>
    internal readonly struct HtmxRequestDataPrior(HtmxRequestData data)
    {
        /// <inheritdoc cref="HtmxRequestData.Timeout" />
        public int? Timeout => data.Timeout;

        /// <summary>
        /// Gets the boolean credentials value supported by HTMX 1.x and 2.x.
        /// </summary>
        public bool? Credentials => data.Credentials switch
        {
            HtmxRequestCredentials.SameOrigin => false,
            HtmxRequestCredentials.Include => true,
            // TODO: Consider throwing an exception when an HTMX 4-only credentials mode is configured for HTMX 1.x or 2.x.
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
    /// <param name="data">The shared request configuration.</param>
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
        public string? Cache => data.Cache;

        /// <inheritdoc cref="HtmxRequestData.Redirect" />
        public string? Redirect => data.Redirect;

        /// <inheritdoc cref="HtmxRequestData.Referrer" />
        public string? Referrer => data.Referrer;

        /// <inheritdoc cref="HtmxRequestData.Integrity" />
        public string? Integrity => data.Integrity;

        /// <inheritdoc cref="HtmxRequestData.Validate" />
        public bool? Validate => data.Validate;
    }

    #endregion
}
