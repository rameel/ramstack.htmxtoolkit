using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxRequestTagHelperTests
{
    [Test]
    [TestCase(HtmxTargetVersion.V1)]
    [TestCase(HtmxTargetVersion.V2)]
    public async Task ProcessAsync_SerializesLegacyRequestConfiguration(HtmxTargetVersion version)
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(version);
        helper.Timeout = 500;
        helper.Credentials = HtmxRequestCredentials.Include;
        helper.NoHeaders = false;

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
    [TestCase(HtmxTargetVersion.V1)]
    [TestCase(HtmxTargetVersion.V2)]
    public async Task ProcessAsync_SerializesSameOriginCredentialsAsFalse(HtmxTargetVersion version)
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(version);
        helper.Credentials = HtmxRequestCredentials.SameOrigin;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-request"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json["credentials"].GetBoolean(), Is.False);
    }

    [Test]
    public async Task ProcessAsync_SerializesNoHeadersWhenTrue()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V2);
        helper.NoHeaders = true;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-request"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json["noHeaders"].GetBoolean(), Is.True);
    }

    [Test]
    public async Task ProcessAsync_OmitsUnsetLegacyProperties()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V2);
        helper.Timeout = 500;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-request"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);

        Assert.That(json["timeout"].GetInt32(), Is.EqualTo(500));
        Assert.That(json.ContainsKey("credentials"), Is.False);
        Assert.That(json.ContainsKey("noHeaders"), Is.False);
    }

    [Test]
    public async Task ProcessAsync_SerializesHtmx4RequestConfiguration()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);
        helper.Timeout = 500;
        helper.Credentials = HtmxRequestCredentials.Omit;
        helper.NoHeaders = true;
        helper.Cache = "no-cache";
        helper.Redirect = "manual";
        helper.Referrer = "no-referrer";
        helper.Integrity = "sha384-example";
        helper.Validate = true;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-config"];

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.TypeOf<HtmlString>());

        var json = JsonHelper.ParseJson(attribute.Value.ToString()!);
        Assert.That(json["timeout"].GetInt32(), Is.EqualTo(500));
        Assert.That(json["credentials"].GetString(), Is.EqualTo("omit"));
        Assert.That(json["cache"].GetString(), Is.EqualTo("no-cache"));
        Assert.That(json["redirect"].GetString(), Is.EqualTo("manual"));
        Assert.That(json["referrer"].GetString(), Is.EqualTo("no-referrer"));
        Assert.That(json["integrity"].GetString(), Is.EqualTo("sha384-example"));
        Assert.That(json["validate"].GetBoolean(), Is.True);
        Assert.That(json.ContainsKey("noHeaders"), Is.False);
        Assert.That(output.Attributes["hx-request"], Is.Null);
    }

    [Test]
    [TestCase(HtmxRequestCredentials.SameOrigin, "same-origin")]
    [TestCase(HtmxRequestCredentials.Include, "include")]
    [TestCase(HtmxRequestCredentials.Omit, "omit")]
    public async Task ProcessAsync_SerializesHtmx4CredentialsMode(HtmxRequestCredentials credentials, string expected)
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V4);
        helper.Credentials = credentials;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-config"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json["credentials"].GetString(), Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessAsync_OmitsHtmx4OnlyCredentialsModeForLegacyVersions()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V2);
        helper.Timeout = 500;
        helper.Credentials = HtmxRequestCredentials.Omit;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-request"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json.ContainsKey("credentials"), Is.False);
    }

    [Test]
    public async Task ProcessAsync_UsesSingleQuotesForJsonAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V2);
        helper.Timeout = 500;

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-request"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
    }

    [Test]
    public async Task ProcessAsync_OmitsUnsetConfiguration()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(HtmxTargetVersion.V2);

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-request"], Is.Null);
    }

    private static HtmxRequestTagHelper CreateHelper(HtmxTargetVersion version)
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
                options.UseHtmxV4();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(version));
        }

        return new HtmxRequestTagHelper(Options.Create(options));
    }
}
