using System.Text.Json;

namespace Ramstack.HtmxToolkit.Tests;

/// <summary>
/// Represents a helper class providing methods for JSON manipulation in test scenarios.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Deserializes a JSON object into a dictionary for key/value assertions.
    /// </summary>
    /// <param name="json">The JSON object to deserialize.</param>
    /// <returns>
    /// The deserialized key/value pairs.
    /// </returns>
    public static Dictionary<string, JsonElement> ParseJson(string json) =>
        JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;
}
