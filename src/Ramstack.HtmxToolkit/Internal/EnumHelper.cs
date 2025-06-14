namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides extension methods for converting and parsing HTMX-related enum values.
/// </summary>
internal static class EnumHelper
{
    /// <summary>
    /// Converts a <see cref="HtmxBinaryType"/> value to its corresponding WebSocket binary type string.
    /// </summary>
    /// <param name="value">The <see cref="HtmxBinaryType"/> value.</param>
    /// <returns>
    /// The string representation: either "blob" or "arraybuffer".
    /// </returns>
    public static string GetWsBinaryTypeValue(this HtmxBinaryType value) =>
        value == HtmxBinaryType.Blob ? "blob" : "arraybuffer";

    /// <summary>
    /// Converts a <see cref="HtmxScrollBehavior"/> value to its corresponding string value used in HTML scroll behavior.
    /// </summary>
    /// <param name="value">The <see cref="HtmxScrollBehavior"/> value.</param>
    /// <returns>
    /// The string representation: either "auto" or "smooth".
    /// </returns>
    public static string GetScrollBehaviorValue(this HtmxScrollBehavior value) =>
        value == HtmxScrollBehavior.Auto ? "auto" : "smooth";

    /// <summary>
    /// Converts a <see cref="HtmxSwap"/> value to its corresponding string representation,
    /// or returns <see langword="null" /> if the value is <see langword="null" />.
    /// </summary>
    /// <param name="value">The nullable <see cref="HtmxSwap"/> value.</param>
    /// <returns>
    /// The string representation of the value,
    /// or <see langword="null" /> if the value is <see langword="null" />.
    /// </returns>
    public static string? GetSwapValue(this HtmxSwap? value) =>
        value?.GetSwapValue();

    /// <summary>
    /// Converts a <see cref="HtmxSwap"/> value to its corresponding string representation used in HTMX attributes.
    /// </summary>
    /// <param name="value">The <see cref="HtmxSwap"/> value.</param>
    /// <returns>
    /// The corresponding string, such as "innerHTML", "beforebegin", etc.
    /// Defaults to "none" if the value is unrecognized.
    /// </returns>
    public static string GetSwapValue(this HtmxSwap value)
    {
        switch (value)
        {
            case HtmxSwap.InnerHtml: return "innerHTML";
            case HtmxSwap.OuterHtml: return "outerHTML";
            case HtmxSwap.BeforeBegin: return "beforebegin";
            case HtmxSwap.AfterBegin: return "afterbegin";
            case HtmxSwap.BeforeEnd: return "beforeend";
            case HtmxSwap.AfterEnd: return "afterend";
            case HtmxSwap.Delete: return "delete";
            default: return "none";
        }
    }

    /// <summary>
    /// Parses a string into a <see cref="HtmxSwap"/> value.
    /// </summary>
    /// <param name="expression">The string to parse.</param>
    /// <returns>
    /// The parsed <see cref="HtmxSwap"/> value if successful;
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
