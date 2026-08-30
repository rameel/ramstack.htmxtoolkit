using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents a <see cref="TagHelper"/> implementation that applies request configuration to matching elements.
/// </summary>
/// <remarks>
/// <para>HTMX 1.x and 2.x use merge-inherited <c>hx-request</c></para>
/// <para>HTMX 4.x uses <c>hx-config</c></para>
/// </remarks>
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
    ///   In HTMX 1.x and 2.x this maps to the <c>credentials</c> boolean option of <c>hx-request</c>,
    ///   where <see cref="HtmxRequestCredentials.Include"/> yields <see langword="true" /> and
    ///   <see cref="HtmxRequestCredentials.SameOrigin"/> yields <see langword="false" />.
    /// </para>
    /// <para>
    ///   In HTMX 4.x this maps to the <c>credentials</c> string option of <c>hx-config</c>.
    /// </para>
    /// <para>
    ///   <see cref="HtmxRequestCredentials.Omit"/> is unsupported in HTMX 1.x and 2.x, so the option
    ///   is omitted and HTMX uses its default value. A future version may throw an exception instead.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(RequestCredentialsAttributeName)]
    public HtmxRequestCredentials? Credentials
    {
        get => _request.Credentials;
        set => _request.Credentials = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX strips all request headers.
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
        var targetVersion = options.Value.TargetVersion;
        var request = targetVersion == HtmxTargetVersion.V4
            ? JsonSerializer.Serialize(new HtmxRequestDataV4(_request), HtmxRequestJsonSerializerContext.Default.HtmxRequestDataV4)
            : JsonSerializer.Serialize(new HtmxRequestDataLegacy(_request), HtmxRequestJsonSerializerContext.Default.HtmxRequestDataLegacy);

        if (request != "{}")
        {
            var attributeName = targetVersion == HtmxTargetVersion.V4 ? "hx-config" : "hx-request";
            output.Attributes.SetAttribute(
                new TagHelperAttribute(attributeName, new HtmlString(request), HtmlAttributeValueStyle.SingleQuotes));
        }

        return Task.CompletedTask;
    }

    #region Inner types

    /// <summary>
    /// Represents all typed request configuration data.
    /// </summary>
    internal sealed class HtmxRequestData
    {
        public int? Timeout { get; set; }
        public HtmxRequestCredentials? Credentials { get; set; }
        public bool? NoHeaders { get; set; }
        public string? Cache { get; set; }
        public string? Redirect { get; set; }
        public string? Referrer { get; set; }
        public string? Integrity { get; set; }
        public bool? Validate { get; set; }
    }

    /// <summary>
    /// Projects request configuration into the <c>hx-request</c> contract used by HTMX 1.x and 2.x.
    /// </summary>
    internal readonly struct HtmxRequestDataLegacy(HtmxRequestData data)
    {
        public int? Timeout => data.Timeout;

        public bool? Credentials => data.Credentials switch
        {
            HtmxRequestCredentials.SameOrigin => false,
            HtmxRequestCredentials.Include => true,
            // TODO: Consider throwing an exception when an HTMX 4-only credentials mode is configured for HTMX 1.x or 2.x
            // HtmxRequestCredentials.Omit => throw new InvalidOperationException(),
            _ => null
        };

        public bool? NoHeaders => data.NoHeaders;
    }

    /// <summary>
    /// Projects request configuration into the <c>hx-config</c> contract used by HTMX 4.x.
    /// </summary>
    internal readonly struct HtmxRequestDataV4(HtmxRequestData data)
    {
        public int? Timeout => data.Timeout;
        public string? Credentials => data.Credentials switch
        {
            HtmxRequestCredentials.SameOrigin => "same-origin",
            HtmxRequestCredentials.Include => "include",
            HtmxRequestCredentials.Omit => "omit",
            _ => null
        };

        public string? Cache => data.Cache;
        public string? Redirect => data.Redirect;
        public string? Referrer => data.Referrer;
        public string? Integrity => data.Integrity;
        public bool? Validate => data.Validate;
    }

    #endregion
}
