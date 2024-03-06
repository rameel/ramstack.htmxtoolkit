using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit.Internal;

internal static class DebugHelpers
{
    public static KeyValuePair<string, string>[] GetHeaders(IHeaderDictionary headers)
    {
        var list = new List<KeyValuePair<string, string>>();
        foreach (var (name, value) in headers)
            if (name.StartsWith("HX-", StringComparison.OrdinalIgnoreCase))
                list.Add(KeyValuePair.Create(name, value.ToString()));

        return [..list];
    }
}
