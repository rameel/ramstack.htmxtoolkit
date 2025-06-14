using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents a <see cref="TagHelper"/> implementation
/// used to generate URIs for htmx actions on matching elements.
/// </summary>
/// <param name="factory">The <see cref="IUrlHelperFactory"/>.</param>
[HtmlTargetElement(Attributes = ActionAttributeName)]
[HtmlTargetElement(Attributes = ControllerAttributeName)]
[HtmlTargetElement(Attributes = AreaAttributeName)]
[HtmlTargetElement(Attributes = PageAttributeName)]
[HtmlTargetElement(Attributes = PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = RouteAttributeName)]
[HtmlTargetElement(Attributes = RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = HostAttributeName)]
[HtmlTargetElement(Attributes = ProtocolAttributeName)]
[HtmlTargetElement(Attributes = FragmentAttributeName)]
public sealed class HtmxUrlTagHelper(IUrlHelperFactory factory) : TagHelper
{
    private const string ActionAttributeName = "hx-action";
    private const string ControllerAttributeName = "hx-controller";
    private const string AreaAttributeName = "hx-area";
    private const string PageAttributeName = "hx-page";
    private const string PageHandlerAttributeName = "hx-page-handler";
    private const string FragmentAttributeName = "hx-fragment";
    private const string HostAttributeName = "hx-host";
    private const string ProtocolAttributeName = "hx-protocol";
    private const string RouteAttributeName = "hx-route";
    private const string RouteValuesDictionaryName = "hx-all-route-data";
    private const string RouteValuesPrefix = "hx-route-";

    private static readonly string[] s_methods = ["hx-get", "hx-post", "hx-delete", "hx-put", "hx-patch"];

    private RouteValueDictionary? _routeValues;

    /// <inheritdoc />
    public override int Order => -1000;

    /// <summary>
    /// Gets or sets the name of the route.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if one of <see cref="Action"/>, <see cref="Controller"/>, <see cref="Area"/> or <see cref="Page"/> is non-<see langword="null" />.
    /// </remarks>
    [HtmlAttributeName(RouteAttributeName)]
    public string? Route { get; set; }

    /// <summary>
    /// Gets or sets the name of the area.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route"/> is non-<see langword="null" />.
    /// </remarks>
    [AspMvcArea]
    [HtmlAttributeName(AreaAttributeName)]
    public string? Area { get; set; }

    /// <summary>
    /// Gets or sets the name of the controller.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route"/> or <see cref="Page"/> is non-<see langword="null" />.
    /// </remarks>
    [AspMvcController]
    [HtmlAttributeName(ControllerAttributeName)]
    public string? Controller { get; set; }

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route"/> or <see cref="Page"/> is non-<see langword="null" />.
    /// </remarks>
    [AspMvcAction]
    [HtmlAttributeName(ActionAttributeName)]
    public string? Action { get; set; }

    /// <summary>
    /// Gets or sets the name of the page.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if one of <see cref="Route"/>, <see cref="Action"/> or <see cref="Controller"/> is non-<see langword="null" />.
    /// </remarks>
    [AspMvcView]
    [HtmlAttributeName(PageAttributeName)]
    public string? Page { get; set; }

    /// <summary>
    /// Gets or sets the name of the page handler.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if one of <see cref="Route"/>, <see cref="Action"/> or <see cref="Controller"/> is non-<see langword="null" />.
    /// </remarks>
    [HtmlAttributeName(PageHandlerAttributeName)]
    public string? PageHandler { get; set; }

    /// <summary>
    /// Gets or sets the protocol for the URL, such as "http" or "https".
    /// </summary>
    [HtmlAttributeName(ProtocolAttributeName)]
    public string? Protocol { get; set; }

    /// <summary>
    /// Gets or sets the host name.
    /// </summary>
    [HtmlAttributeName(HostAttributeName)]
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the URL fragment name.
    /// </summary>
    [HtmlAttributeName(FragmentAttributeName)]
    public string? Fragment { get; set; }

    /// <summary>
    /// Gets or sets the additional parameters for the route.
    /// </summary>
    [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
    public RouteValueDictionary RouteValues
    {
        get => _routeValues ??= [];
        set => _routeValues = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext"/> for the current request.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var routeLink = Route != null;
        var actionLink = Controller != null || Action != null;
        var pageLink = Page != null || PageHandler != null;

        if ((routeLink && actionLink) || (routeLink && pageLink) || (actionLink && pageLink))
            Error_CannotDetermineUrl();

        if (Area is not null)
        {
            // Unconditionally replace any value from hx-area
            RouteValues["area"] = Area;
        }

        var generator = factory.GetUrlHelper(ViewContext);

        string? url;
        if (pageLink)
        {
            url = generator.Page(
                pageName: Page,
                pageHandler: PageHandler,
                values: RouteValues,
                protocol: Protocol,
                host: Host,
                fragment: Fragment);
        }
        else if (routeLink)
        {
            url = generator.RouteUrl(
                routeName: Route,
                values: RouteValues,
                protocol: Protocol,
                host: Host,
                fragment: Fragment);
        }
        else
        {
            url = generator.Action(
                action: Action,
                controller: Controller,
                values: RouteValues,
                protocol: Protocol,
                host: Host,
                fragment: Fragment);
        }

        string? definedMethod = null;

        foreach (var method in s_methods)
        {
            if (output.Attributes[method] is null)
                continue;

            if (definedMethod is not null)
                Error_AmbiguousMethods();

            definedMethod = method;
        }

        var attribute = new TagHelperAttribute(definedMethod ?? "hx-get", url);
        output.Attributes.SetAttribute(attribute);

        return Task.CompletedTask;
    }

    [DoesNotReturn]
    private static void Error_CannotDetermineUrl()
    {
        const string Message = $"""
            Cannot determine the URL for the element. The following attributes are mutually exclusive:
            {RouteAttributeName},
            {ControllerAttributeName}, {ActionAttributeName},
            {PageAttributeName}, {PageHandlerAttributeName}
            """;
        throw new InvalidOperationException(Message);
    }

    [DoesNotReturn]
    private static void Error_AmbiguousMethods()
    {
        const string Message = "Ambiguous htmx method. Only one of the following methods is allowed: hx-get, hx-post, hx-delete, hx-put, hx-patch";
        throw new InvalidOperationException(Message);
    }
}
