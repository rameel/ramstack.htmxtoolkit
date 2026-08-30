using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides preconfigured <see cref="JsonSerializerOptions" /> for JSON serialization.
/// </summary>
internal static class JsonOptions
{
    /// <summary>
    /// Preserves characters from all Unicode ranges while escaping JavaScript-
    /// and HTML-sensitive characters.
    /// </summary>
    private static readonly JavaScriptEncoder s_encoder =
        JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All));

    /// <summary>
    /// Configures serializer options to preserve Unicode characters while escaping
    /// JavaScript- and HTML-sensitive characters.
    /// </summary>
    /// <param name="options">The serializer options to configure.</param>
    public static void ConfigureHtmlSafeUnicode(JsonSerializerOptions options) =>
        options.Encoder = s_encoder;

    /// <summary>
    /// The shared JSON serializer options that use <see cref="JsonNamingPolicy.CamelCase" />
    /// for property names and dictionary keys and omit properties
    /// whose values are <see langword="null" />.
    /// </summary>
    public static readonly JsonSerializerOptions CamelCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}
