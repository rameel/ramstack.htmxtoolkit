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
    public void Request_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.Request, "true"));
        Assert.That(headers.Request, Is.True);
    }

    [Test]
    public void RequestType_ReturnsHeaderValue()
    {
        var headers = CreateHeaders((HtmxRequestHeaderNames.RequestType, "partial"));
        Assert.That(headers.RequestType, Is.EqualTo("partial"));
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
        Assert.That(headers.RequestType, Is.Null);
        Assert.That(headers.Source, Is.Null);
        Assert.That(headers.Target, Is.Null);
        Assert.That(headers.TriggerName, Is.Null);
        Assert.That(headers.Trigger, Is.Null);
    }

    private static HtmxRequestHeaders CreateHeaders(params (string Name, string Value)[] headers) =>
        TestHelper.CreateHttpContext(headers).Request.GetHtmxHeaders();
}
