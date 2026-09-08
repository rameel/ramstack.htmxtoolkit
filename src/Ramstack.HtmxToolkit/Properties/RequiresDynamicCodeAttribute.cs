#if !NET7_0_OR_GREATER

// ReSharper disable CheckNamespace
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that a member requires the ability to generate code at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
internal sealed class RequiresDynamicCodeAttribute : Attribute
{
    /// <summary>
    /// Gets a message describing why the member requires dynamic code.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets or sets an optional URL containing more information.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiresDynamicCodeAttribute" /> class.
    /// </summary>
    /// <param name="message">A message describing why the member requires dynamic code.</param>
    public RequiresDynamicCodeAttribute(string message) =>
        Message = message;
}

#endif
