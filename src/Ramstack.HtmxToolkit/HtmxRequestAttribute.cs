using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Restricts an action to HTMX requests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class HtmxRequestAttribute : Attribute, IActionConstraint
{
    /// <inheritdoc />
    public int Order => 0;

    /// <summary>
    /// Gets or sets a value indicating whether the action accepts boosted
    /// or non-boosted HTMX requests.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><see langword="true" /> accepts only boosted requests.</item>
    ///   <item><see langword="false" /> accepts only non-boosted requests.</item>
    ///   <item><see langword="null" /> accepts any HTMX request.</item>
    /// </list>
    /// </remarks>
    public bool? Boosted { get; set; }

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        var request = context.RouteContext.HttpContext.Request;
        if (request.IsHtmxRequest())
            return Boosted is null || request.IsHtmxBoosted() == Boosted.GetValueOrDefault();

        return false;
    }
}
