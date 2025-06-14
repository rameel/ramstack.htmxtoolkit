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
    /// Returns an HTML string containing the script for htmx to integrate with the anti-forgery feature of ASP.NET Core.
    /// </summary>
    /// <param name="_">The <see cref="IHtmlHelper"/> instance that this method extends.</param>
    /// <param name="debug">A boolean value indicating whether to use the debug version of the script.
    /// Defaults to <see langword="false" />.</param>
    /// <returns>
    /// An HTML string of the script.
    /// </returns>
    public static IHtmlContent HtmxAntiforgeryScript(this IHtmlHelper _, bool debug = false) =>
        debug ? s_debugScript : s_script;

    /// <summary>
    /// Returns an HTML string of the path to the script to integrate with the anti-forgery feature of ASP.NET Core.
    /// </summary>
    /// <param name="_">The <see cref="IHtmlHelper"/> instance that this method extends.</param>
    /// <param name="debug">A boolean value indicating whether to use the debug version of the script.
    /// Defaults to <see langword="false" />.</param>
    /// <returns>
    /// An HTML string of the path to the script.
    /// </returns>
    public static IHtmlContent HtmxAntiforgeryScriptPath(this IHtmlHelper _, bool debug = false) =>
        debug ? DebugPath : Path;
}
