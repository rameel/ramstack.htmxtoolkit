using Microsoft.AspNetCore.Http;

namespace Ramstack.HtmxToolkit.Internal;

internal static class DebugHelpers
{
    public static KeyValuePair<string, string>[] GetRequestHeaders(IHeaderDictionary headers)
    {
        return GetHeaders(headers,
            HtmxRequestHeaderNames.Boosted,
            HtmxRequestHeaderNames.CurrentUrl,
            HtmxRequestHeaderNames.HistoryRestoreRequest,
            HtmxRequestHeaderNames.Prompt,
            HtmxRequestHeaderNames.Request,
            HtmxRequestHeaderNames.Target,
            HtmxRequestHeaderNames.TriggerName,
            HtmxRequestHeaderNames.Trigger);
    }

    public static KeyValuePair<string, string>[] GetResponseHeaders(IHeaderDictionary headers)
    {
        return GetHeaders(headers,
            HtmxResponseHeaderNames.Location,
            HtmxResponseHeaderNames.PushUrl,
            HtmxResponseHeaderNames.Redirect,
            HtmxResponseHeaderNames.Refresh,
            HtmxResponseHeaderNames.ReplaceUrl,
            HtmxResponseHeaderNames.Reswap,
            HtmxResponseHeaderNames.Retarget,
            HtmxResponseHeaderNames.Reselect,
            HtmxResponseHeaderNames.Trigger,
            HtmxResponseHeaderNames.TriggerAfterSettle,
            HtmxResponseHeaderNames.TriggerAfterSwap);
    }

    private static KeyValuePair<string, string>[] GetHeaders(IHeaderDictionary headers, params string[] properties)
    {
        return properties
            .Where(headers.ContainsKey)
            .Select(name =>
                KeyValuePair.Create(name, headers[name].ToString()))
            .ToArray();
    }
}
