namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the kind of HTMX request accepted by an action.
/// </summary>
public enum HtmxRequestKind
{
    /// <summary>
    /// Accepts both boosted and non-boosted HTMX requests.
    /// </summary>
    Any,

    /// <summary>
    /// Accepts only boosted HTMX requests.
    /// </summary>
    Boosted,

    /// <summary>
    /// Accepts only non-boosted HTMX requests.
    /// </summary>
    NonBoosted
}
