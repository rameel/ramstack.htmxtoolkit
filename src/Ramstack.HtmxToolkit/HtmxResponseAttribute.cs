using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.AspNetCore.Mvc.Filters;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies HTMX response headers to apply when an action result is executed for an HTMX request.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class HtmxResponseAttribute : Attribute, IResultFilter
{
    private readonly List<(string Key, string Value)> _headers = [];

    /// <summary>
    /// Gets or sets a value indicating whether the <c>HX-Refresh</c> header is set
    /// to request a full-page refresh.
    /// </summary>
    public bool Refresh
    {
        get => GetValue(HtmxResponseHeaderNames.Refresh) == "true";
        set => SetValue(HtmxResponseHeaderNames.Refresh, value ? "true" : "");
    }

    /// <summary>
    /// Gets or sets the swap style to specify in the <c>HX-Reswap</c> header.
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
    /// Gets or sets the complete <c>HX-Reswap</c> header value, including any swap modifiers.
    /// </summary>
    [MaybeNull]
    public string ReswapExpression
    {
        get => GetValue(HtmxResponseHeaderNames.Reswap);
        set => SetValue(HtmxResponseHeaderNames.Reswap, value);
    }

    /// <summary>
    /// Gets or sets the CSS selector to specify in the <c>HX-Retarget</c> header.
    /// </summary>
    [MaybeNull]
    public string Retarget
    {
        get => GetValue(HtmxResponseHeaderNames.Retarget);
        set => SetValue(HtmxResponseHeaderNames.Retarget, value);
    }

    /// <summary>
    /// Gets or sets the CSS selector to specify in the <c>HX-Reselect</c> header.
    /// </summary>
    [MaybeNull]
    public string Reselect
    {
        get => GetValue(HtmxResponseHeaderNames.Reselect);
        set => SetValue(HtmxResponseHeaderNames.Reselect, value);
    }

    /// <inheritdoc />
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.HttpContext.Request.IsHtmxRequest())
        {
            var response = context.HttpContext.Response;
            var headers = response.Headers;
            foreach (ref var kvp in CollectionsMarshal.AsSpan(_headers))
                headers[kvp.Key] = kvp.Value;
        }
    }

    /// <inheritdoc />
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    /// <summary>
    /// Gets the pending value of the specified response header.
    /// </summary>
    /// <param name="key">The name of the header.</param>
    /// <returns>
    /// The header value, or <see langword="null" /> if no value has been configured.
    /// </returns>
    private string? GetValue(string key)
    {
        foreach (ref var kvp in CollectionsMarshal.AsSpan(_headers))
            if (kvp.Key == key)
                return kvp.Value;

        return null;
    }

    /// <summary>
    /// Adds a nonempty response header value to the pending headers.
    /// </summary>
    /// <param name="key">The name of the header.</param>
    /// <param name="value">The header value.</param>
    private void SetValue(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _headers.Add((key, value));
    }
}
