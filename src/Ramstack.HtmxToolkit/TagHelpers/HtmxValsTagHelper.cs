using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Applies an <c>hx-vals</c> attribute assembled from strongly typed tag helper attributes.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>HTMX 1.x and 2.x merge-inherit <c>hx-vals</c> from parent elements.</item>
///   <item>
///     HTMX 4.x requires either the explicit inheritance modifier or the global
///     <see cref="HtmxV4Config.ImplicitInheritance" /> option for inheritance.
///   </item>
///   <item>A child declaration of a value overrides a parent declaration.</item>
/// </list>
/// </remarks>
[HtmlTargetElement(Attributes = InheritedAttributeName)]
[HtmlTargetElement(Attributes = ValuesDictionaryName)]
[HtmlTargetElement(Attributes = ValuesPrefix + "*")]
public sealed class HtmxValsTagHelper(IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string InheritedAttributeName = "hx-vals-inherited";
    private const string ValuesPrefix = "hx-val-";
    private const string ValuesDictionaryName = "hx-all-vals";

    /// <summary>
    /// Gets or sets a value indicating whether <c>hx-vals</c> is explicitly inherited.
    /// </summary>
    /// <remarks>
    /// HTMX 1.x and 2.x merge-inherit values without a modifier.
    /// HTMX 4.x emits the <c>inherited</c> modifier when this property is <see langword="true" />.
    /// </remarks>
    [HtmlAttributeName(InheritedAttributeName)]
    public bool Inherited { get; set; }

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
            var name = "hx-vals";
            if (Inherited && options.Value.TargetVersion == HtmxTargetVersion.V4)
                if (options.Value.HtmxConfig is HtmxV4Config config)
                    name = string.IsNullOrEmpty(config.MetaCharacter)
                        ? "hx-vals:inherited"
                        : $"hx-vals{config.MetaCharacter}inherited";

            var info = HtmxDictionaryJsonSerializerContext.Default.IDictionaryStringString;
            var json = JsonSerializer.Serialize(values, info);
            var attribute = new TagHelperAttribute(name, new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes);

            output.Attributes.SetAttribute(attribute);
        }

        return Task.CompletedTask;
    }
}
