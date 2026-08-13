namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class EnumHelperTests
{
    [TestCase(HtmxSwap.InnerHtml, "innerHTML")]
    [TestCase(HtmxSwap.OuterHtml, "outerHTML")]
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

    [TestCase(HttpVerb.Get)]
    [TestCase(HttpVerb.Head)]
    [TestCase(HttpVerb.Post)]
    [TestCase(HttpVerb.Put)]
    [TestCase(HttpVerb.Delete)]
    [TestCase(HttpVerb.Connect)]
    [TestCase(HttpVerb.Options)]
    [TestCase(HttpVerb.Trace)]
    [TestCase(HttpVerb.Patch)]
    public void GetHttpVerbValue_ReturnsExpectedString(HttpVerb value) =>
        Assert.That(value.GetHttpVerbValue(), Is.EqualTo(value.ToString().ToLower()));

    [TestCase(HtmxBinaryType.Blob)]
    [TestCase(HtmxBinaryType.ArrayBuffer)]
    public void GetWsBinaryTypeValue_ReturnsExpectedString(HtmxBinaryType value) =>
        Assert.That(value.GetWsBinaryTypeValue(), Is.EqualTo(value.ToString().ToLower()));

    [TestCase(HtmxScrollBehavior.Auto)]
    [TestCase(HtmxScrollBehavior.Smooth)]
    [TestCase(HtmxScrollBehavior.Instant)]
    public void GetScrollBehaviorValue_ReturnsExpectedString(HtmxScrollBehavior value) =>
        Assert.That(value.GetScrollBehaviorValue(), Is.EqualTo(value.ToString().ToLower()));
}
