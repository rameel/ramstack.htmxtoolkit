// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides helper methods to validate arguments and throw exceptions if the arguments do not meet specified conditions.
/// </summary>
internal static class Argument
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the specified object is <see langword="null"/>.
    /// </summary>
    /// <param name="value">The object to check.</param>
    /// <param name="paramName">The name of the parameter being checked. Automatically supplied by the compiler.</param>
    public static void ThrowIfNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
            Error_ArgumentNull(paramName);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the specified string is <see langword="null"/>,
    /// or an <see cref="ArgumentException"/> if it is empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="paramName">The name of the parameter being checked. Automatically supplied by the compiler.</param>
    public static void ThrowIfNullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(value))
            Error_ArgumentNullOrEmpty(value, paramName);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the specified string is <see langword="null"/>,
    /// or an <see cref="ArgumentException"/> if it is empty or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="paramName">The name of the parameter being checked. Automatically supplied by the compiler.</param>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            Error_ArgumentNullOrWhiteSpace(value, paramName);
    }

    [DoesNotReturn]
    private static void Error_ArgumentNull(string? paramName) =>
        throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    private static void Error_ArgumentNullOrEmpty(string? value, string? paramName)
    {
        if (value is null)
            throw new ArgumentNullException(paramName);

        throw new ArgumentException(
            "Argument should not be empty.",
            paramName);
    }

    [DoesNotReturn]
    private static void Error_ArgumentNullOrWhiteSpace(string? value, string? paramName)
    {
        if (value is null)
            throw new ArgumentNullException(paramName);

        throw new ArgumentException(
            "Argument should not be empty or whitespace.",
            paramName);
    }
}
