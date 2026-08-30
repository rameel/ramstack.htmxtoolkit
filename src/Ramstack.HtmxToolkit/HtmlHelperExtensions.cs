using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Ramstack.HtmxToolkit.Hosting;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides extension methods for the <see cref="IHtmlHelper" /> interface.
/// </summary>
public static class HtmlHelperExtensions
{
    private static readonly HtmlString s_script = new(HtmxAssets.Script);
    private static readonly HtmlString s_debugScript = new(HtmxAssets.DebugScript);

    /// <summary>
    /// Gets or sets the HTML string that represents the path to the minified script.
    /// </summary>
    internal static HtmlString Path { get; set; } = new(EndpointRouteBuilderExtensions.AssetPath);

    /// <summary>
    /// Gets or sets the HTML string that represents the path to the debug script.
    /// </summary>
    internal static HtmlString DebugPath { get; set; } = new(EndpointRouteBuilderExtensions.AssetPath + "?debug");

    /// <summary>
    /// Returns the embedded HTMX Toolkit script.
    /// </summary>
    /// <param name="_">The HTML helper instance.</param>
    /// <param name="debug"><see langword="true" /> to return the debug script;
    /// otherwise, <see langword="false" />.</param>
    /// <returns>
    /// The embedded script content.
    /// </returns>
    public static IHtmlContent HtmxToolkitScript(this IHtmlHelper _, bool debug = false) =>
        debug ? s_debugScript : s_script;

    /// <summary>
    /// Returns the path to the HTMX Toolkit script endpoint.
    /// </summary>
    /// <param name="_">The HTML helper instance.</param>
    /// <param name="debug"><see langword="true" /> to return the debug script path;
    /// otherwise, <see langword="false" />.</param>
    /// <returns>
    /// The HTMX Toolkit script endpoint path.
    /// </returns>
    public static IHtmlContent HtmxToolkitScriptPath(this IHtmlHelper _, bool debug = false) =>
        debug ? DebugPath : Path;
}
