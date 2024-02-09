namespace Ramstack.HtmxToolkit.Internal;

internal static class EnumHelper
{
    public static string GetWsBinaryTypeValue(this HtmxBinaryType value) =>
        value == HtmxBinaryType.Blob ? "blob" : "arraybuffer";

    public static string GetScrollBehaviorValue(this HtmxScrollBehavior value) =>
        value == HtmxScrollBehavior.Auto ? "auto" : "smooth";

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

    public static HtmxSwap? ParseHtmxSwap(string? expression)
    {
        expression ??= "";

        var index = expression.IndexOf(' ');
        if (index < 0)
            index = expression.Length;

        Enum.TryParse<HtmxSwap>(expression.AsSpan(0, index), ignoreCase: true, out var value);
        return value;
    }
}
