using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents the <see cref="ITagHelper" /> implementation that renders
/// the application-wide HTMX configuration as a meta element.
/// </summary>
/// <param name="antiforgery">The service used to generate antiforgery tokens.</param>
/// <param name="options">The configured HTMX Toolkit options.</param>
[HtmlTargetElement("meta", Attributes = "htmx-config", TagStructure = TagStructure.WithoutEndTag)]
[HtmlTargetElement("htmx-config", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class HtmxConfigTagHelper(IAntiforgery antiforgery, IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string RequestTokenAttributeName = "data-antiforgery-request-token";
    private const string HeaderNameAttributeName = "data-antiforgery-header-name";
    private const string FormFieldNameAttributeName = "data-antiforgery-form-field-name";

    /// <summary>
    /// Gets or sets the current view context.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (output.TagName == "meta")
            output.Attributes.RemoveAll("htmx-config");

        output.TagName = "meta";
        output.TagMode = TagMode.SelfClosing;
        output.Attributes.SetAttribute("name", "htmx-config");

        var json = options.Value.HtmxConfig.ToJson();

        output.Attributes.SetAttribute(
            new TagHelperAttribute("content", new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes));

        if (options.Value.IncludeAntiforgeryToken)
            RenderAntiforgeryAttributes(output);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Renders antiforgery request metadata into the output element.
    /// </summary>
    /// <param name="output">The tag helper output to update.</param>
    private void RenderAntiforgeryAttributes(TagHelperOutput output)
    {
        var tokens = antiforgery.GetAndStoreTokens(ViewContext.HttpContext);

        output.Attributes.Add(RequestTokenAttributeName, tokens.RequestToken);
        output.Attributes.Add(HeaderNameAttributeName, tokens.HeaderName);
        output.Attributes.Add(FormFieldNameAttributeName, tokens.FormFieldName);
    }
}
