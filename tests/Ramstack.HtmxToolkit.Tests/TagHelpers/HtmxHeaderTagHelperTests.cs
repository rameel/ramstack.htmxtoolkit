using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests.TagHelpers;

[TestFixture]
public class HtmxHeaderTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesHeaders()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper();
        helper.Headers["X-Requested-With"] = "XMLHttpRequest";
        helper.Headers["X-Custom"] = "value's";

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
        var helper = CreateHelper();
        helper.Headers["X-Custom"] = "first";
        helper.Headers["x-custom"] = "second";

        Assert.That(helper.Headers, Has.Count.EqualTo(1));
        Assert.That(helper.Headers["X-CUSTOM"], Is.EqualTo("second"));
    }

    [Test]
    public async Task ProcessAsync_UsesSingleQuotesForJsonAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper();
        helper.Headers["X-Custom"] = "value";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-headers"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
    }

    [Test]
    public async Task ProcessAsync_SerializesEmptyDictionary()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper();

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        var attribute = output.Attributes["hx-headers"];
        Assert.That(attribute, Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_Inherited_UsesInheritedAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);

        helper.Inherited = true;
        helper.Headers["X-Custom"] = "value";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers:inherited"];

        Assert.That(attribute.Value.ToString(), Is.EqualTo("{\"X-Custom\":\"value\"}"));
        Assert.That(output.Attributes["hx-headers"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_Inherited_UsesConfiguredMetaCharacter()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4, "-");

        helper.Inherited = true;
        helper.Headers["X-Custom"] = "value";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers-inherited"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"X-Custom\":\"value\"}"));
        Assert.That(output.Attributes["hx-headers:inherited"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_Htmx4_NotInherited_UsesOrdinaryAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);
        helper.Headers["X-Custom"] = "value";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"X-Custom\":\"value\"}"));
        Assert.That(output.Attributes["hx-headers:inherited"], Is.Null);
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
        helper.Headers["X-Custom"] = "value";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-headers"];

        Assert.That(attribute!.Value.ToString(), Is.EqualTo("{\"X-Custom\":\"value\"}"));
        Assert.That(output.Attributes["hx-headers:inherited"], Is.Null);
        Assert.That(output.Attributes["hx-inherit"], Is.Null);
    }

    private static HtmxHeaderTagHelper CreateHelper(HtmxTargetVersion version = HtmxTargetVersion.V2, string? metachar = null)
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

        return new HtmxHeaderTagHelper(Options.Create(options));
    }
}
