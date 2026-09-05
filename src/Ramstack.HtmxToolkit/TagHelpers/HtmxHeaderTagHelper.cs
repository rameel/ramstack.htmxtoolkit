using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Applies an <c>hx-headers</c> attribute assembled from strongly typed tag helper attributes.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>HTMX 1.x and 2.x merge-inherit <c>hx-headers</c> from parent elements.</item>
///   <item>
///     HTMX 4.x requires either the explicit inheritance modifier or the global
///     <see cref="HtmxV4Config.ImplicitInheritance" /> option for inheritance.
///   </item>
///   <item>A child declaration of a header overrides a parent declaration.</item>
/// </list>
/// </remarks>
[HtmlTargetElement(Attributes = InheritedAttributeName)]
[HtmlTargetElement(Attributes = HeadersDictionaryName)]
[HtmlTargetElement(Attributes = HeadersPrefix + "*")]
public sealed class HtmxHeaderTagHelper(IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string InheritedAttributeName = "hx-headers-inherited";
    private const string HeadersPrefix = "hx-header-";
    private const string HeadersDictionaryName = "hx-all-headers";

    /// <summary>
    /// Gets or sets a value indicating whether <c>hx-headers</c> is explicitly inherited.
    /// </summary>
    /// <remarks>
    /// <para>HTMX 1.x and 2.x merge-inherit headers without a modifier.</para>
    /// <para>HTMX 4.x emits the <c>inherited</c> modifier when this property is <see langword="true" />.</para>
    /// </remarks>
    [HtmlAttributeName(InheritedAttributeName)]
    public bool Inherited { get; set; }

    /// <summary>
    /// Gets or sets the <c>hx-headers</c> attribute values.
    /// </summary>
    [HtmlAttributeName(HeadersDictionaryName, DictionaryAttributePrefix = HeadersPrefix)]
    public IDictionary<string, string> Headers
    {
        get => field ??= new SmallDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        set;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Headers is { Count: > 0 })
        {
            var name = "hx-headers";
            if (Inherited && options.Value.TargetVersion == HtmxTargetVersion.V4)
            {
                if (options.Value.HtmxConfig is HtmxV4Config config)
                    name = string.IsNullOrEmpty(config.MetaCharacter)
                        ? "hx-headers:inherited"
                        : $"hx-headers{config.MetaCharacter}inherited";
            }

            var info = HtmxDictionaryJsonSerializerContext.Default.IDictionaryStringString;
            var json = JsonSerializer.Serialize(Headers, info);

            output.Attributes.SetAttribute(
                new TagHelperAttribute(name, new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes));
        }

        return Task.CompletedTask;
    }
}
