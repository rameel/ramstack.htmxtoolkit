using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Provides source-generated JSON serialization metadata for <c>HX-Location</c> options.
/// </summary>
/// <remarks>
/// The context uses the default encoder, which escapes non-ASCII characters,
/// because response headers allow only ASCII characters.
/// </remarks>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(HtmxLocationOptionsJsonView))]
internal partial class HtmxLocationOptionsJsonSerializerContext : JsonSerializerContext;
