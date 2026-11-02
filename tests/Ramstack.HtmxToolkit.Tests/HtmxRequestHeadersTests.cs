namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxRequestHeadersTests
{
    [Test]
    public void Boosted_IsTrue_WhenHeaderIsTrue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Boosted, "true"));
        Assert.That(headers.Boosted, Is.True);
    }

    [Test]
    public void Boosted_IsFalse_WhenHeaderIsFalse()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Boosted, "false"));
        Assert.That(headers.Boosted, Is.False);
    }

    [Test]
    public void Boosted_IsFalse_WhenHeaderAbsent()
    {
        var headers = CreateHeaders();
        Assert.That(headers.Boosted, Is.False);
    }

    [Test]
    public void CurrentUrl_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.CurrentUrl, "https://example.com/"));
        Assert.That(headers.CurrentUrl, Is.EqualTo("https://example.com/"));
    }

    [Test]
    public void HistoryRestoreRequest_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.HistoryRestoreRequest, "true"));
        Assert.That(headers.HistoryRestoreRequest, Is.True);
    }

    [Test]
    public void Prompt_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Prompt, "yes"));
        Assert.That(headers.Prompt, Is.EqualTo("yes"));
    }

    [Test]
    public void StringProperties_AreDecoded_WhenHeaderIsAutoEncoded()
    {
        var headers = CreateHeaders(
            (HtmxRequestHeaderNames.Prompt, "%D0%94%D0%B0%20100%2525"),
            (HtmxRequestHeaderNames.Prompt + "-URI-AutoEncoded", "true"),
            (HtmxRequestHeaderNames.Target, "%D1%86%D0%B5%D0%BB%D1%8C"),
            (HtmxRequestHeaderNames.Target + "-URI-AutoEncoded", "true"),
            (HtmxRequestHeaderNames.Trigger, "%D1%84%D0%BE%D1%80%D0%BC%D0%B0"),
            (HtmxRequestHeaderNames.Trigger + "-URI-AutoEncoded", "true"),
            (HtmxRequestHeaderNames.TriggerName, "%D0%B8%D0%BC%D1%8F"),
            (HtmxRequestHeaderNames.TriggerName + "-URI-AutoEncoded", "true"));

        Assert.That(headers.Prompt, Is.EqualTo("Да 100%25"));
        Assert.That(headers.Target, Is.EqualTo("цель"));
        Assert.That(headers.Trigger, Is.EqualTo("форма"));
        Assert.That(headers.TriggerName, Is.EqualTo("имя"));
    }

    [Test]
    public void Prompt_IsNotDecoded_WithoutAutoEncodedHeader()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Prompt, "100%25"));
        Assert.That(headers.Prompt, Is.EqualTo("100%25"));
    }

    [Test]
    public void Prompt_IsNotDecoded_WhenAutoEncodedHeaderIsNotTrue()
    {
        var headers = CreateHeaders(
            (HtmxRequestHeaderNames.Prompt, "100%25"),
            (HtmxRequestHeaderNames.Prompt + "-URI-AutoEncoded", "false"));

        Assert.That(headers.Prompt, Is.EqualTo("100%25"));
    }

    [Test]
    public void Prompt_IsNotDecoded_WhenOtherHeaderIsAutoEncoded()
    {
        var headers = CreateHeaders(
            (HtmxRequestHeaderNames.Prompt, "100%25"),
            (HtmxRequestHeaderNames.Trigger + "-URI-AutoEncoded", "true"));

        Assert.That(headers.Prompt, Is.EqualTo("100%25"));
    }

    [Test]
    public void Request_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Request, "true"));
        Assert.That(headers.Request, Is.True);
    }

    [TestCase("full", HtmxRequestType.Full)]
    [TestCase("partial", HtmxRequestType.Partial)]
    [TestCase("", HtmxRequestType.Unspecified)]
    [TestCase("unknown", HtmxRequestType.Unspecified)]
    public void RequestType_ReturnsHeaderValue(string value, HtmxRequestType expected)
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.RequestType, value));
        Assert.That(headers.RequestType, Is.EqualTo(expected));
    }

    [Test]
    public void RequestType_IsUnspecified_WhenHeaderAbsent()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Boosted, "true"));
        Assert.That(headers.RequestType, Is.EqualTo(HtmxRequestType.Unspecified));
    }

    [TestCase(null, HtmxRequestType.Full)]
    [TestCase("full", HtmxRequestType.Full)]
    [TestCase("partial", HtmxRequestType.Partial)]
    [TestCase("unknown", HtmxRequestType.Unspecified)]
    public void RequestType_IsFull_ForHistoryRestoreRequestWithoutHeader(string? value, HtmxRequestType expected)
    {
        var headers = value is null
            ? CreateHeaders((HtmxRequestHeaderNames.HistoryRestoreRequest, "true"))
            : CreateHeaders((HtmxRequestHeaderNames.HistoryRestoreRequest, "true"), (HtmxRequestHeaderNames.RequestType, value));

        Assert.That(headers.RequestType, Is.EqualTo(expected));
    }

    [Test]
    public void RequestType_IsUnspecified_WhenHistoryRestoreRequestIsFalse()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.HistoryRestoreRequest, "false"));
        Assert.That(headers.RequestType, Is.EqualTo(HtmxRequestType.Unspecified));
    }

    [Test]
    public void Source_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Source, "button#submit"));
        Assert.That(headers.Source, Is.EqualTo("button#submit"));
    }

    [Test]
    public void Target_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Target, "content"));
        Assert.That(headers.Target, Is.EqualTo("content"));
    }

    [Test]
    public void TriggerName_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.TriggerName, "btn"));
        Assert.That(headers.TriggerName, Is.EqualTo("btn"));
    }

    [Test]
    public void Trigger_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Trigger, "btn-id"));
        Assert.That(headers.Trigger, Is.EqualTo("btn-id"));
    }

    [Test]
    public void StringProperties_ReturnNull_WhenHeaderAbsent()
    {
        var headers = CreateHeaders();

        Assert.That(headers.CurrentUrl, Is.Null);
        Assert.That(headers.Prompt, Is.Null);
        Assert.That(headers.Source, Is.Null);
        Assert.That(headers.Target, Is.Null);
        Assert.That(headers.TriggerName, Is.Null);
        Assert.That(headers.Trigger, Is.Null);
    }

    private static HtmxRequestHeaders CreateHeaders(params (string Name, string Value)[] headers) =>
        TestHelper.CreateHttpContext(headers).Request.GetHtmxHeaders();
}
