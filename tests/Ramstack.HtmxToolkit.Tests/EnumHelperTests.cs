namespace Ramstack.HtmxToolkit.Tests;

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

    [Test]
    public void ParseHtmxSwap_IgnoresModifiers() =>
        Assert.That(EnumHelper.ParseHtmxSwap("innerHTML show:#content"), Is.EqualTo(HtmxSwap.InnerHtml));

    [Test]
    public void ParseHtmxSwap_IsCaseInsensitive() =>
        Assert.That(EnumHelper.ParseHtmxSwap("OuterHTML"), Is.EqualTo(HtmxSwap.OuterHtml));

    [Test]
    public void ParseHtmxSwap_ReturnsNull_ForUnknownValue() =>
        Assert.That(EnumHelper.ParseHtmxSwap("bogus"), Is.Null);

    [Test]
    public void ParseHtmxSwap_ReturnsNull_ForNullExpression() =>
        Assert.That(EnumHelper.ParseHtmxSwap(null), Is.Null);

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

    [Test]
    public void GetSwapValue_ReturnsNone_ForUndefinedValue() =>
        Assert.That(((HtmxSwap)999).GetSwapValue(), Is.EqualTo("none"));

    [Test]
    public void GetHttpVerbValue_ReturnsPatch_ForUndefinedValue() =>
        Assert.That(((HttpVerb)999).GetHttpVerbValue(), Is.EqualTo("patch"));

    [Test]
    public void GetScrollBehaviorValue_ReturnsInstant_ForUndefinedValue() =>
        Assert.That(((HtmxScrollBehavior)999).GetScrollBehaviorValue(), Is.EqualTo("instant"));

    [Test]
    public void GetWsBinaryTypeValue_ReturnsArrayBuffer_ForUndefinedValue() =>
        Assert.That(((HtmxBinaryType)999).GetWsBinaryTypeValue(), Is.EqualTo("arraybuffer"));

    [Test]
    public void GetFetchModeValue_ReturnsNoCors_ForUndefinedValue() =>
        Assert.That(((HtmxFetchMode)999).GetFetchModeValue(), Is.EqualTo("no-cors"));
}
