using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Collections;
using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Generates URLs for HTMX request attributes from the standard ASP.NET Core <c>asp-*</c> routing attributes.
/// </summary>
/// <param name="factory">The factory used to create URL helpers.</param>
/// <param name="options">The configured HtmxToolkit options.</param>
/// <remarks>
/// <para>
///   The tag helper applies to elements that have an <c>hx-get</c>, <c>hx-post</c>, <c>hx-put</c>,
///   <c>hx-patch</c>, <c>hx-delete</c>, or HTMX 4.x <c>hx-action</c> attribute together with
///   at least one of the <c>asp-action</c>, <c>asp-controller</c>, <c>asp-area</c>, <c>asp-page</c>,
///   <c>asp-page-handler</c>, <c>asp-route</c>, <c>asp-route-*</c>, <c>asp-all-route-data</c>,
///   or <c>asp-fragment</c> attributes.
///   The generated URL is written to the HTMX attribute, which must not have a value.
/// </para>
/// <para>
///   On <c>&lt;a&gt;</c>, <c>&lt;form&gt;</c>, <c>&lt;button&gt;</c>, and submit or image
///   <c>&lt;input&gt;</c> elements, the built-in ASP.NET Core tag helpers process the same
///   <c>asp-*</c> attributes and also render <c>href</c>, <c>action</c>, or <c>formaction</c>.
/// </para>
/// <para>
///   An <see cref="InvalidOperationException" /> is thrown when more than one HTMX request attribute
///   is present, when the HTMX request attribute has a value, even an empty one, when <c>hx-action</c> is used
///   with a version other than HTMX 4.x, or when attributes from more than one of the following
///   groups are specified: <c>asp-route</c>; <c>asp-controller</c> and <c>asp-action</c>;
///   <c>asp-page</c> and <c>asp-page-handler</c>.
/// </para>
/// </remarks>
[HtmlTargetElement(Attributes = "hx-get," + ActionAttributeName)]
[HtmlTargetElement(Attributes = "hx-get," + ControllerAttributeName)]
[HtmlTargetElement(Attributes = "hx-get," + AreaAttributeName)]
[HtmlTargetElement(Attributes = "hx-get," + PageAttributeName)]
[HtmlTargetElement(Attributes = "hx-get," + PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = "hx-get," + RouteAttributeName)]
[HtmlTargetElement(Attributes = "hx-get," + RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = "hx-get," + RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = "hx-get," + FragmentAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + ActionAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + ControllerAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + AreaAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + PageAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + RouteAttributeName)]
[HtmlTargetElement(Attributes = "hx-post," + RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = "hx-post," + RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = "hx-post," + FragmentAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + ActionAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + ControllerAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + AreaAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + PageAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + RouteAttributeName)]
[HtmlTargetElement(Attributes = "hx-put," + RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = "hx-put," + RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = "hx-put," + FragmentAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + ActionAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + ControllerAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + AreaAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + PageAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + RouteAttributeName)]
[HtmlTargetElement(Attributes = "hx-patch," + RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = "hx-patch," + RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = "hx-patch," + FragmentAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + ActionAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + ControllerAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + AreaAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + PageAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + RouteAttributeName)]
[HtmlTargetElement(Attributes = "hx-delete," + RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = "hx-delete," + RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = "hx-delete," + FragmentAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + ActionAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + ControllerAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + AreaAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + PageAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + PageHandlerAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + RouteAttributeName)]
[HtmlTargetElement(Attributes = "hx-action," + RouteValuesDictionaryName)]
[HtmlTargetElement(Attributes = "hx-action," + RouteValuesPrefix + "*")]
[HtmlTargetElement(Attributes = "hx-action," + FragmentAttributeName)]
[RequiresUnreferencedCode("Razor Page URL generation is not compatible with trimming.")]
public sealed class HtmxUrlTagHelper(IUrlHelperFactory factory, IOptions<HtmxToolkitOptions> options) : TagHelper
{
    private const string ActionAttributeName = "asp-action";
    private const string ControllerAttributeName = "asp-controller";
    private const string AreaAttributeName = "asp-area";
    private const string PageAttributeName = "asp-page";
    private const string PageHandlerAttributeName = "asp-page-handler";
    private const string FragmentAttributeName = "asp-fragment";
    private const string RouteAttributeName = "asp-route";
    private const string RouteValuesDictionaryName = "asp-all-route-data";
    private const string RouteValuesPrefix = "asp-route-";
    private const string HxActionAttributeName = "hx-action";

    private static readonly string[] s_requestAttributes = ["hx-get", "hx-post", "hx-delete", "hx-put", "hx-patch", "hx-action"];

    private IDictionary<string, string>? _routeValues;

    /// <inheritdoc />
    /// <remarks>
    /// Runs after the built-in ASP.NET Core tag helpers that process the same <c>asp-*</c> attributes (-1000)
    /// and before tag helpers with the default order, so that they see the generated URL attribute.
    /// The value lies between -1000 and the built-in
    /// <see cref="Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper" /> (-900),
    /// leaving room for tag helpers that need to run before or after this one.
    /// </remarks>
    public override int Order => -950;

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
    /// Can be combined with a named route, controller action, or Razor Page.
    /// When not <see langword="null" />, overrides any <c>area</c> value in <see cref="RouteValues" />.
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
        get => _routeValues ??= new SmallDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
        var name = GetRequestAttributeName(output);
        if (name is null)
            return Task.CompletedTask;

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
            // Unconditionally replace any existing area route value with the value from asp-area.
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
                protocol: null,
                host: null,
                fragment: Fragment);
        }
        else if (routeLink)
        {
            url = generator.RouteUrl(
                routeName: Route,
                values: routeValues,
                protocol: null,
                host: null,
                fragment: Fragment);
        }
        else
        {
            url = generator.Action(
                action: Action,
                controller: Controller,
                values: routeValues,
                protocol: null,
                host: null,
                fragment: Fragment);
        }

        output.Attributes.SetAttribute(name, url);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the name of the HTMX request attribute that receives the generated URL.
    /// </summary>
    /// <param name="output">The tag helper output to inspect.</param>
    /// <returns>
    /// The name of the HTMX request attribute, or <see langword="null" /> if the element has none.
    /// </returns>
    private string? GetRequestAttributeName(TagHelperOutput output)
    {
        string? name = null;

        foreach (var attributeName in s_requestAttributes)
        {
            var attribute = output.Attributes[attributeName];
            if (attribute is null)
                continue;

            if (name is not null)
                Error_AmbiguousRequestAttributes();

            // Accept only an attribute written without a value, such as <button hx-post>.
            // Any value, even an empty one, is rejected rather than guessed to mean "no URL".
            if (attribute.ValueStyle != HtmlAttributeValueStyle.Minimized)
                Error_RequestAttributeHasValue(attributeName);

            name = attributeName;
        }

        if (name == HxActionAttributeName && options.Value.TargetVersion != HtmxTargetVersion.V4)
            Error_HxActionRequiresHtmx4();

        return name;
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
    /// Throws an exception indicating that multiple HTMX request attributes were specified.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_AmbiguousRequestAttributes()
    {
        const string Message = "Ambiguous HTMX request. Only one of the following attributes is allowed: hx-get, hx-post, hx-delete, hx-put, hx-patch, hx-action";
        throw new InvalidOperationException(Message);
    }

    /// <summary>
    /// Throws an exception indicating that the HTMX request attribute has a value.
    /// </summary>
    /// <param name="attributeName">The name of the HTMX request attribute.</param>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_RequestAttributeHasValue(string attributeName)
    {
        var message = $"The {attributeName} attribute must be written without a value to receive the URL generated from the asp-* attributes. Remove its value, or remove the asp-* attributes.";
        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Throws an exception indicating that <c>hx-action</c> is used with HTMX 1.9.x or 2.x.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_HxActionRequiresHtmx4()
    {
        const string Message = "The hx-action attribute is supported only in HTMX 4.x. Use hx-get, hx-post, hx-delete, hx-put, or hx-patch instead.";
        throw new InvalidOperationException(Message);
    }
}
