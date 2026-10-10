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
    /// Gets or sets the type of HTMX request accepted by the action.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The default, <see cref="HtmxRequestType.Unspecified" />, does not restrict the request type.
    ///   Other values require a matching <c>HX-Request-Type</c> header.
    /// </para>
    /// <para>
    ///   HTMX 1.x and 2.x do not send this header. The request type is not inferred from boosted navigation.
    ///   When <see cref="BoostedFilter" /> is also set, both conditions must be satisfied.
    /// </para>
    /// </remarks>
    public HtmxRequestType RequestType { get; set; }

    /// <summary>
    /// Gets or sets the boosted navigation filter for the action.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><see cref="HtmxBoostedFilter.Any" />, the default, does not restrict boosted navigation.</item>
    ///   <item><see cref="HtmxBoostedFilter.Boosted" /> accepts only boosted HTMX requests.</item>
    ///   <item><see cref="HtmxBoostedFilter.NonBoosted" /> accepts only non-boosted HTMX requests.</item>
    /// </list>
    /// </remarks>
    public HtmxBoostedFilter BoostedFilter { get; set; }

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        var request = context.RouteContext.HttpContext.Request;
        if (!request.IsHtmxRequest(out var headers))
            return false;

        if (RequestType != HtmxRequestType.Unspecified && RequestType != headers.RequestType)
            return false;

        if (BoostedFilter == HtmxBoostedFilter.Any)
            return true;

        var filter = headers.Boosted
            ? HtmxBoostedFilter.Boosted
            : HtmxBoostedFilter.NonBoosted;

        return BoostedFilter == filter;
    }
}
