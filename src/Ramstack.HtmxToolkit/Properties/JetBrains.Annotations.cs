/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

#nullable disable

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable ConvertToPrimaryConstructor

namespace JetBrains.Annotations;

/// <summary>
/// Indicates that the value of the marked element could be <c>null</c> sometimes,
/// so checking for <c>null</c> is required before its usage.
/// </summary>
/// <example><code>
/// [CanBeNull] object Test() => null;
/// 
/// void UseTest() {
///   var p = Test();
///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
/// }
/// </code></example>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
    AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
internal sealed class CanBeNullAttribute : Attribute;

/// <summary>
/// Indicates that the value of the marked element can never be <c>null</c>.
/// </summary>
/// <example><code>
/// [NotNull] object Foo() {
///   return null; // Warning: Possible 'null' assignment
/// }
/// </code></example>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
    AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
internal sealed class NotNullAttribute : Attribute;

/// <summary>
/// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
/// so this symbol will be ignored by usage-checking inspections. <br/>
/// You can use <see cref="ImplicitUseKindFlags"/> and <see cref="ImplicitUseTargetFlags"/>
/// to configure how this attribute is applied.
/// </summary>
/// <example><code>
/// [UsedImplicitly]
/// public class TypeConverter {}
/// 
/// public class SummaryData
/// {
///   [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
///   public SummaryData() {}
/// }
/// 
/// [UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.Default)]
/// public interface IService {}
/// </code></example>
[AttributeUsage(AttributeTargets.All)]
internal sealed class UsedImplicitlyAttribute : Attribute
{
    public UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    public ImplicitUseKindFlags UseKindFlags { get; }

    public ImplicitUseTargetFlags TargetFlags { get; }
}

/// <summary>
/// Can be applied to attributes, type parameters, and parameters of a type assignable from <see cref="System.Type"/> .
/// When applied to an attribute, the decorated attribute behaves the same as <see cref="UsedImplicitlyAttribute"/>.
/// When applied to a type parameter or to a parameter of type <see cref="System.Type"/>,
/// indicates that the corresponding type is used implicitly.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter | AttributeTargets.Parameter)]
internal sealed class MeansImplicitUseAttribute : Attribute
{
    public MeansImplicitUseAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    [UsedImplicitly] public ImplicitUseKindFlags UseKindFlags { get; }

    [UsedImplicitly] public ImplicitUseTargetFlags TargetFlags { get; }
}

/// <summary>
/// Specifies the details of an implicitly used symbol when it is marked
/// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
/// </summary>
[Flags]
internal enum ImplicitUseKindFlags
{
    Default = Access | Assign | InstantiatedWithFixedConstructorSignature,
    /// <summary>Only entity marked with attribute considered used.</summary>
    Access = 1,
    /// <summary>Indicates implicit assignment to a member.</summary>
    Assign = 2,
    /// <summary>
    /// Indicates implicit instantiation of a type with fixed constructor signature.
    /// That means any unused constructor parameters won't be reported as such.
    /// </summary>
    InstantiatedWithFixedConstructorSignature = 4,
    /// <summary>Indicates implicit instantiation of a type.</summary>
    InstantiatedNoFixedConstructorSignature = 8,
}

/// <summary>
/// Specifies what is considered to be used implicitly when marked
/// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
/// </summary>
[Flags]
internal enum ImplicitUseTargetFlags
{
    Default = Itself,
    Itself = 1,
    /// <summary>Members of the type marked with the attribute are considered used.</summary>
    Members = 2,
    /// <summary> Inherited entities are considered used. </summary>
    WithInheritors = 4,
    /// <summary>Entity marked with the attribute and all its members considered used.</summary>
    WithMembers = Itself | Members
}

#region ASP.NET MVC

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC action. If applied to a method, the MVC action name is calculated
/// implicitly from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
internal sealed class AspMvcActionAttribute : Attribute
{
    public AspMvcActionAttribute() { }

    public AspMvcActionAttribute([NotNull] string anonymousProperty)
    {
        AnonymousProperty = anonymousProperty;
    }

    [CanBeNull] public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. Indicates that the marked parameter is an MVC area.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
internal sealed class AspMvcAreaAttribute : Attribute
{
    public AspMvcAreaAttribute() { }

    public AspMvcAreaAttribute([NotNull] string anonymousProperty)
    {
        AnonymousProperty = anonymousProperty;
    }

    [CanBeNull] public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter is
/// an MVC controller. If applied to a method, the MVC controller name is calculated
/// implicitly from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
internal sealed class AspMvcControllerAttribute : Attribute
{
    public AspMvcControllerAttribute() { }

    public AspMvcControllerAttribute([NotNull] string anonymousProperty)
    {
        AnonymousProperty = anonymousProperty;
    }

    [CanBeNull] public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC view component. If applied to a method, the MVC view name is calculated implicitly
/// from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Controller.View(Object)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
internal sealed class AspMvcViewAttribute : Attribute;

#endregion
