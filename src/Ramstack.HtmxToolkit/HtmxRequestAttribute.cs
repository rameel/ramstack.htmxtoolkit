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
    /// Gets or sets the kind of HTMX request accepted by the action.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><see cref="HtmxRequestKind.Any" />, the default, accepts any HTMX request.</item>
    ///   <item><see cref="HtmxRequestKind.Boosted" /> accepts only boosted HTMX requests.</item>
    ///   <item><see cref="HtmxRequestKind.NonBoosted" /> accepts only non-boosted HTMX requests.</item>
    /// </list>
    /// </remarks>
    public HtmxRequestKind Kind { get; set; }

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        var request = context.RouteContext.HttpContext.Request;
        if (!request.IsHtmxRequest())
            return false;

        return Kind switch
        {
            HtmxRequestKind.Any => true,
            HtmxRequestKind.Boosted => request.IsHtmxBoosted(),
            HtmxRequestKind.NonBoosted => !request.IsHtmxBoosted(),
            _ => false
        };
    }
}
