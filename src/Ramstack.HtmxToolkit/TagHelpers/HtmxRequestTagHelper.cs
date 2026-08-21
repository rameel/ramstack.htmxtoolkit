using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents a <see cref="TagHelper"/> implementation that applies the <c>hx-request</c> attribute to matching elements.
/// </summary>
/// <remarks>
/// <c>hx-request</c> is merge-inherited and can be placed on a parent element.
/// </remarks>
[HtmlTargetElement(Attributes = RequestTimeoutAttributeName)]
[HtmlTargetElement(Attributes = RequestCredentialsAttributeName)]
[HtmlTargetElement(Attributes = RequestNoHeadersAttributeName)]
public sealed class HtmxRequestTagHelper : TagHelper
{
    private const string RequestTimeoutAttributeName = "hx-request-timeout";
    private const string RequestCredentialsAttributeName = "hx-request-credentials";
    private const string RequestNoHeadersAttributeName = "hx-request-no-headers";

    private readonly HtmxRequestData _request = new();

    /// <summary>
    /// Gets or sets the timeout for the request in milliseconds.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName(RequestTimeoutAttributeName)]
    public int? Timeout
    {
        get => _request.Timeout;
        set => _request.Timeout = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the request sends credentials.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName(RequestCredentialsAttributeName)]
    public bool? Credentials
    {
        get => _request.Credentials;
        set => _request.Credentials = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether htmx strips all request headers.
    /// </summary>
    /// <remarks>Supported in HTMX 1.x and 2.x.</remarks>
    [HtmlAttributeName(RequestNoHeadersAttributeName)]
    public bool? NoHeaders
    {
        get => _request.NoHeaders;
        set => _request.NoHeaders = value;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Timeout is not null || Credentials is not null || NoHeaders is not null)
        {
            var info = HtmxRequestJsonSerializerContext.Default.HtmxRequestData;
            var request = new HtmlString(JsonSerializer.Serialize(_request, info));
            output.Attributes.SetAttribute(new TagHelperAttribute("hx-request", request));
        }

        return Task.CompletedTask;
    }

    #region Inner type: HtmxRequestData

    /// <summary>
    /// Represents the serializable request configuration data.
    /// </summary>
    internal sealed class HtmxRequestData
    {
        public int? Timeout { get; set; }
        public bool? Credentials { get; set; }
        public bool? NoHeaders { get; set; }
    }

    #endregion
}
