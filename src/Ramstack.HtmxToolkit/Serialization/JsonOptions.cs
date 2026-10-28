using System.Text.Encodings.Web;
using System.Text.Json;
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
    /// The shared JSON serializer options with the <see cref="JsonSerializerDefaults.Web" /> defaults,
    /// which ASP.NET Core uses for JSON responses: property names are camel-cased, while dictionary keys
    /// and <see langword="null" /> values are written as is.
    /// </summary>
    /// <remarks>
    /// The options use the default encoder, which escapes non-ASCII characters,
    /// so that the resulting JSON can be written into a response header.
    /// </remarks>
    public static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

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
