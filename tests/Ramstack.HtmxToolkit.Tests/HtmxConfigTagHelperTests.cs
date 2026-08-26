using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxConfigTagHelperTests
{
    [Test]
    public async Task ProcessAsync_RendersMetaTag()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxConfigTagHelper(new StubAntiforgery());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.TagName, Is.EqualTo("meta"));
        Assert.That(output.TagMode, Is.EqualTo(TagMode.SelfClosing));
        Assert.That(output.Attributes["name"]!.Value, Is.EqualTo("htmx-config"));
    }

    [Test]
    public async Task ProcessAsync_RemovesHtmxConfigAttribute_FromMetaTag()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("htmx-config")
        };

        var output = TestHelper.CreateTagHelperOutput("meta", attributes);
        var helper = new HtmxConfigTagHelper(new StubAntiforgery());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext("meta", attributes), output);

        Assert.That(output.Attributes["htmx-config"], Is.Null);
    }

    [Test]
    public async Task ProcessAsync_WithDefaults_SerializesEmptyObject()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxConfigTagHelper(new StubAntiforgery());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(GetContent(output), Is.EqualTo("{}"));
    }

    [Test]
    public async Task ProcessAsync_UsesSingleQuotesForJsonAttribute()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxConfigTagHelper(new StubAntiforgery());

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["content"]!.ValueStyle, Is.EqualTo(HtmlAttributeValueStyle.SingleQuotes));
    }

    [Test]
    public async Task ProcessAsync_SerializesConfiguredOptions()
    {
        var helper = new HtmxConfigTagHelper(new StubAntiforgery())
        {
            LogAll = true,
            Prefix = "data-custom-",
            MetaCharacter = "-",
            History = HtmxHistoryMode.Disabled,
            HistoryCacheSize = 42,
            RefreshOnHistoryMiss = true,
            DefaultSwapStyle = HtmxSwap.OuterHtml,
            DefaultSwapEmpty = true,
            DefaultSwapDelay = 250,
            DefaultSettleDelay = 300,
            IncludeIndicatorStyles = false,
            IndicatorClass = "индикатор's",
            RequestClass = "my-request",
            AddedClass = "my-added",
            SwappingClass = "my-swapping",
            SettlingClass = "my-settling",
            AllowEval = false,
            AllowScriptTags = false,
            InlineScriptNonce = "nonce",
            Extensions = "sse, ws",
            InlineStyleNonce = "style-nonce",
            AttributesToSettle = ["class", "style"],
            UseTemplateFragments = true,
            WsReconnectDelay = "exponential",
            WsBinaryType = HtmxBinaryType.ArrayBuffer,
            DisableSelector = "[data-disable]",
            WithCredentials = true,
            DisableInheritance = true,
            DefaultTimeout = 5000,
            Mode = HtmxFetchMode.Cors,
            ScrollBehavior = HtmxScrollBehavior.Instant,
            DefaultFocusScroll = true,
            GetCacheBusterParam = true,
            GlobalViewTransitions = true,
            MorphIgnore = ["data-htmx-powered", "data-preserve"],
            MorphSkip = "[data-skip]",
            MorphSkipChildren = "[data-skip-children]",
            MorphScanLimit = 20,
            NoSwap = ["204", "4xx"],
            MethodsThatUseUrlParams = [HttpVerb.Get, HttpVerb.Delete],
            IgnoreTitle = true,
            ScrollIntoViewOnBoost = false,
            TriggerSpecsCacheEnabled = true,
            AllowNestedOobSwaps = true,
            HistoryRestoreAsHxRequest = false,
            ReportValidityOfForms = true
        };

        var output = TestHelper.CreateTagHelperOutput();
        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["content"]!.Value, Is.TypeOf<HtmlString>());

        var content = GetContent(output);
        Assert.That(content, Does.Contain("\"индикатор\\u0027s\""));
        Assert.That(content, Does.Not.Contain("'"));

        var json = JsonHelper.ParseJson(content);

        Assert.That(json["logAll"].GetBoolean(), Is.True);
        Assert.That(json["prefix"].GetString(), Is.EqualTo("data-custom-"));
        Assert.That(json["metaCharacter"].GetString(), Is.EqualTo("-"));
        Assert.That(json["historyEnabled"].GetBoolean(), Is.False);
        Assert.That(json["history"].GetBoolean(), Is.False);
        Assert.That(json["historyCacheSize"].GetInt32(), Is.EqualTo(42));
        Assert.That(json["refreshOnHistoryMiss"].GetBoolean(), Is.True);
        Assert.That(json["defaultSwapStyle"].GetString(), Is.EqualTo("outerHTML"));
        Assert.That(json["defaultSwap"].GetString(), Is.EqualTo("outerHTML"));
        Assert.That(json["defaultSwapEmpty"].GetBoolean(), Is.True);
        Assert.That(json["defaultSwapDelay"].GetInt32(), Is.EqualTo(250));
        Assert.That(json["defaultSettleDelay"].GetInt32(), Is.EqualTo(300));
        Assert.That(json["includeIndicatorStyles"].GetBoolean(), Is.False);
        Assert.That(json["indicatorClass"].GetString(), Is.EqualTo("индикатор's"));
        Assert.That(json["includeIndicatorCSS"].GetBoolean(), Is.False);
        Assert.That(json["requestClass"].GetString(), Is.EqualTo("my-request"));
        Assert.That(json["addedClass"].GetString(), Is.EqualTo("my-added"));
        Assert.That(json["swappingClass"].GetString(), Is.EqualTo("my-swapping"));
        Assert.That(json["settlingClass"].GetString(), Is.EqualTo("my-settling"));
        Assert.That(json["allowEval"].GetBoolean(), Is.False);
        Assert.That(json["allowScriptTags"].GetBoolean(), Is.False);
        Assert.That(json["inlineScriptNonce"].GetString(), Is.EqualTo("nonce"));
        Assert.That(json["extensions"].GetString(), Is.EqualTo("sse, ws"));
        Assert.That(json["inlineStyleNonce"].GetString(), Is.EqualTo("style-nonce"));
        Assert.That(json["attributesToSettle"].GetRawText(), Is.EqualTo("[\"class\",\"style\"]"));
        Assert.That(json["useTemplateFragments"].GetBoolean(), Is.True);
        Assert.That(json["wsReconnectDelay"].GetString(), Is.EqualTo("exponential"));
        Assert.That(json["wsBinaryType"].GetString(), Is.EqualTo("arraybuffer"));
        Assert.That(json["disableSelector"].GetString(), Is.EqualTo("[data-disable]"));
        Assert.That(json["withCredentials"].GetBoolean(), Is.True);
        Assert.That(json["disableInheritance"].GetBoolean(), Is.True);
        Assert.That(json["implicitInheritance"].GetBoolean(), Is.False);
        Assert.That(json["timeout"].GetInt32(), Is.EqualTo(5000));
        Assert.That(json["defaultTimeout"].GetInt32(), Is.EqualTo(5000));
        Assert.That(json["mode"].GetString(), Is.EqualTo("cors"));
        Assert.That(json["scrollBehavior"].GetString(), Is.EqualTo("instant"));
        Assert.That(json["defaultFocusScroll"].GetBoolean(), Is.True);
        Assert.That(json["getCacheBusterParam"].GetBoolean(), Is.True);
        Assert.That(json["globalViewTransitions"].GetBoolean(), Is.True);
        Assert.That(json["transitions"].GetBoolean(), Is.True);
        Assert.That(json["morphIgnore"].GetRawText(), Is.EqualTo("[\"data-htmx-powered\",\"data-preserve\"]"));
        Assert.That(json["morphSkip"].GetString(), Is.EqualTo("[data-skip]"));
        Assert.That(json["morphSkipChildren"].GetString(), Is.EqualTo("[data-skip-children]"));
        Assert.That(json["morphScanLimit"].GetInt32(), Is.EqualTo(20));
        Assert.That(json["noSwap"].GetRawText(), Is.EqualTo("[\"204\",\"4xx\"]"));
        Assert.That(json["methodsThatUseUrlParams"].GetRawText(), Is.EqualTo("[\"get\",\"delete\"]"));
        Assert.That(json["selfRequestsOnly"].GetBoolean(), Is.False);
        Assert.That(json["ignoreTitle"].GetBoolean(), Is.True);
        Assert.That(json["scrollIntoViewOnBoost"].GetBoolean(), Is.False);
        Assert.That(json["triggerSpecsCache"].GetRawText(), Is.EqualTo("{}"));
        Assert.That(json["allowNestedOobSwaps"].GetBoolean(), Is.True);
        Assert.That(json["historyRestoreAsHxRequest"].GetBoolean(), Is.False);
        Assert.That(json["reportValidityOfForms"].GetBoolean(), Is.True);
    }

    [Test]
    public async Task ProcessAsync_EscapesHtmlSensitiveCharacters()
    {
        var helper = new HtmxConfigTagHelper(new StubAntiforgery())
        {
            RequestClass = "<a> & \"b\" 'c'"
        };

        var output = TestHelper.CreateTagHelperOutput();
        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        var content = GetContent(output);
        Assert.That(content, Is.EqualTo("{\"requestClass\":\"\\u003Ca\\u003E \\u0026 \\u0022b\\u0022 \\u0027c\\u0027\"}"));
    }

    [Test]
    public async Task ProcessAsync_IncludeAntiForgeryToken_SerializesTokens()
    {
        var antiforgery = new StubAntiforgery();
        var httpContext = new DefaultHttpContext();
        var helper = new HtmxConfigTagHelper(antiforgery)
        {
            IncludeAntiForgeryToken = true,
            ViewContext = new ViewContext
            {
                HttpContext = httpContext
            }
        };

        var output = TestHelper.CreateTagHelperOutput();
        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(antiforgery.StoredHttpContext, Is.SameAs(httpContext));

        var json = JsonHelper.ParseJson(GetContent(output));
        var antiForgery = json["antiForgery"];

        Assert.That(antiForgery.GetProperty("requestToken").GetString(), Is.EqualTo("request-token"));
        Assert.That(antiForgery.GetProperty("headerName").GetString(), Is.EqualTo("RequestVerificationToken"));
        Assert.That(antiForgery.GetProperty("cookieToken").GetString(), Is.EqualTo("cookie-token"));
    }

    [Test]
    public async Task ProcessAsync_ExecutesChildContent_CollectsResponseHandlingEntries()
    {
        var items = new Dictionary<object, object>();
        var context = TestHelper.CreateTagHelperContext("htmx-config", null, items);
        var child = new ResponseHandlingTagHelper
        {
            Code = "404",
            Swap = false
        };

        var output = new TagHelperOutput("htmx-config", [], async (_, _) =>
        {
            await child.ProcessAsync(context, TestHelper.CreateTagHelperOutput("response-handling"));
            return new DefaultTagHelperContent();
        });

        var helper = new HtmxConfigTagHelper(new StubAntiforgery());
        await helper.ProcessAsync(context, output);

        Assert.That(helper.ResponseHandling!.Count, Is.EqualTo(1));
        Assert.That(helper.ResponseHandling[0].Code, Is.EqualTo("404"));
        Assert.That(helper.ResponseHandling[0].Swap, Is.False);
    }

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
