using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Provides source-generated JSON serialization metadata for <c>HX-Location</c> options.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(HtmxLocationOptions))]
internal partial class HtmxLocationOptionsJsonSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// Initializes the default serializer context with HTML-safe Unicode encoding.
    /// </summary>
    static HtmxLocationOptionsJsonSerializerContext()
    {
        JsonOptions.ConfigureHtmlSafeUnicode(s_defaultOptions);
        s_defaultContext = new HtmxLocationOptionsJsonSerializerContext(s_defaultOptions);
    }
}
