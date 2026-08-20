namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxHeaderTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesHeaders()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxHeaderTagHelper();
        helper.Headers["X-Requested-With"] = "XMLHttpRequest";
        helper.Headers["X-Custom"] = "value";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json["X-Requested-With"].GetString(), Is.EqualTo("XMLHttpRequest"));
        Assert.That(json["X-Custom"].GetString(), Is.EqualTo("value"));
    }

    [Test]
    public void Headers_TreatsNamesAsCaseInsensitive()
    {
        var helper = new HtmxHeaderTagHelper();
        helper.Headers["X-Custom"] = "first";
        helper.Headers["x-custom"] = "second";

        Assert.That(helper.Headers, Has.Count.EqualTo(1));
        Assert.That(helper.Headers["X-CUSTOM"], Is.EqualTo("second"));
    }

    [Test]
    public async Task ProcessAsync_SerializesEmptyDictionary()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxHeaderTagHelper();

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        var attribute = output.Attributes["hx-headers"];
        Assert.That(attribute, Is.Null);
    }
}
