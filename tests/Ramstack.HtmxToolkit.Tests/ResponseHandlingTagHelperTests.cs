namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class ResponseHandlingTagHelperTests
{
    [Test]
    public async Task ProcessAsync_AddsEntryToConfig()
    {
        var config = new HtmxConfigTagHelper(null!);
        var items = new Dictionary<object, object>
        {
            [typeof(HtmxConfigTagHelper)] = config
        };

        var entry = new ResponseHandlingTagHelper
        {
            Code = "4[0-9]{2}",
            Swap = false,
            Error = true,
            IgnoreTitle = true,
            Select = "#content",
            Target = "#target",
            SwapOverride = "innerHTML"
        };

        var output = TestHelper.CreateTagHelperOutput("response-handling");
        await entry.ProcessAsync(
            TestHelper.CreateTagHelperContext("response-handling", null, items),
            output);

        Assert.That(config.ResponseHandling!.Count, Is.EqualTo(1));

        var actual = config.ResponseHandling![0];
        Assert.That(actual.Code, Is.EqualTo("4[0-9]{2}"));
        Assert.That(actual.Swap, Is.False);
        Assert.That(actual.Error, Is.True);
        Assert.That(actual.IgnoreTitle, Is.True);
        Assert.That(actual.Select, Is.EqualTo("#content"));
        Assert.That(actual.Target, Is.EqualTo("#target"));
        Assert.That(actual.SwapOverride, Is.EqualTo("innerHTML"));
    }

    [Test]
    public async Task ProcessAsync_SuppressesOutput()
    {
        var config = new HtmxConfigTagHelper(null!);
        var items = new Dictionary<object, object>
        {
            [typeof(HtmxConfigTagHelper)] = config
        };

        var entry = new ResponseHandlingTagHelper();
        var output = TestHelper.CreateTagHelperOutput("response-handling");

        await entry.ProcessAsync(
            TestHelper.CreateTagHelperContext("response-handling", null, items),
            output);

        Assert.That(output.TagName, Is.Null);
        Assert.That(output.IsContentModified, Is.True);
    }

    [Test]
    public void ProcessAsync_WithoutConfig_Throws()
    {
        var entry = new ResponseHandlingTagHelper();
        var output = TestHelper.CreateTagHelperOutput("response-handling");

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await entry.ProcessAsync(
                TestHelper.CreateTagHelperContext("response-handling"),
                output));
    }
}
