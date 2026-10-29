using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Internal;
using Ramstack.HtmxToolkit.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Applies an <c>hx-headers</c> attribute assembled from <c>hx-header-*</c> attributes
/// and the <c>hx-all-headers</c> dictionary.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>HTMX 1.9.x and 2.x merge-inherit <c>hx-headers</c> from parent elements.</item>
///   <item>
///     HTMX 4.x requires either the explicit inheritance modifier or the global
///     <see cref="HtmxV4Config.ImplicitInheritance" /> option for inheritance.
///   </item>
///   <item>In HTMX 4.x, the <c>append</c> modifier merges a child declaration into inherited headers.</item>
/// </list>
/// <para>
///   An <see cref="InvalidOperationException" /> is thrown when the element also has an <c>hx-headers</c>
///   attribute, with or without HTMX 4.x modifiers, rather than replacing it.
/// </para>
/// </remarks>
[HtmlTargetElement(Attributes = InheritedAttributeName)]
[HtmlTargetElement(Attributes = AppendAttributeName)]
[HtmlTargetElement(Attributes = HeadersDictionaryName)]
[HtmlTargetElement(Attributes = HeadersPrefix + "*")]
public sealed class HtmxHeaderTagHelper(IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string InheritedAttributeName = "hx-headers-inherited";
    private const string AppendAttributeName = "hx-headers-append";
    private const string HeadersPrefix = "hx-header-";
    private const string HeadersDictionaryName = "hx-all-headers";

    /// <summary>
    /// Gets or sets a value indicating whether <c>hx-headers</c> is explicitly inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   HTMX 1.9.x and 2.x merge-inherit headers without a modifier,
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
    /// Gets or sets a value indicating whether <c>hx-headers</c> is appended to inherited headers.
    /// </summary>
    /// <remarks>
    /// <para>The property does not change the generated attribute name for HTMX 1.9.x and 2.x.</para>
    /// <para>
    ///   For HTMX 4.x, the tag helper emits the <c>append</c> modifier
    ///   when this property is <see langword="true" />.
    /// </para>
    /// </remarks>
    [HtmlAttributeName(AppendAttributeName)]
    public bool Append { get; set; }

    /// <summary>
    /// Gets or sets the <c>hx-headers</c> attribute values, which can also be set
    /// with <c>hx-header-*</c> attributes.
    /// </summary>
    /// <remarks>
    /// The default dictionary compares header names case-insensitively.
    /// </remarks>
    [HtmlAttributeName(HeadersDictionaryName, DictionaryAttributePrefix = HeadersPrefix)]
    public IDictionary<string, string> Headers
    {
        get => field ??= new SmallDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        set;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        const string SourceAttributes = $"{HeadersPrefix}*, {HeadersDictionaryName}, {InheritedAttributeName}, or {AppendAttributeName}";
        output.ThrowIfAttributeDeclared("hx-headers", SourceAttributes);

        if (Headers is { Count: > 0 })
        {
            var name = options.Value.TargetVersion switch
            {
                HtmxTargetVersion.V4 when Inherited && Append => "hx-headers:inherited:append",
                HtmxTargetVersion.V4 when Inherited => "hx-headers:inherited",
                HtmxTargetVersion.V4 when Append => "hx-headers:append",
                _ => "hx-headers"
            };

            var info = HtmxDictionaryJsonSerializerContext.Default.IDictionaryStringString;
            var json = JsonSerializer.Serialize(Headers, info);

            output.Attributes.SetAttribute(
                new TagHelperAttribute(name, new HtmlString(json), HtmlAttributeValueStyle.SingleQuotes));
        }

        return Task.CompletedTask;
    }
}
