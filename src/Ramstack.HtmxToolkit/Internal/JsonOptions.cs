using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides preconfigured <see cref="JsonSerializerOptions"/> for JSON serialization.
/// </summary>
internal static class JsonOptions
{
    /// <summary>
    /// JSON serializer options using <see cref="JsonNamingPolicy.CamelCase"/> for property names and dictionary keys,
    /// and ignoring properties with <see langword="null"/> values.
    /// </summary>
    public static readonly JsonSerializerOptions CamelCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// JSON serializer options that preserve original property and key casing
    /// while ignoring properties with <see langword="null"/> values.
    /// </summary>
    public static readonly JsonSerializerOptions PreserveKeyCase = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
}
