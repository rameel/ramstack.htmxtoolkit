using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.Tests;

/// <summary>
/// Represents a helper class for building <see cref="HttpContext"/> instances in tests.
/// </summary>
internal static class TestHelper
{
    /// <summary>
    /// Creates a <see cref="HttpContext"/> with the given request headers.
    /// </summary>
    /// <param name="headers">The request headers as key/value pairs.</param>
    /// <returns>
    /// The configured <see cref="HttpContext"/>.
    /// </returns>
    public static HttpContext CreateHttpContext(params (string Name, string Value)[] headers)
    {
        var context = new DefaultHttpContext();

        foreach (var (name, value) in headers)
            context.Request.Headers[name] = value;

        return context;
    }

    /// <summary>
    /// Creates a <see cref="HttpContext"/> whose request is an htmx request.
    /// </summary>
    /// <param name="boosted"><see langword="true"/> to mark the request as boosted;
    /// otherwise, <see langword="false"/>.</param>
    /// <returns>
    /// The configured <see cref="HttpContext"/>.
    /// </returns>
    public static HttpContext CreateHtmxRequestContext(bool boosted = false)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HtmxRequestHeaderNames.Request] = "true";

        if (boosted)
            context.Request.Headers[HtmxRequestHeaderNames.Boosted] = "true";

        return context;
    }

    /// <summary>
    /// Creates a <see cref="TagHelperContext"/> with the given tag name, attributes and items.
    /// </summary>
    public static TagHelperContext CreateTagHelperContext(string tagName = "div", TagHelperAttributeList? attributes = null, IDictionary<object, object>? items = null) =>
        new(tagName, attributes ?? [], items ?? new Dictionary<object, object>(), "test");

    /// <summary>
    /// Creates a <see cref="TagHelperOutput"/> with empty child content.
    /// </summary>
    public static TagHelperOutput CreateTagHelperOutput(string tagName = "div", TagHelperAttributeList? attributes = null) =>
        new(tagName, attributes ?? [], (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
}
