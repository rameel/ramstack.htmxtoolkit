using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides helper methods for debugger views.
/// </summary>
internal static class DebugHelpers
{
    /// <summary>
    /// Returns all headers whose names start with <c>"HX-"</c>,
    /// using a case-insensitive comparison.
    /// </summary>
    /// <param name="headers">The header dictionary to inspect.</param>
    /// <returns>
    /// An array containing the matching header names and values.
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
