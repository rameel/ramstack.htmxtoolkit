using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.Builder;

/// <summary>
/// Provides extension methods for the <see cref="IEndpointRouteBuilder"/> interface to add "htmxtoolkit" endpoints.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Gets the default path to the htmx anti-forgery script asset.
    /// </summary>
    internal static string AssetPath { get; private set; } = $"/htmxtoolkit/{HtmxAssets.Hash}";

    /// <summary>
    /// Maps a GET request to the default path for the htmx anti-forgery script.
    /// </summary>
    /// <remarks>
    /// Ensure to include the following script tag in your <c>Layout.cshtml</c> or <c>Razor view</c>:
    /// <code>
    /// <![CDATA[
    /// <script src="@Html.HtmxAntiforgeryScriptPath()"></script>
    /// or
    /// <script src="@Html.HtmxAntiforgeryScriptPath(debug: true)"></script>
    /// ]]>
    /// </code>
    /// </remarks>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <returns>
    /// An <see cref="IEndpointConventionBuilder"/> that can be used to further configure the endpoint.
    /// </returns>
    public static IEndpointConventionBuilder MapHtmxAntiforgeryScript(this IEndpointRouteBuilder builder) =>
        builder.MapHtmxAntiforgeryScript(AssetPath);

    /// <summary>
    /// Maps a GET request to the specified path for the htmx anti-forgery script.
    /// </summary>
    /// <remarks>
    /// Ensure to include the following script tag in your <c>Layout.cshtml</c> or <c>Razor view</c>:
    /// <code>
    /// <![CDATA[
    /// <script src="@Html.HtmxAntiforgeryScriptPath()"></script>
    /// or
    /// <script src="@Html.HtmxAntiforgeryScriptPath(debug: true)"></script>
    /// ]]>
    /// </code>
    /// </remarks>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="path">The path to map the GET request to.</param>
    /// <returns>
    /// An <see cref="IEndpointConventionBuilder"/> that can be used to further configure the endpoint.
    /// </returns>
    public static IEndpointConventionBuilder MapHtmxAntiforgeryScript(this IEndpointRouteBuilder builder, string path)
    {
        if (path.Length == 0)
            throw new ArgumentException(
                "The 'path' parameter cannot be null or empty.", nameof(path));

        if (AssetPath != path)
        {
            if (path[0] != '/')
                path = "/" + path;

            AssetPath = path;
            HtmlHelperExtensions.Path = new HtmlString(path);
            HtmlHelperExtensions.DebugPath = new HtmlString(path + "?debug");
        }

        return builder.MapGet(path, context =>
        {
            context.Response.ContentType = "text/javascript";
            context.Response.Headers["Cache-Control"] = "public,max-age=31536000";

            return context.Response.WriteAsync(
                context.Request.QueryString.Value == "?debug"
                    ? HtmxAssets.DebugScript
                    : HtmxAssets.Script);
        });
    }
}
