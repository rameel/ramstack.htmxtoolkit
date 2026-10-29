using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Provides extension methods for <see cref="TagHelperOutput" />.
/// </summary>
internal static class TagHelperOutputExtensions
{
    /// <summary>
    /// Throws an exception if the element already has the attribute that the tag helper renders,
    /// with or without HTMX 4.x modifiers, such as <c>hx-vals:inherited</c>.
    /// </summary>
    /// <param name="output">The tag helper output to inspect.</param>
    /// <param name="attributeName">The name of the attribute that the tag helper renders, without modifiers.</param>
    /// <param name="sourceAttributes">The tag helper attributes that render <paramref name="attributeName" />,
    /// as listed in the exception message.</param>
    public static void ThrowIfAttributeDeclared(this TagHelperOutput output, string attributeName, string sourceAttributes)
    {
        var attributes = output.Attributes;
        var count = attributes.Count;

        for (var i = 0; i < count; i++)
        {
            var name = attributes[i].Name;
            if (name.StartsWith(attributeName, StringComparison.OrdinalIgnoreCase)
                && (name.Length == attributeName.Length || name[attributeName.Length] == ':'))
                Error_AttributeDeclared(name, attributeName, sourceAttributes);
        }
    }

    [DoesNotReturn]
    private static void Error_AttributeDeclared(string declaredName, string attributeName, string sourceAttributes)
    {
        var message = $"The {declaredName} attribute cannot be combined with {sourceAttributes}, which render {attributeName}. Remove either the {declaredName} attribute or those attributes.";
        throw new InvalidOperationException(message);
    }
}
