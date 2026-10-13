using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Restricts an action to HTMX requests.
/// </summary>
/// <remarks>
/// The attribute accepts the requests for which
/// <see cref="HttpRequestExtensions.IsHtmxRequest(Microsoft.AspNetCore.Http.HttpRequest)" /> returns
/// <see langword="true" />. History restoration requests are not among them, because HTMX treats them
/// as full page loads, so MVC selects an action without the attribute for the same route.
/// </remarks>
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
    ///   Other values require a matching <see cref="HtmxRequestHeaders.RequestType" />.
    ///   When <see cref="BoostedFilter" /> is also set, both conditions must be satisfied.
    /// </para>
    /// <para>
    ///   HTMX 4.x sends the <c>HX-Request-Type</c> header itself; for HTMX 1.9.x and 2.x,
    ///   the Toolkit script (<c>htmx-toolkit.js</c>) adds it.
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
