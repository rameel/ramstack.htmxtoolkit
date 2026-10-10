using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests.Internal;

[TestFixture]
public class EnumHelperTests
{
    [TestCase(HtmxSwap.InnerHtml, "innerHTML")]
    [TestCase(HtmxSwap.OuterHtml, "outerHTML")]
    [TestCase(HtmxSwap.InnerMorph, "innerMorph")]
    [TestCase(HtmxSwap.OuterMorph, "outerMorph")]
    [TestCase(HtmxSwap.OuterSync, "outerSync")]
    [TestCase(HtmxSwap.TextContent, "textContent")]
    [TestCase(HtmxSwap.BeforeBegin, "beforebegin")]
    [TestCase(HtmxSwap.AfterBegin, "afterbegin")]
    [TestCase(HtmxSwap.BeforeEnd, "beforeend")]
    [TestCase(HtmxSwap.AfterEnd, "afterend")]
    [TestCase(HtmxSwap.Delete, "delete")]
    [TestCase(HtmxSwap.None, "none")]
    public void GetSwapValue_ReturnsExpectedString(HtmxSwap value, string expected) =>
        Assert.That(value.GetSwapValue(), Is.EqualTo(expected));

    [Test]
    public void GetSwapValue_ReturnsNull_ForNullValue()
    {
        HtmxSwap? value = null;
        Assert.That(value.GetSwapValue(), Is.Null);
    }

    [TestCase("innerHTML", HtmxSwap.InnerHtml)]
    [TestCase("outerHTML", HtmxSwap.OuterHtml)]
    [TestCase("innerMorph", HtmxSwap.InnerMorph)]
    [TestCase("outerMorph", HtmxSwap.OuterMorph)]
    [TestCase("outerSync", HtmxSwap.OuterSync)]
    [TestCase("textContent", HtmxSwap.TextContent)]
    [TestCase("beforebegin", HtmxSwap.BeforeBegin)]
    [TestCase("afterbegin", HtmxSwap.AfterBegin)]
    [TestCase("beforeend", HtmxSwap.BeforeEnd)]
    [TestCase("afterend", HtmxSwap.AfterEnd)]
    [TestCase("delete", HtmxSwap.Delete)]
    [TestCase("none", HtmxSwap.None)]
    public void ParseHtmxSwap_ParsesValue(string expression, HtmxSwap expected) =>
        Assert.That(EnumHelper.ParseHtmxSwap(expression), Is.EqualTo(expected));

    [TestCase("innerHTML show:#content", HtmxSwap.InnerHtml)]
    [TestCase("outerHTML\tsettle:200ms", HtmxSwap.OuterHtml)]
    [TestCase("beforeend\n  swap:1s", HtmxSwap.BeforeEnd)]
    public void ParseHtmxSwap_IgnoresModifiers(string expression, HtmxSwap expected) =>
        Assert.That(EnumHelper.ParseHtmxSwap(expression), Is.EqualTo(expected));

    [TestCase(" outerHTML", HtmxSwap.OuterHtml)]
    [TestCase("\t outerHTML show:top", HtmxSwap.OuterHtml)]
    public void ParseHtmxSwap_IgnoresLeadingWhitespace(string expression, HtmxSwap expected) =>
        Assert.That(EnumHelper.ParseHtmxSwap(expression), Is.EqualTo(expected));

    [TestCase("OuterHTML")]
    [TestCase("outerhtml")]
    [TestCase("InnerHtml")]
    [TestCase("BEFOREEND")]
    public void ParseHtmxSwap_IsCaseSensitive(string expression) =>
        Assert.That(EnumHelper.ParseHtmxSwap(expression), Is.Null);

    [TestCase("bogus")]
    [TestCase("1")]
    [TestCase("42")]
    [TestCase("-1")]
    [TestCase("innerHTML,outerHTML")]
    [TestCase("before")]
    [TestCase("append")]
    public void ParseHtmxSwap_ReturnsNull_ForUnknownValue(string expression) =>
        Assert.That(EnumHelper.ParseHtmxSwap(expression), Is.Null);

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("show:top")]
    [TestCase("swap:1s settle:200ms")]
    public void ParseHtmxSwap_ReturnsNull_WithoutStyle(string? expression) =>
        Assert.That(EnumHelper.ParseHtmxSwap(expression), Is.Null);

    [TestCase(HttpVerb.Get, "get")]
    [TestCase(HttpVerb.Head, "head")]
    [TestCase(HttpVerb.Post, "post")]
    [TestCase(HttpVerb.Put, "put")]
    [TestCase(HttpVerb.Delete, "delete")]
    [TestCase(HttpVerb.Connect, "connect")]
    [TestCase(HttpVerb.Options, "options")]
    [TestCase(HttpVerb.Trace, "trace")]
    [TestCase(HttpVerb.Patch, "patch")]
    public void GetHttpVerbValue_ReturnsExpectedString(HttpVerb value, string expected) =>
        Assert.That(value.GetHttpVerbValue(), Is.EqualTo(expected));

    [TestCase(HtmxBinaryType.Blob, "blob")]
    [TestCase(HtmxBinaryType.ArrayBuffer, "arraybuffer")]
    public void GetWsBinaryTypeValue_ReturnsExpectedString(HtmxBinaryType value, string expected) =>
        Assert.That(value.GetWsBinaryTypeValue(), Is.EqualTo(expected));

    [TestCase(HtmxScrollBehavior.Auto, "auto")]
    [TestCase(HtmxScrollBehavior.Smooth, "smooth")]
    [TestCase(HtmxScrollBehavior.Instant, "instant")]
    public void GetScrollBehaviorValue_ReturnsExpectedString(HtmxScrollBehavior value, string expected) =>
        Assert.That(value.GetScrollBehaviorValue(), Is.EqualTo(expected));

    [TestCase(HtmxFetchMode.SameOrigin, "same-origin")]
    [TestCase(HtmxFetchMode.Cors, "cors")]
    [TestCase(HtmxFetchMode.NoCors, "no-cors")]
    public void GetFetchModeValue_ReturnsExpectedString(HtmxFetchMode value, string expected) =>
        Assert.That(value.GetFetchModeValue(), Is.EqualTo(expected));
}
