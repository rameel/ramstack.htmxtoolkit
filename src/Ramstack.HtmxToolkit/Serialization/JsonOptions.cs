using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Provides shared options and encoding settings for JSON serialization.
/// </summary>
internal static class JsonOptions
{
    /// <summary>
    /// The shared encoder that preserves characters from all Unicode ranges while escaping
    /// JavaScript- and HTML-sensitive characters.
    /// </summary>
    private static readonly JavaScriptEncoder s_encoder =
        JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All));

    /// <summary>
    /// The shared JSON serializer options that use <see cref="JsonNamingPolicy.CamelCase" />
    /// for property names and dictionary keys and omit properties
    /// whose values are <see langword="null" />.
    /// </summary>
    /// <remarks>
    /// The options use the default encoder, which escapes non-ASCII characters,
    /// so that the resulting JSON can be written into a response header.
    /// </remarks>
    public static readonly JsonSerializerOptions CamelCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Configures serializer options to preserve Unicode characters while escaping
    /// JavaScript- and HTML-sensitive characters.
    /// </summary>
    /// <param name="options">The serializer options to configure.</param>
    /// <remarks>
    /// Use it only for JSON rendered into HTML attributes. Response headers allow
    /// only ASCII characters and must use the default encoder.
    /// </remarks>
    public static void ConfigureHtmlSafeUnicode(JsonSerializerOptions options) =>
        options.Encoder = s_encoder;
}
