namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides extension methods for converting and parsing HTMX-related enum values.
/// </summary>
internal static class EnumHelper
{
    /// <summary>
    /// Converts a <see cref="HtmxBinaryType" /> value
    /// to the corresponding WebSocket binary type string.
    /// </summary>
    /// <param name="value">The <see cref="HtmxBinaryType" /> value.</param>
    /// <returns>
    /// <c>blob</c> or <c>arraybuffer</c>.
    /// </returns>
    public static string GetWsBinaryTypeValue(this HtmxBinaryType value) =>
        value == HtmxBinaryType.Blob ? "blob" : "arraybuffer";

    /// <summary>
    /// Converts a <see cref="HtmxScrollBehavior" /> value
    /// to the corresponding scrolling behavior string.
    /// </summary>
    /// <param name="value">The <see cref="HtmxScrollBehavior" /> value.</param>
    /// <returns>
    /// <c>auto</c>, <c>smooth</c>, or <c>instant</c>.
    /// </returns>
    public static string GetScrollBehaviorValue(this HtmxScrollBehavior value)
    {
        return value switch
        {
            HtmxScrollBehavior.Auto => "auto",
            HtmxScrollBehavior.Smooth => "smooth",
            _ => "instant"
        };
    }

    /// <summary>
    /// Converts a <see cref="HtmxSwap" /> value to its corresponding string representation,
    /// or returns <see langword="null" /> if the value is <see langword="null" />.
    /// </summary>
    /// <param name="value">The nullable <see cref="HtmxSwap" /> value.</param>
    /// <returns>
    /// The string representation of the value,
    /// or <see langword="null" /> if the value is <see langword="null" />.
    /// </returns>
    public static string? GetSwapValue(this HtmxSwap? value) =>
        value?.GetSwapValue();

    /// <summary>
    /// Converts a <see cref="HtmxSwap" /> value to the corresponding string
    /// representation used in HTMX attributes.
    /// </summary>
    /// <param name="value">The <see cref="HtmxSwap" /> value.</param>
    /// <returns>
    /// The corresponding HTMX swap value, or <c>none</c> if
    /// <paramref name="value" /> is not recognized.
    /// </returns>
    public static string GetSwapValue(this HtmxSwap value)
    {
        return value switch
        {
            HtmxSwap.InnerHtml => "innerHTML",
            HtmxSwap.OuterHtml => "outerHTML",
            HtmxSwap.InnerMorph => "innerMorph",
            HtmxSwap.OuterMorph => "outerMorph",
            HtmxSwap.OuterSync => "outerSync",
            HtmxSwap.TextContent => "textContent",
            HtmxSwap.BeforeBegin => "beforebegin",
            HtmxSwap.AfterBegin => "afterbegin",
            HtmxSwap.BeforeEnd => "beforeend",
            HtmxSwap.AfterEnd => "afterend",
            HtmxSwap.Delete => "delete",
            _ => "none"
        };
    }

    /// <summary>
    /// Converts a <see cref="HttpVerb" /> value to its corresponding lowercase HTTP method string.
    /// </summary>
    /// <param name="value">The <see cref="HttpVerb" /> value.</param>
    /// <returns>
    /// The lowercase HTTP method name, such as <c>get</c>, <c>post</c>, or <c>delete</c>.
    /// </returns>
    public static string GetHttpVerbValue(this HttpVerb value)
    {
        return value switch
        {
            HttpVerb.Get => "get",
            HttpVerb.Head => "head",
            HttpVerb.Post => "post",
            HttpVerb.Put => "put",
            HttpVerb.Delete => "delete",
            HttpVerb.Connect => "connect",
            HttpVerb.Options => "options",
            HttpVerb.Trace => "trace",
            _ => "patch"
        };
    }

    /// <summary>
    /// Converts a <see cref="HtmxFetchMode" /> value
    /// to the corresponding Fetch API request mode string.
    /// </summary>
    /// <param name="value">The <see cref="HtmxFetchMode" /> value.</param>
    /// <returns>
    /// <c>same-origin</c>, <c>cors</c>, or <c>no-cors</c>.
    /// </returns>
    public static string GetFetchModeValue(this HtmxFetchMode value)
    {
        return value switch
        {
            HtmxFetchMode.SameOrigin => "same-origin",
            HtmxFetchMode.Cors => "cors",
            _ => "no-cors"
        };
    }

    /// <summary>
    /// Parses a string into a <see cref="HtmxFetchMode" /> value.
    /// </summary>
    /// <param name="expression">The string to parse.</param>
    /// <returns>
    /// The parsed <see cref="HtmxFetchMode" /> value if successful;
    /// otherwise, <see langword="null" />.
    /// </returns>
    public static HtmxFetchMode? ParseHtmxFetchMode(string? expression)
    {
        return expression switch
        {
            "same-origin" => HtmxFetchMode.SameOrigin,
            "cors" => HtmxFetchMode.Cors,
            "no-cors" => HtmxFetchMode.NoCors,
            _ => null
        };
    }

    /// <summary>
    /// Parses a string into a <see cref="HtmxSwap" /> value.
    /// </summary>
    /// <param name="expression">The string to parse.</param>
    /// <returns>
    /// The parsed <see cref="HtmxSwap" /> value if successful;
    /// otherwise, <see langword="null" />.
    /// </returns>
    public static HtmxSwap? ParseHtmxSwap(string? expression)
    {
        expression ??= "";

        var index = expression.IndexOf(' ');
        if (index < 0)
            index = expression.Length;

        var s = expression.AsSpan(0, index);
        if (Enum.TryParse<HtmxSwap>(s, ignoreCase: true, out var value))
            return value;

        return null;
    }
}
