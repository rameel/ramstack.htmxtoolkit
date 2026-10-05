namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies whether an action accepts boosted or non-boosted HTMX requests.
/// </summary>
public enum HtmxBoostedFilter
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
