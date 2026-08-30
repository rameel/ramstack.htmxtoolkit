using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ramstack.HtmxToolkit.Builder;

/// <summary>
/// Provides HTMX Toolkit endpoint mappings for an <see cref="IEndpointRouteBuilder" />.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Gets the current path to the HTMX Toolkit script asset.
    /// </summary>
    internal static string AssetPath { get; private set; } = $"/htmxtoolkit/{HtmxAssets.Hash}";

    /// <summary>
    /// Maps an endpoint that serves the HTMX Toolkit script at the default path.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>
    /// The endpoint convention builder for the mapped script endpoint.
    /// </returns>
    public static IEndpointConventionBuilder MapHtmxToolkitScript(this IEndpointRouteBuilder builder) =>
        builder.MapHtmxToolkitScript(AssetPath);

    /// <summary>
    /// Maps an endpoint that serves the HTMX Toolkit script at the specified path.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <param name="path">The path at which to serve the script.</param>
    /// <returns>
    /// The endpoint convention builder for the mapped script endpoint.
    /// </returns>
    public static IEndpointConventionBuilder MapHtmxToolkitScript(this IEndpointRouteBuilder builder, string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(
                $"The '{nameof(path)}' parameter cannot be null or empty.",
                nameof(path));

        if (AssetPath != path)
        {
            if (path[0] != '/')
                path = "/" + path;

            AssetPath = path;
            HtmlHelperExtensions.Path = new HtmlString(path);
            HtmlHelperExtensions.DebugPath = new HtmlString(path + "?debug");
        }

        return builder.MapGet(path, static context =>
        {
            context.Response.ContentType = "text/javascript";
            context.Response.Headers.CacheControl = "public,max-age=31536000";

            return context.Response.WriteAsync(
                context.Request.QueryString.Value == "?debug"
                    ? HtmxAssets.DebugScript
                    : HtmxAssets.Script);
        });
    }
}
