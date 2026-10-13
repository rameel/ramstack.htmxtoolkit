using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Applies an <c>hx-vals</c> attribute assembled from <c>hx-val-*</c> attributes
/// and the <c>hx-all-vals</c> dictionary.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>HTMX 1.x and 2.x merge-inherit <c>hx-vals</c> from parent elements.</item>
///   <item>
///     HTMX 4.x requires either the explicit inheritance modifier or the global
///     <see cref="HtmxV4Config.ImplicitInheritance" /> option for inheritance.
///   </item>
///   <item>In HTMX 4.x, the <c>append</c> modifier merges a child declaration into inherited values.</item>
/// </list>
/// </remarks>
[HtmlTargetElement(Attributes = InheritedAttributeName)]
[HtmlTargetElement(Attributes = AppendAttributeName)]
[HtmlTargetElement(Attributes = ValuesDictionaryName)]
[HtmlTargetElement(Attributes = ValuesPrefix + "*")]
public sealed class HtmxValsTagHelper(IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string InheritedAttributeName = "hx-vals-inherited";
    private const string AppendAttributeName = "hx-vals-append";
    private const string ValuesPrefix = "hx-val-";
    private const string ValuesDictionaryName = "hx-all-vals";

    /// <summary>
    /// Gets or sets a value indicating whether <c>hx-vals</c> is explicitly inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   HTMX 1.x and 2.x merge-inherit values without a modifier,
    ///   so the property does not change the generated attribute name for them.
    /// </para>
    /// <para>
    ///   For HTMX 4.x, the tag helper emits the <c>inherited</c> modifier
    ///   when this property is <see langword="true" />.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(InheritedAttributeName)]
    public bool Inherited { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <c>hx-vals</c> is appended to inherited values.
    /// </summary>
    /// <remarks>
    /// <para>The property does not change the generated attribute name for HTMX 1.x and 2.x.</para>
    /// <para>
    ///   For HTMX 4.x, the tag helper emits the <c>append</c> modifier
    ///   when this property is <see langword="true" />.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(AppendAttributeName)]
    public bool Append { get; set; }

    /// <summary>
    /// Gets or sets the <c>hx-vals</c> attribute values, which can also be set
    /// with <c>hx-val-*</c> attributes.
    /// </summary>
    /// <remarks>
    /// The default dictionary compares value names case-sensitively.
    /// </remarks>
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
            var name = options.Value.TargetVersion switch
            {
                HtmxTargetVersion.V4 when Inherited && Append => "hx-vals:inherited:append",
                HtmxTargetVersion.V4 when Inherited => "hx-vals:inherited",
                HtmxTargetVersion.V4 when Append => "hx-vals:append",
                _ => "hx-vals"
            };

            var info = HtmxDictionaryJsonSerializerContext.Default.IDictionaryStringString;
            var json = JsonSerializer.Serialize(values, info);
            var attribute = new TagHelperAttribute(name, new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes);

            output.Attributes.SetAttribute(attribute);
        }

        return Task.CompletedTask;
    }
}
