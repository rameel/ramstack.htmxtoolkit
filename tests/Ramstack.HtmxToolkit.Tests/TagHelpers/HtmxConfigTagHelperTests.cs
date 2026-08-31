using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests.TagHelpers;

[TestFixture]
public class HtmxConfigTagHelperTests
{
    [Test]
    public async Task ProcessAsync_RendersMetaTag()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(new HtmxToolkitOptions());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.Multiple(() =>
        {
            Assert.That(output.TagName, Is.EqualTo("meta"));
            Assert.That(output.TagMode, Is.EqualTo(TagMode.SelfClosing));
            Assert.That(output.Attributes["name"]!.Value, Is.EqualTo("htmx-config"));
        });
    }

    [Test]
    public async Task ProcessAsync_RemovesHtmxConfigAttribute_FromMetaTag()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("htmx-config")
        };

        var output = TestHelper.CreateTagHelperOutput("meta", attributes);
        var helper = CreateHelper(new HtmxToolkitOptions());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext("meta", attributes), output);

        Assert.That(output.Attributes["htmx-config"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_WithDefaults_SerializesEmptyHtmx2Object()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(new HtmxToolkitOptions());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(GetContent(output), Is.EqualTo("{}"));
    }

    [Test]
    public async Task ProcessAsync_UsesSingleQuotesForJsonAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = CreateHelper(new HtmxToolkitOptions());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["content"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
    }

    [Test]
    public async Task ProcessAsync_SerializesEmptyMethodsThatUseUrlParams_AsEmptyArray()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV2(htmx => htmx.MethodsThatUseUrlParams = []);

        var json = await RenderJson(options);

        Assert.That(json["methodsThatUseUrlParams"].GetRawText(), Is.EqualTo("[]"));
    }

    [Test]
    public async Task ProcessAsync_SerializesFullResponseHandlingConfig()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV2(htmx => htmx.ResponseHandling =
        [
            new ResponseHandlingConfig
            {
                Code = "404",
                Swap = false,
                Error = true,
                IgnoreTitle = true,
                Select = "#content",
                Target = "#target",
                SwapOverride = "innerHTML"
            }
        ]);

        var json = await RenderJson(options);

        var entry = json["responseHandling"].EnumerateArray().Single();
        Assert.Multiple(() =>
        {
            Assert.That(entry.GetProperty("code").GetString(), Is.EqualTo("404"));
            Assert.That(entry.GetProperty("swap").GetBoolean(), Is.False);
            Assert.That(entry.GetProperty("error").GetBoolean(), Is.True);
            Assert.That(entry.GetProperty("ignoreTitle").GetBoolean(), Is.True);
            Assert.That(entry.GetProperty("select").GetString(), Is.EqualTo("#content"));
            Assert.That(entry.GetProperty("target").GetString(), Is.EqualTo("#target"));
            Assert.That(entry.GetProperty("swapOverride").GetString(), Is.EqualTo("innerHTML"));
        });
    }

    [Test]
    public async Task ProcessAsync_SerializesOnlyHtmx1Options()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV1(htmx =>
        {
            htmx.HistoryEnabled = false;
            htmx.HistoryCacheSize = 42;
            htmx.RefreshOnHistoryMiss = true;
            htmx.DefaultSwapStyle = HtmxSwap.OuterHtml;
            htmx.DefaultSwapDelay = 250;
            htmx.DefaultSettleDelay = 300;
            htmx.IncludeIndicatorStyles = false;
            htmx.IndicatorClass = "indicator";
            htmx.RequestClass = "request";
            htmx.AddedClass = "added";
            htmx.SwappingClass = "swapping";
            htmx.SettlingClass = "settling";
            htmx.AllowEval = false;
            htmx.AllowScriptTags = false;
            htmx.InlineScriptNonce = "script-nonce";
            htmx.AttributesToSettle = ["class", "style"];
            htmx.UseTemplateFragments = true;
            htmx.WsReconnectDelay = "exponential";
            htmx.WsBinaryType = HtmxBinaryType.ArrayBuffer;
            htmx.DisableSelector = "[data-disable]";
            htmx.WithCredentials = true;
            htmx.Timeout = 5000;
            htmx.SelfRequestsOnly = false;
            htmx.ScrollBehavior = HtmxScrollBehavior.Smooth;
            htmx.DefaultFocusScroll = true;
            htmx.GetCacheBusterParam = true;
            htmx.GlobalViewTransitions = true;
            htmx.MethodsThatUseUrlParams = [HttpVerb.Get, HttpVerb.Delete];
            htmx.IgnoreTitle = true;
            htmx.ScrollIntoViewOnBoost = false;
            htmx.TriggerSpecsCacheEnabled = true;
        });

        var json = await RenderJson(options);

        Assert.That(json.Keys, Is.EquivalentTo(new[]
        {
            "historyEnabled", "historyCacheSize", "refreshOnHistoryMiss", "defaultSwapStyle",
            "defaultSwapDelay", "defaultSettleDelay", "includeIndicatorStyles", "indicatorClass",
            "requestClass", "addedClass", "swappingClass", "settlingClass", "allowEval",
            "allowScriptTags", "inlineScriptNonce", "attributesToSettle", "useTemplateFragments",
            "wsReconnectDelay", "wsBinaryType", "disableSelector", "withCredentials", "timeout",
            "selfRequestsOnly", "scrollBehavior", "defaultFocusScroll", "getCacheBusterParam",
            "globalViewTransitions", "methodsThatUseUrlParams", "ignoreTitle", "scrollIntoViewOnBoost",
            "triggerSpecsCache"
        }));
        Assert.That(json["defaultSwapStyle"].GetString(), Is.EqualTo("outerHTML"));
        Assert.That(json["wsBinaryType"].GetString(), Is.EqualTo("arraybuffer"));
        Assert.That(json["scrollBehavior"].GetString(), Is.EqualTo("smooth"));
        Assert.That(json["methodsThatUseUrlParams"].GetRawText(), Is.EqualTo("[\"get\",\"delete\"]"));
        Assert.That(json["triggerSpecsCache"].GetRawText(), Is.EqualTo("{}"));
    }

    [Test]
    public async Task ProcessAsync_SerializesOnlyHtmx2Options_InResponseHandlingOrder()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV2(htmx =>
        {
            htmx.HistoryEnabled = true;
            htmx.HistoryCacheSize = 42;
            htmx.RefreshOnHistoryMiss = true;
            htmx.DefaultSwapStyle = HtmxSwap.InnerHtml;
            htmx.DefaultSwapDelay = 250;
            htmx.DefaultSettleDelay = 300;
            htmx.IncludeIndicatorStyles = false;
            htmx.IndicatorClass = "indicator";
            htmx.RequestClass = "request";
            htmx.AddedClass = "added";
            htmx.SwappingClass = "swapping";
            htmx.SettlingClass = "settling";
            htmx.AllowEval = false;
            htmx.AllowScriptTags = false;
            htmx.InlineScriptNonce = "script-nonce";
            htmx.InlineStyleNonce = "style-nonce";
            htmx.AttributesToSettle = ["class", "style"];
            htmx.WsReconnectDelay = "exponential";
            htmx.WsBinaryType = HtmxBinaryType.Blob;
            htmx.DisableSelector = "[data-disable]";
            htmx.WithCredentials = true;
            htmx.DisableInheritance = true;
            htmx.Timeout = 5000;
            htmx.SelfRequestsOnly = true;
            htmx.ScrollBehavior = HtmxScrollBehavior.Instant;
            htmx.DefaultFocusScroll = true;
            htmx.GetCacheBusterParam = true;
            htmx.GlobalViewTransitions = true;
            htmx.MethodsThatUseUrlParams = [HttpVerb.Get, HttpVerb.Delete];
            htmx.IgnoreTitle = true;
            htmx.ScrollIntoViewOnBoost = false;
            htmx.TriggerSpecsCacheEnabled = true;
            htmx.ResponseHandling =
            [
                new ResponseHandlingConfig { Code = "204", Swap = false },
                new ResponseHandlingConfig { Code = "[45]..", Swap = false, Error = true }
            ];
            htmx.AllowNestedOobSwaps = true;
            htmx.HistoryRestoreAsHxRequest = false;
            htmx.ReportValidityOfForms = true;
        });

        var json = await RenderJson(options);

        Assert.That(json.Keys, Is.EquivalentTo(new[]
        {
            "historyEnabled", "historyCacheSize", "refreshOnHistoryMiss", "defaultSwapStyle",
            "defaultSwapDelay", "defaultSettleDelay", "includeIndicatorStyles", "indicatorClass",
            "requestClass", "addedClass", "swappingClass", "settlingClass", "allowEval",
            "allowScriptTags", "inlineScriptNonce", "inlineStyleNonce", "attributesToSettle",
            "wsReconnectDelay", "wsBinaryType", "disableSelector", "withCredentials",
            "disableInheritance", "timeout", "selfRequestsOnly", "scrollBehavior", "defaultFocusScroll",
            "getCacheBusterParam", "globalViewTransitions", "methodsThatUseUrlParams", "ignoreTitle",
            "scrollIntoViewOnBoost", "triggerSpecsCache", "responseHandling", "allowNestedOobSwaps",
            "historyRestoreAsHxRequest", "reportValidityOfForms"
        }));
        Assert.That(json["defaultSwapStyle"].GetString(), Is.EqualTo("innerHTML"));
        Assert.That(json["scrollBehavior"].GetString(), Is.EqualTo("instant"));

        var responseHandling = json["responseHandling"].EnumerateArray().ToArray();
        Assert.That(responseHandling[0].GetProperty("code").GetString(), Is.EqualTo("204"));
        Assert.That(responseHandling[1].GetProperty("code").GetString(), Is.EqualTo("[45].."));
    }

    [Test]
    public async Task ProcessAsync_SerializesOnlyHtmx4Options()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV4(htmx =>
        {
            htmx.LogAll = true;
            htmx.Prefix = "data-custom-";
            htmx.MetaCharacter = "-";
            htmx.History = HtmxHistoryMode.Reload;
            htmx.DefaultSwap = HtmxSwap.OuterMorph;
            htmx.AllowEmptySwapAfterOOB = true;
            htmx.DefaultSettleDelay = 1;
            htmx.IncludeIndicatorCss = false;
            htmx.IndicatorClass = "indicator";
            htmx.RequestClass = "request";
            htmx.InlineScriptNonce = "nonce";
            htmx.Extensions = "sse, ws";
            htmx.ImplicitInheritance = false;
            htmx.DefaultTimeout = 5000;
            htmx.Mode = HtmxFetchMode.NoCors;
            htmx.DefaultFocusScroll = true;
            htmx.Transitions = true;
            htmx.MorphIgnore = ["data-htmx-powered"];
            htmx.MorphSkip = "[data-skip]";
            htmx.MorphSkipChildren = "[data-skip-children]";
            htmx.MorphScanLimit = 20;
            htmx.NoSwap = ["204", "4xx"];
        });

        var json = await RenderJson(options);

        Assert.That(json.Keys, Is.EquivalentTo(new[]
        {
            "logAll", "prefix", "metaCharacter", "history", "defaultSwap", "allowEmptySwapAfterOOB",
            "defaultSettleDelay", "includeIndicatorCSS", "indicatorClass", "requestClass",
            "inlineScriptNonce", "extensions", "implicitInheritance", "defaultTimeout", "mode",
            "defaultFocusScroll", "transitions", "morphIgnore", "morphSkip", "morphSkipChildren",
            "morphScanLimit", "noSwap"
        }));
        Assert.That(json["history"].GetString(), Is.EqualTo("reload"));
        Assert.That(json["defaultSwap"].GetString(), Is.EqualTo("outerMorph"));
        Assert.That(json["mode"].GetString(), Is.EqualTo("no-cors"));
        Assert.That(json.ContainsKey("timeout"), Is.False);
        Assert.That(json.ContainsKey("defaultSwapStyle"), Is.False);
    }

    [Test]
    [TestCase(HtmxHistoryMode.Enabled, "true")]
    [TestCase(HtmxHistoryMode.Disabled, "false")]
    [TestCase(HtmxHistoryMode.Reload, "\"reload\"")]
    public async Task ProcessAsync_SerializesHtmx4HistoryMode(HtmxHistoryMode mode, string expected)
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV4(htmx => htmx.History = mode);

        var json = await RenderJson(options);

        Assert.That(json["history"].GetRawText(), Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessAsync_EscapesHtmlSensitiveCharacters()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV2(htmx => htmx.RequestClass = "<a> & \"b\" 'c'");

        var output = TestHelper.CreateTagHelperOutput();
        await CreateHelper(options).ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(
            GetContent(output),
            Is.EqualTo("{\"requestClass\":\"\\u003Ca\\u003E \\u0026 \\u0022b\\u0022 \\u0027c\\u0027\"}"));
    }

    [Test]
    public async Task ProcessAsync_AntiforgeryEnabledByDefault_RendersDataAttributes()
    {
        var antiforgery = new StubAntiforgery();
        var httpContext = new DefaultHttpContext();
        var options = new HtmxToolkitOptions();
        var helper = CreateHelper(options, antiforgery);
        helper.ViewContext = new ViewContext { HttpContext = httpContext };

        var output = TestHelper.CreateTagHelperOutput();
        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.Multiple(() =>
        {
            Assert.That(antiforgery.StoredHttpContext, Is.SameAs(httpContext));
            Assert.That(output.Attributes["data-antiforgery-request-token"]!.Value, Is.EqualTo("request-token"));
            Assert.That(output.Attributes["data-antiforgery-header-name"]!.Value, Is.EqualTo("RequestVerificationToken"));
            Assert.That(output.Attributes["data-antiforgery-form-field-name"]!.Value, Is.EqualTo("__RequestVerificationToken"));
            Assert.That(output.Attributes.Any(attribute => attribute.Name.Contains("cookie", StringComparison.OrdinalIgnoreCase)), Is.False);
            Assert.That(GetContent(output), Is.EqualTo("{}"));
        });
    }

    [Test]
    public async Task ProcessAsync_AntiforgeryDisabled_DoesNotRequestOrRenderTokens()
    {
        var antiforgery = new StubAntiforgery();
        var output = TestHelper.CreateTagHelperOutput();

        await CreateHelper(new HtmxToolkitOptions { IncludeAntiforgeryToken = false }, antiforgery)
            .ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.Multiple(() =>
        {
            Assert.That(antiforgery.StoredHttpContext, Is.Null);
            Assert.That(output.Attributes["data-antiforgery-request-token"], Is.Null);
            Assert.That(output.Attributes["data-antiforgery-header-name"], Is.Null);
            Assert.That(output.Attributes["data-antiforgery-form-field-name"], Is.Null);
        });
    }

    private static async Task<Dictionary<string, System.Text.Json.JsonElement>> RenderJson(HtmxToolkitOptions options)
    {
        var output = TestHelper.CreateTagHelperOutput();
        await CreateHelper(options).ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        Assert.That(output.Attributes["content"]!.Value, Is.TypeOf<HtmlString>());
        return JsonHelper.ParseJson(GetContent(output));
    }

    private static HtmxConfigTagHelper CreateHelper(HtmxToolkitOptions options, IAntiforgery? antiforgery = null) =>
        new(antiforgery ?? new StubAntiforgery(), Options.Create(options))
        {
            ViewContext = new ViewContext { HttpContext = new DefaultHttpContext() }
        };

    private static string GetContent(TagHelperOutput output) =>
        output.Attributes["content"]!.Value!.ToString()!;

    #region Inner type: StubAntiforgery

    private sealed class StubAntiforgery : IAntiforgery
    {
        public HttpContext? StoredHttpContext { get; private set; }

        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            StoredHttpContext = httpContext;
            return new AntiforgeryTokenSet(
                requestToken: "request-token",
                cookieToken: "cookie-token",
                formFieldName: "__RequestVerificationToken",
                headerName: "RequestVerificationToken");
        }

        public AntiforgeryTokenSet GetTokens(HttpContext httpContext) => throw new NotSupportedException();
        public Task<bool> IsRequestValidAsync(HttpContext httpContext) => throw new NotSupportedException();
        public void SetCookieTokenAndHeader(HttpContext httpContext) => throw new NotSupportedException();
        public Task ValidateRequestAsync(HttpContext httpContext) => throw new NotSupportedException();
    }

    #endregion
}
