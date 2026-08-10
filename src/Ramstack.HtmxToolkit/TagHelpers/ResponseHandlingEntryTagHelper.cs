using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents a <see cref="TagHelper"/> that defines a single response handling entry
/// as a child of the <c>htmx-config</c> tag helper.
/// Each entry specifies how htmx should handle responses matching a particular HTTP status code pattern.
/// </summary>
/// <remarks>Supported only in HTMX 2.x.</remarks>
[HtmlTargetElement("response-handling", ParentTag = "htmx-config", TagStructure = TagStructure.WithoutEndTag)]
public sealed class ResponseHandlingEntryTagHelper : TagHelper
{
    /// <summary>
    /// Gets or sets a regular expression that will be tested against response status codes.
    /// </summary>
    [HtmlAttributeName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the response should be swapped into the DOM.
    /// </summary>
    [HtmlAttributeName("swap")]
    public bool? Swap { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should treat this response as an error.
    /// </summary>
    [HtmlAttributeName("error")]
    public bool? Error { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should ignore title tags in the response.
    /// </summary>
    [HtmlAttributeName("ignore-title")]
    public bool? IgnoreTitle { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector to use to select content from the response.
    /// </summary>
    [HtmlAttributeName("select")]
    public string? Select { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector specifying an alternative target for the response.
    /// </summary>
    [HtmlAttributeName("target")]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets an alternative swap mechanism for the response.
    /// </summary>
    [HtmlAttributeName("swap-override")]
    public string? SwapOverride { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context.Items.TryGetValue(typeof(HtmxConfigTagHelper), out var value))
        {
            if (value is HtmxConfigTagHelper config)
            {
                config.ResponseHandling ??= new List<ResponseHandlingEntry>();
                config.ResponseHandling.Add(new ResponseHandlingEntry
                {
                    Code = Code,
                    Swap = Swap,
                    Error = Error,
                    IgnoreTitle = IgnoreTitle,
                    Select = Select,
                    Target = Target,
                    SwapOverride = SwapOverride
                });
            }
        }

        output.SuppressOutput();
    }
}
