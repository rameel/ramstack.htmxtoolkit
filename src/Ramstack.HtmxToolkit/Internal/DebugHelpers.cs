using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides helper methods for debugging purposes.
/// </summary>
internal static class DebugHelpers
{
    /// <summary>
    /// Extracts all headers that start with <c>"HX-"</c> (case-insensitive) from the given header collection.
    /// </summary>
    /// <param name="headers">The header dictionary to inspect.</param>
    /// <returns>
    /// An array of key-value pairs representing the headers that start with <c>"HX-"</c>.
    /// </returns>
    public static KeyValuePair<string, string>[] GetHeaders(IHeaderDictionary headers)
    {
        var list = new List<KeyValuePair<string, string>>();
        foreach (var (name, value) in headers)
            if (name.StartsWith("HX-", StringComparison.OrdinalIgnoreCase))
                list.Add(KeyValuePair.Create(name, value.ToString()));

        return [..list];
    }
}
