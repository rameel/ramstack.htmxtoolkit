using System.Text.Json;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.TagHelpers
{
    /// <summary>
    /// Represents a <see cref="TagHelper"/> implementation
    /// that targets elements to generate <c>hx-headers</c> attribute.
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
        /// Gets or sets the the <c>hx-header</c> attribute values.
        /// </summary>
        [HtmlAttributeName(HeadersDictionaryName, DictionaryAttributePrefix = HeadersPrefix)]
        public Dictionary<string, string> Headers { get; set; } = new();

        /// <inheritdoc />
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var headers = new HtmlString(
                JsonSerializer.Serialize(Headers, JsonOptions.PreserveKeyCase));

            output.Attributes.SetAttribute(
                new TagHelperAttribute("hx-headers", headers, HtmlAttributeValueStyle.SingleQuotes));

            return Task.CompletedTask;
        }
    }
}
