using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.Tests.TagHelpers;

[TestFixture]
public class HtmxValsTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesValues()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxValsTagHelper
        {
            Values =
            {
                ["категория"] = "Детские книги '<script>\" &",
                ["sort"] = "title"
            }
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-vals"];

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.TypeOf<HtmlString>());

        var serialized = attribute.Value.ToString()!;

        Assert.That(serialized, Is.EqualTo("{\"категория\":\"Детские книги \\u0027\\u003Cscript\\u003E\\u0022 \\u0026\",\"sort\":\"title\"}"));
        Assert.That(serialized, Does.Not.Contain("'"));
        Assert.That(serialized, Does.Not.Contain("<"));
        Assert.That(serialized, Does.Not.Contain(">"));
        Assert.That(serialized, Does.Not.Contain("&"));

        var json = JsonHelper.ParseJson(serialized);
        Assert.That(json["категория"].GetString(), Is.EqualTo("Детские книги '<script>\" &"));
        Assert.That(json["sort"].GetString(), Is.EqualTo("title"));
    }

    [Test]
    public async Task ProcessAsync_UsesSingleQuotesForJsonAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxValsTagHelper
        {
            Values =
            {
                ["sort"] = "title"
            }
        };

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-vals"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
    }

    [Test]
    public async Task ProcessAsync_OmitsEmptyDictionary()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxValsTagHelper();

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-vals"], Is.Null);
    }
}
