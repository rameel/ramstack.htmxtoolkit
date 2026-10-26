using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Tests.TagHelpers;

[TestFixture]
public class HtmxUrlTagHelperTests
{
    [Test]
    public async Task ProcessAsync_Action_UsesActionLink()
    {
        var (urlHelper, output) = await RunAsync(t =>
        {
            t.Action = "Index";
            t.Controller = "Home";
        });

        Assert.That(urlHelper.ActionCalls, Is.EqualTo(1));
        Assert.That(urlHelper.RouteUrlCalls, Is.Zero);
        Assert.That(urlHelper.ActionContextReceived!.Action, Is.EqualTo("Index"));
        Assert.That(urlHelper.ActionContextReceived.Controller, Is.EqualTo("Home"));

        var attribute = output.Attributes["hx-get"];
        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.EqualTo(TestUrlHelper.ActionUrl));
    }

    [Test]
    public async Task ProcessAsync_Route_UsesRouteLink()
    {
        var (urlHelper, output) = await RunAsync(t => t.Route = "default");

        Assert.That(urlHelper.RouteUrlCalls, Is.EqualTo(1));
        Assert.That(urlHelper.ActionCalls, Is.Zero);
        Assert.That(urlHelper.RouteContextReceived!.RouteName, Is.EqualTo("default"));

        var attribute = output.Attributes["hx-get"];
        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.EqualTo(TestUrlHelper.RouteUrlResult));
    }

    [Test]
    public async Task ProcessAsync_Page_UsesPageLink()
    {
        var (urlHelper, output) = await RunAsync(t =>
        {
            t.Page = "/Account/Login";
            t.PageHandler = "External";
        });

        Assert.That(urlHelper.RouteUrlCalls, Is.EqualTo(1));
        Assert.That(urlHelper.ActionCalls, Is.Zero);

        var values = (RouteValueDictionary)urlHelper.RouteContextReceived!.Values!;
        Assert.That(values["page"], Is.EqualTo("/Account/Login"));
        Assert.That(values["handler"], Is.EqualTo("External"));

        var attribute = output.Attributes["hx-get"];
        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Value, Is.EqualTo(TestUrlHelper.RouteUrlResult));
    }

    [Test]
    public async Task ProcessAsync_PassesRouteValuesAndArea_ToAction()
    {
        var (urlHelper, _) = await RunAsync(t =>
        {
            t.Action = "Edit";
            t.Area = "Admin";
            t.RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["id"] = "42"
            };
        });

        var values = (RouteValueDictionary)urlHelper.ActionContextReceived!.Values!;
        Assert.That(values["area"], Is.EqualTo("Admin"));
        Assert.That(values["id"], Is.EqualTo("42"));
    }

    [Test]
    public async Task ProcessAsync_UsesExistingHtmxMethod()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("hx-post")
        };

        var (_, output) = await RunAsync(attributes: attributes);

        Assert.That(output.Attributes["hx-get"], Is.Null);
        Assert.That(output.Attributes["hx-post"]!.Value, Is.EqualTo(TestUrlHelper.ActionUrl));
    }

    [TestCase("hx-get", "hx-post")]
    [TestCase("hx-get", "hx-delete")]
    [TestCase("hx-get", "hx-put")]
    [TestCase("hx-get", "hx-patch")]
    [TestCase("hx-post", "hx-delete")]
    [TestCase("hx-post", "hx-put")]
    [TestCase("hx-post", "hx-patch")]
    [TestCase("hx-delete", "hx-put")]
    [TestCase("hx-delete", "hx-patch")]
    [TestCase("hx-put", "hx-patch")]
    [TestCase("hx-get", "hx-action")]
    [TestCase("hx-post", "hx-action")]
    [TestCase("hx-delete", "hx-action")]
    [TestCase("hx-put", "hx-action")]
    [TestCase("hx-patch", "hx-action")]
    public void ProcessAsync_MultipleMethods_Throws(string method1, string method2)
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute(method1),
            new TagHelperAttribute(method2)
        };

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await RunAsync(attributes: attributes, version: HtmxTargetVersion.V4));
    }

    [Test]
    public void ProcessAsync_EmptyMethodValue_Throws()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("hx-post", new HtmlString(""))
        };

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await RunAsync(attributes: attributes));
    }

    [Test]
    public void ProcessAsync_MethodWithUrl_Throws()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("hx-post", new HtmlString("/explicit"))
        };

        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await RunAsync(attributes: attributes));

        Assert.That(exception!.Message, Does.Contain("hx-post"));
    }

    [Test]
    public async Task ProcessAsync_HxAction_WritesUrl_ForHtmx4()
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("hx-action"),
            new TagHelperAttribute("hx-method", new HtmlString("post"))
        };

        var (_, output) = await RunAsync(attributes: attributes, version: HtmxTargetVersion.V4);

        Assert.That(output.Attributes["hx-action"]!.Value, Is.EqualTo(TestUrlHelper.ActionUrl));
        Assert.That(output.Attributes["hx-get"], Is.Null);
    }

    [TestCase(HtmxTargetVersion.V1)]
    [TestCase(HtmxTargetVersion.V2)]
    public void ProcessAsync_HxAction_Throws_BeforeHtmx4(HtmxTargetVersion version)
    {
        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("hx-action")
        };

        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await RunAsync(attributes: attributes, version: version));

        Assert.That(exception!.Message, Does.Contain("HTMX 4.x"));
    }

    [TestCase("Route", "Action")]
    [TestCase("Route", "Controller")]
    [TestCase("Route", "Page")]
    [TestCase("Route", "PageHandler")]
    [TestCase("Action", "Page")]
    [TestCase("Action", "PageHandler")]
    [TestCase("Controller", "Page")]
    [TestCase("Controller", "PageHandler")]
    public void ProcessAsync_MutuallyExclusiveLinkTypes_Throws(string first, string second)
    {
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await RunAsync(t =>
            {
                SetLinkType(t, first);
                SetLinkType(t, second);
            }));

        static void SetLinkType(HtmxUrlTagHelper tagHelper, string linkType)
        {
            switch (linkType)
            {
                case "Route": tagHelper.Route = "route"; break;
                case "Action": tagHelper.Action = "action"; break;
                case "Controller": tagHelper.Controller = "controller"; break;
                case "Page": tagHelper.Page = "page"; break;
                case "PageHandler": tagHelper.PageHandler = "handler"; break;
            }
        }
    }

    private static async Task<(TestUrlHelper UrlHelper, TagHelperOutput Output)> RunAsync(Action<HtmxUrlTagHelper>? configure = null, TagHelperAttributeList? attributes = null, HtmxTargetVersion version = HtmxTargetVersion.V2)
    {
        var urlHelper = new TestUrlHelper();
        var factory = new TestUrlHelperFactory(urlHelper);

        var tagHelper = new HtmxUrlTagHelper(factory, Options.Create(CreateOptions(version)))
        {
            ViewContext = new ViewContext()
        };

        configure?.Invoke(tagHelper);

        var output = TestHelper.CreateTagHelperOutput("div", attributes ?? [new TagHelperAttribute("hx-get")]);
        await tagHelper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        return (urlHelper, output);
    }

    private static HtmxToolkitOptions CreateOptions(HtmxTargetVersion version)
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

        return options;
    }


    private sealed class TestUrlHelperFactory(IUrlHelper helper) : IUrlHelperFactory
    {
        public IUrlHelper GetUrlHelper(ActionContext context) =>
            helper;
    }

    #region Inner type: TestUrlHelper

    private sealed class TestUrlHelper : IUrlHelper
    {
        public const string ActionUrl = "/action";
        public const string RouteUrlResult = "/route";

        public int ActionCalls { get; private set; }
        public int RouteUrlCalls { get; private set; }

        public UrlActionContext? ActionContextReceived { get; private set; }
        public UrlRouteContext? RouteContextReceived { get; private set; }

        public ActionContext ActionContext { get; } = new(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        public string Action(UrlActionContext actionContext)
        {
            ActionCalls++;
            ActionContextReceived = actionContext;
            return ActionUrl;
        }

        public string Content(string? contentPath) =>
            throw new NotSupportedException();

        public bool IsLocalUrl(string? url) =>
            throw new NotSupportedException();

        public string Link(string? routeName, object? values) =>
            throw new NotSupportedException();

        public string RouteUrl(UrlRouteContext routeContext)
        {
            RouteUrlCalls++;
            RouteContextReceived = routeContext;
            return RouteUrlResult;
        }
    }

    #endregion
}
