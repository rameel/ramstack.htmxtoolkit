using Microsoft.AspNetCore.Html;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxRequestTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesRequestConfiguration()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxRequestTagHelper
        {
            Timeout = 500,
            Credentials = true,
            NoHeaders = false
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-request"];

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.TypeOf<HtmlString>());

        var json = JsonHelper.ParseJson(attribute.Value.ToString()!);
        Assert.That(json["timeout"].GetInt32(), Is.EqualTo(500));
        Assert.That(json["credentials"].GetBoolean(), Is.True);
        Assert.That(json["noHeaders"].GetBoolean(), Is.False);
    }

    [Test]
    public async Task ProcessAsync_OmitsUnsetProperties()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxRequestTagHelper
        {
            Timeout = 500
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-request"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);

        Assert.That(json["timeout"].GetInt32(), Is.EqualTo(500));
        Assert.That(json.ContainsKey("credentials"), Is.False);
        Assert.That(json.ContainsKey("noHeaders"), Is.False);
    }

    [Test]
    public async Task ProcessAsync_OmitsUnsetConfiguration()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxRequestTagHelper();

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-request"], Is.Null);
    }
}
