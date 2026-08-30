using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.TagHelpers;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Provides source-generated JSON serialization metadata for HTMX request configuration data.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(HtmxRequestTagHelper.HtmxRequestDataPrior))]
[JsonSerializable(typeof(HtmxRequestTagHelper.HtmxRequestDataV4))]
internal partial class HtmxRequestJsonSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// Initializes the default serializer context with HTML-safe Unicode encoding.
    /// </summary>
    static HtmxRequestJsonSerializerContext()
    {
        JsonOptions.ConfigureHtmlSafeUnicode(s_defaultOptions);
        s_defaultContext = new HtmxRequestJsonSerializerContext(s_defaultOptions);
    }
}
