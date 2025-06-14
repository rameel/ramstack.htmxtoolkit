using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.AspNetCore.Mvc.Filters;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Identifies an action that sets htmx response headers.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class HtmxResponseAttribute : Attribute, IResultFilter
{
    private readonly List<(string Key, string Value)> _headers = [];

    /// <summary>
    /// Gets or sets the <c>HX-Refresh</c> header to perform a full page refresh.
    /// </summary>
    public bool Refresh
    {
        get => GetValue(HtmxResponseHeaderNames.Refresh) == "true";
        set => SetValue(HtmxResponseHeaderNames.Refresh, value ? "true" : "");
    }

    /// <summary>
    /// Gets or sets the <c>HX-Reswap</c> header to specify how the response will be swapped into the DOM.
    /// </summary>
    public HtmxSwap Reswap
    {
        get
        {
            var value = GetValue(HtmxResponseHeaderNames.Reswap);
            return EnumHelper.ParseHtmxSwap(value).GetValueOrDefault();
        }
        set => SetValue(HtmxResponseHeaderNames.Reswap, value.GetSwapValue());
    }

    /// <summary>
    /// Gets or sets the <c>HX-Reswap</c> header to specify how the response will be swapped into the DOM.
    /// </summary>
    [MaybeNull]
    public string ReswapExpression
    {
        get => GetValue(HtmxResponseHeaderNames.Reswap);
        set => SetValue(HtmxResponseHeaderNames.Reswap, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Retarget</c> header that specifies a selector to change
    /// the target of the content update to a different element on the page.
    /// </summary>
    [MaybeNull]
    public string Retarget
    {
        get => GetValue(HtmxResponseHeaderNames.Retarget);
        set => SetValue(HtmxResponseHeaderNames.Retarget, value);
    }

    /// <summary>
    /// Gets or sets the <c>HX-Reselect</c> header that specifies a selector
    /// to choose which part of the response content will be swapped in.
    /// </summary>
    [MaybeNull]
    public string Reselect
    {
        get => GetValue(HtmxResponseHeaderNames.Reselect);
        set => SetValue(HtmxResponseHeaderNames.Reselect, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a special HTTP status code should be set to stop polling.
    /// </summary>
    public bool StopPolling { get; set; }

    /// <inheritdoc />
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.HttpContext.Request.IsHtmxRequest())
        {
            var response = context.HttpContext.Response;
            if (StopPolling)
                response.StatusCode = HtmxResponse.StopPollingStatusCode;

            var headers = response.Headers;
            foreach (ref var kvp in CollectionsMarshal.AsSpan(_headers))
                headers[kvp.Key] = kvp.Value;
        }
    }

    /// <inheritdoc />
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    private string? GetValue(string key)
    {
        foreach (ref var kvp in CollectionsMarshal.AsSpan(_headers))
            if (kvp.Key == key)
                return kvp.Value;

        return null;
    }

    private void SetValue(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _headers.Add((key, value));
    }
}
