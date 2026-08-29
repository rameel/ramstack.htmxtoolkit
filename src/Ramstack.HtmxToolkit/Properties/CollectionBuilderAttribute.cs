#if !NET8_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Indicates the method used to build a collection from a collection expression.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
internal sealed class CollectionBuilderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBuilderAttribute"/> class.
    /// </summary>
    /// <param name="builderType">The type containing the builder method.</param>
    /// <param name="methodName">The name of the builder method.</param>
    public CollectionBuilderAttribute(Type builderType, string methodName)
    {
        BuilderType = builderType;
        MethodName = methodName;
    }

    /// <summary>
    /// Gets the type containing the builder method.
    /// </summary>
    public Type BuilderType { get; }

    /// <summary>
    /// Gets the name of the builder method.
    /// </summary>
    public string MethodName { get; }
}
#endif
