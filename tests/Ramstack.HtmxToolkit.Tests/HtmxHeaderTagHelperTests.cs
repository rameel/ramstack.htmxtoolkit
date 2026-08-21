using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
            Headers =
            {
                ["X-Requested-With"] = "XMLHttpRequest",
                ["X-Custom"] = "value's"
            }
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers"];

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.TypeOf<HtmlString>());
        Assert.That(attribute.Value.ToString(), Is.EqualTo("{\"X-Requested-With\":\"XMLHttpRequest\",\"X-Custom\":\"value\\u0027s\"}"));

        var json = JsonHelper.ParseJson(attribute.Value.ToString()!);
        Assert.That(json["X-Requested-With"].GetString(), Is.EqualTo("XMLHttpRequest"));
        Assert.That(json["X-Custom"].GetString(), Is.EqualTo("value's"));
    }

    [Test]
    public void Headers_TreatsNamesAsCaseInsensitive()
    {
        var helper = new HtmxHeaderTagHelper
        {
            Headers =
            {
                ["X-Custom"] = "first",
                ["x-custom"] = "second"
            }
        };

        Assert.That(helper.Headers, Has.Count.EqualTo(1));
        Assert.That(helper.Headers["X-CUSTOM"], Is.EqualTo("second"));
    }

    [Test]
    public async Task ProcessAsync_UsesSingleQuotesForJsonAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxHeaderTagHelper
        {
            Headers =
            {
                ["X-Custom"] = "value"
            }
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-headers"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
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
