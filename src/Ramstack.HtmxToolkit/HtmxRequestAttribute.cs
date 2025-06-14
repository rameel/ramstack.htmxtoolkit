using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Identifies an action that supports htmx requests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class HtmxRequestAttribute : Attribute, IActionConstraint
{
    /// <inheritdoc />
    public int Order => 0;

    /// <summary>
    /// Gets or sets a value that indicates whether the action should be executed
    /// for boosted or non-boosted htmx requests.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>If set to <see langword="true" />, the action will be executed only for boosted requests.</item>
    ///   <item>If set to <see langword="false" />, the action will be executed only for non-boosted requests.</item>
    ///   <item>If set to <see langword="null" />, the action will be executed for any htmx request.</item>
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
