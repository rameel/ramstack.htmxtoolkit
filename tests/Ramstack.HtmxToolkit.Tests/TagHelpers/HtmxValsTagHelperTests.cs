using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests.TagHelpers;

[TestFixture]
public class HtmxValsTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesValues()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper();
        helper.Values["категория"] = "Детские книги '<script>\" &";
        helper.Values["sort"] = "title";

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
        var helper = CreateHelper();
        helper.Values["sort"] = "title";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-vals"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
    }

    [Test]
    public async Task ProcessAsync_OmitsEmptyDictionary()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper();

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-vals"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_Inherited_UsesInheritedAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);

        helper.Inherited = true;
        helper.Values["sort"] = "title";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-vals:inherited"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"sort\":\"title\"}"));
        Assert.That(output.Attributes["hx-vals"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_Inherited_UsesConfiguredMetaCharacter()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4, "-");

        helper.Inherited = true;
        helper.Values["sort"] = "title";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-vals-inherited"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"sort\":\"title\"}"));
        Assert.That(output.Attributes["hx-vals:inherited"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_NotInherited_UsesOrdinaryAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);
        helper.Values["sort"] = "title";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-vals"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"sort\":\"title\"}"));
        Assert.That(output.Attributes["hx-vals:inherited"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_Inherited_WithEmptyDictionary_OmitsAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);
        helper.Inherited = true;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes, Is.Empty);
    }

    [TestCase(HtmxTargetVersion.V1)]
    [TestCase(HtmxTargetVersion.V2)]
    public async Task ProcessAsync_Htmx1_Htmx2_Inherited_UsesOrdinaryAttribute(HtmxTargetVersion version)
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(version);

        helper.Inherited = true;
        helper.Values["sort"] = "title";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-vals"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"sort\":\"title\"}"));
        Assert.That(output.Attributes["hx-vals:inherited"], Is.Null);
        Assert.That(output.Attributes["hx-inherit"], Is.Null);
    }

    private static HtmxValsTagHelper CreateHelper(HtmxTargetVersion version = HtmxTargetVersion.V2, string? metachar = null)
    {
        var options = new HtmxToolkitOptions();

        switch (version)
        {
            case HtmxTargetVersion.V1:
                options.UseHtmxV1();
                break;
            case HtmxTargetVersion.V2:
                options.UseHtmxV2();
                break;
            case HtmxTargetVersion.V4:
                options.UseHtmxV4(config => config.MetaCharacter = metachar);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(version));
        }

        return new HtmxValsTagHelper(Options.Create(options));
    }
}
