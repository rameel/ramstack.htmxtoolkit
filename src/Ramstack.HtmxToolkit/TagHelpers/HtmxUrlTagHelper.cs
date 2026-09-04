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
/// Generates URLs for HTMX request attributes on matching elements.
/// </summary>
/// <param name="factory">The factory used to create URL helpers.</param>
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

    private IDictionary<string, string>? _routeValues;

    /// <inheritdoc />
    public override int Order => -1000;

    /// <summary>
    /// Gets or sets the name of the route.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Action" />,
    /// <see cref="Controller" />, <see cref="Page" />, or <see cref="PageHandler" />
    /// is not <see langword="null" />.
    /// </remarks>
    [HtmlAttributeName(RouteAttributeName)]
    public string? Route { get; set; }

    /// <summary>
    /// Gets or sets the name of the area.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route" /> is not <see langword="null" />.
    /// </remarks>
    [AspMvcArea]
    [HtmlAttributeName(AreaAttributeName)]
    public string? Area { get; set; }

    /// <summary>
    /// Gets or sets the name of the controller.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route" />, <see cref="Page" />,
    /// or <see cref="PageHandler" /> is not <see langword="null" />.
    /// </remarks>
    [AspMvcController]
    [HtmlAttributeName(ControllerAttributeName)]
    public string? Controller { get; set; }

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route" />, <see cref="Page" />,
    /// or <see cref="PageHandler" /> is not <see langword="null" />.
    /// </remarks>
    [AspMvcAction]
    [HtmlAttributeName(ActionAttributeName)]
    public string? Action { get; set; }

    /// <summary>
    /// Gets or sets the name of the page.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route" />, <see cref="Action" />,
    /// or <see cref="Controller" /> is not <see langword="null" />.
    /// </remarks>
    [AspMvcView]
    [HtmlAttributeName(PageAttributeName)]
    public string? Page { get; set; }

    /// <summary>
    /// Gets or sets the name of the page handler.
    /// </summary>
    /// <remarks>
    /// Must be <see langword="null" /> if <see cref="Route" />, <see cref="Action" />,
    /// or <see cref="Controller" /> is not <see langword="null" />.
    /// </remarks>
    [HtmlAttributeName(PageHandlerAttributeName)]
    public string? PageHandler { get; set; }

    /// <summary>
    /// Gets or sets the protocol for the URL, such as <c>http</c> or <c>https</c>.
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
    /// Gets or sets the additional route values.
    /// </summary>
    [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
    public IDictionary<string, string> RouteValues
    {
        get => _routeValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        set => _routeValues = value;
    }

    /// <summary>
    /// Gets or sets the view context for the current request.
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

        if ((routeLink && (actionLink || pageLink)) || (actionLink && pageLink))
            Error_CannotDetermineUrl();

        RouteValueDictionary? routeValues = null;
        if (_routeValues is { Count: > 0 })
            routeValues = new RouteValueDictionary(_routeValues!);

        if (Area is not null)
        {
            // Unconditionally replace any existing area route value with the value from hx-area.
            routeValues ??= new RouteValueDictionary();
            routeValues["area"] = Area;
        }

        var generator = factory.GetUrlHelper(ViewContext);

        string? url;
        if (pageLink)
        {
            url = generator.Page(
                pageName: Page,
                pageHandler: PageHandler,
                values: routeValues,
                protocol: Protocol,
                host: Host,
                fragment: Fragment);
        }
        else if (routeLink)
        {
            url = generator.RouteUrl(
                routeName: Route,
                values: routeValues,
                protocol: Protocol,
                host: Host,
                fragment: Fragment);
        }
        else
        {
            url = generator.Action(
                action: Action,
                controller: Controller,
                values: routeValues,
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

    /// <summary>
    /// Throws an exception indicating that mutually exclusive URL attributes were specified.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
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

    /// <summary>
    /// Throws an exception indicating that multiple HTMX method attributes were specified.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_AmbiguousMethods()
    {
        const string Message = "Ambiguous htmx method. Only one of the following methods is allowed: hx-get, hx-post, hx-delete, hx-put, hx-patch";
        throw new InvalidOperationException(Message);
    }
}
