using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Ramstack.HtmxToolkit.Builder;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for the <see cref="IHtmlHelper"/> interface.
/// </summary>
public static class HtmlHelperExtensions
{
    private static readonly HtmlString s_script = new(HtmxAssets.Script);
    private static readonly HtmlString s_debugScript = new(HtmxAssets.DebugScript);

    /// <summary>
    /// Gets the HTML string that represents the path to the minified version of the script.
    /// </summary>
    internal static HtmlString Path { get; set; } = new(EndpointRouteBuilderExtensions.AssetPath);

    /// <summary>
    /// Gets the HTML string that represents the path to the debug version of the script.
    /// </summary>
    internal static HtmlString DebugPath { get; set; } = new(EndpointRouteBuilderExtensions.AssetPath + "?debug");

    /// <summary>
    /// Returns the HTMX toolkit script content.
    /// </summary>
    /// <param name="_">The HTML helper.</param>
    /// <param name="debug">Whether to return the debug version of the script.</param>
    /// <returns>
    /// The HTMX toolkit script content.
    /// </returns>
    public static IHtmlContent HtmxToolkitScript(this IHtmlHelper _, bool debug = false) =>
        debug ? s_debugScript : s_script;

    /// <summary>
    /// Returns the path to the HTMX toolkit script endpoint.
    /// </summary>
    /// <param name="_">The HTML helper.</param>
    /// <param name="debug">Whether to return the debug version of the script.</param>
    /// <returns>
    /// The HTMX toolkit script endpoint path.
    /// </returns>
    public static IHtmlContent HtmxToolkitScriptPath(this IHtmlHelper _, bool debug = false) =>
        debug ? DebugPath : Path;
}
