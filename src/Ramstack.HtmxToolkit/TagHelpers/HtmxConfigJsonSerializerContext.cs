using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents source-generated JSON serialization metadata for HTMX configuration data.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(HtmxV1Options))]
[JsonSerializable(typeof(HtmxV2Options))]
[JsonSerializable(typeof(HtmxV4Options))]
internal partial class HtmxConfigJsonSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// Initializes the default serializer context with HTML-safe Unicode encoding.
    /// </summary>
    static HtmxConfigJsonSerializerContext()
    {
        JsonOptions.ConfigureHtmlSafeUnicode(s_defaultOptions);
        s_defaultContext = new HtmxConfigJsonSerializerContext(s_defaultOptions);
    }
}
