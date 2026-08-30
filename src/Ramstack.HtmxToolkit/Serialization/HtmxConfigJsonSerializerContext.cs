using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Represents source-generated JSON serialization metadata for HTMX configuration data.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(HtmxV1Config))]
[JsonSerializable(typeof(HtmxV2Config))]
[JsonSerializable(typeof(HtmxV4Config))]
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
