using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Razor.TagHelpers;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides extension methods for <see cref="TagHelperOutput" />.
/// </summary>
internal static class TagHelperOutputExtensions
{
    private const string AttributePrefix = "hx-";
    private const string DefaultAliasPrefix = "data-hx-";

    /// <summary>
    /// Throws an exception if the element already has the attribute that the tag helper renders,
    /// or the alias that HTMX reads as the same attribute, such as <c>data-hx-vals</c>,
    /// with or without HTMX 4.x modifiers, such as <c>hx-vals:inherited</c>.
    /// </summary>
    /// <param name="output">The tag helper output to inspect.</param>
    /// <param name="attributeName">The <c>hx-*</c> name of the attribute that the tag helper renders, without modifiers.</param>
    /// <param name="sourceAttributes">The tag helper attributes that render <paramref name="attributeName" />,
    /// as listed in the exception message.</param>
    /// <param name="options">The options that determine the alias prefix.</param>
    public static void ThrowIfAttributeDeclared(this TagHelperOutput output, string attributeName, string sourceAttributes, HtmxToolkitOptions options)
    {
        Debug.Assert(attributeName is "hx-vals" or "hx-headers" or "hx-request" or "hx-config");

        // HTMX 1.9.x and 2.x always read data-hx-*. HTMX 4.x reads the configured prefix.
        var aliasPrefix = (options.HtmxConfig as HtmxV4Config)?.Prefix ?? DefaultAliasPrefix;
        var suffix = attributeName.AsSpan(AttributePrefix.Length);

        var attributes = output.Attributes;
        var count = attributes.Count;

        for (var i = 0; i < count; i++)
        {
            var name = attributes[i].Name;
            if (IsDeclared(name, AttributePrefix, suffix) || IsDeclared(name, aliasPrefix, suffix))
                Error_AttributeDeclared(name, attributeName, sourceAttributes);
        }
    }

    /// <summary>
    /// Determines whether the attribute name consists of the prefix and the suffix, optionally followed by modifiers.
    /// An empty prefix never matches: for HTMX 4.x, it means that there is no alias, not that the bare suffix is one.
    /// </summary>
    /// <param name="name">The attribute name to check.</param>
    /// <param name="prefix">The expected prefix.</param>
    /// <param name="suffix">The expected attribute name without the prefix and modifiers.</param>
    /// <returns>
    /// <see langword="true" /> if the attribute name matches; otherwise, <see langword="false" />.
    /// </returns>
    private static bool IsDeclared(string name, string prefix, ReadOnlySpan<char> suffix)
    {
        if (prefix.Length == 0 || !name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        var rest = name.AsSpan(prefix.Length);
        return rest.StartsWith(suffix, StringComparison.OrdinalIgnoreCase)
            && (rest.Length == suffix.Length || rest[suffix.Length] == ':');
    }

    [DoesNotReturn]
    private static void Error_AttributeDeclared(string declaredName, string attributeName, string sourceAttributes)
    {
        var message = $"The {declaredName} attribute cannot be combined with {sourceAttributes}, which render {attributeName}. Remove either the {declaredName} attribute or those attributes.";
        throw new InvalidOperationException(message);
    }
}
