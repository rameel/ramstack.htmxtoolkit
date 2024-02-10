using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents an attribute that designates an action to be exclusively executed for htmx requests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class HtmxRequestAttribute : Attribute, IActionConstraint
{
    /// <inheritdoc />
    public int Order => 0;

    /// <summary>
    /// Gets or sets a value indicates whether the action should be executed
    /// exclusively for boosted or non-boosted requests.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>If set to <c>true</c>, the action will be executed exclusively for boosted requests.</item>
    ///   <item>If set to <c>false</c>, the action will be executed exclusively for non-boosted requests.</item>
    ///   <item>If set to <c>null</c>, the action will be executed for any htmx request.</item>
    /// </list>
    /// </remarks>
    public bool? Boosted { get; set; }

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        var request = context.RouteContext.HttpContext.Request;
        var boosted = Boosted;

        return request.IsHtmxRequest() && (boosted is null || request.IsHtmxBoosted() == boosted.Value);
    }
}
