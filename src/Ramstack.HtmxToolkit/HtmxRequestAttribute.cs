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
    /// Gets or sets a value indicates whether the action
    /// should be executed exclusively for htmx boosted requests.
    /// </summary>
    public bool Boosted { get; set; }

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        var request = context.RouteContext.HttpContext.Request;
        return Boosted
            ? request.IsHtmxBoosted()
            : request.IsHtmxRequest();
    }
}
