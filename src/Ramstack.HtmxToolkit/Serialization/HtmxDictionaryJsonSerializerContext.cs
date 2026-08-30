using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Provides source-generated JSON serialization metadata for HTMX string dictionaries.
/// </summary>
[JsonSerializable(typeof(IDictionary<string, string>))]
[JsonSourceGenerationOptions(WriteIndented = false, GenerationMode = JsonSourceGenerationMode.Default)]
internal partial class HtmxDictionaryJsonSerializerContext : JsonSerializerContext
{
    /// <summary>
    /// Initializes the default serializer context with HTML-safe Unicode encoding.
    /// </summary>
    static HtmxDictionaryJsonSerializerContext()
    {
        JsonOptions.ConfigureHtmlSafeUnicode(s_defaultOptions);
        s_defaultContext = new HtmxDictionaryJsonSerializerContext(s_defaultOptions);
    }
}
