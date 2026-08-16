namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxHeaderTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesHeaders()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxHeaderTagHelper
        {
            Headers = new Dictionary<string, string>
            {
                ["X-Requested-With"] = "XMLHttpRequest",
                ["X-Custom"] = "value"
            }
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json["X-Requested-With"].GetString(), Is.EqualTo("XMLHttpRequest"));
        Assert.That(json["X-Custom"].GetString(), Is.EqualTo("value"));
    }

    [Test]
    public async Task ProcessAsync_SerializesEmptyDictionary()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxHeaderTagHelper
        {
            Headers = new Dictionary<string, string>()
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        var attribute = output.Attributes["hx-headers"];
        Assert.That(attribute, Is.Null);
    }
}
