using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Applies an <c>hx-vals</c> attribute assembled from strongly typed tag helper attributes.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item><c>hx-vals</c> is inherited and can be placed on a parent element.</item>
///   <item>A child declaration of a value overrides a parent declaration.</item>
/// </list>
/// </remarks>
[HtmlTargetElement(Attributes = ValuesDictionaryName)]
[HtmlTargetElement(Attributes = ValuesPrefix + "*")]
public sealed class HtmxValsTagHelper : TagHelper
{
    private const string ValuesPrefix = "hx-val-";
    private const string ValuesDictionaryName = "hx-all-vals";

    /// <summary>
    /// Gets or sets the <c>hx-vals</c> attribute values.
    /// </summary>
    [HtmlAttributeName(ValuesDictionaryName, DictionaryAttributePrefix = ValuesPrefix)]
    public IDictionary<string, string> Values
    {
        get => field ??= new SmallDictionary<string, string>(StringComparer.Ordinal);
        set;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Values is { Count: > 0 } values)
        {
            var info = HtmxDictionaryJsonSerializerContext.Default.IDictionaryStringString;
            var json = JsonSerializer.Serialize(values, info);
            var attribute = new TagHelperAttribute("hx-vals", new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes);

            output.Attributes.SetAttribute(attribute);
        }

        return Task.CompletedTask;
    }
}
