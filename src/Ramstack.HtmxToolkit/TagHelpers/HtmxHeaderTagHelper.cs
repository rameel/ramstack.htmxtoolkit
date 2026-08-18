using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents a <see cref="TagHelper"/> implementation that applies the <c>hx-headers</c> attribute to matching elements.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item><c>hx-headers</c> is inherited and can be placed on a parent element.</item>
///   <item>A child declaration of a header overrides a parent declaration.</item>
/// </list>
/// </remarks>
[HtmlTargetElement(Attributes = HeadersDictionaryName)]
[HtmlTargetElement(Attributes = HeadersPrefix + "*")]
public sealed class HtmxHeaderTagHelper : TagHelper
{
    private const string HeadersPrefix = "hx-header-";
    private const string HeadersDictionaryName = "hx-all-headers";

    /// <summary>
    /// Gets or sets the <c>hx-headers</c> attribute values.
    /// </summary>
    [HtmlAttributeName(HeadersDictionaryName, DictionaryAttributePrefix = HeadersPrefix)]
    public IDictionary<string, string> Headers
    {
        get => field ??= new Dictionary<string, string>();
        set;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Headers is { Count: > 0 })
        {
            var info = HtmxDictionaryJsonSerializerContext.Default.IDictionaryStringString;
            var headers = new HtmlString(JsonSerializer.Serialize(Headers, info));

            output.Attributes.SetAttribute(
                new TagHelperAttribute("hx-headers", headers, HtmlAttributeValueStyle.SingleQuotes)
                );
        }

        return Task.CompletedTask;
    }
}
