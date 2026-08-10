namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the scrolling behavior for a boosted link during page transitions.
/// </summary>
public enum HtmxScrollBehavior
{
    /// <summary>
    /// Specifies instant scrolling behavior, similar to a vanilla link.
    /// </summary>
    Auto,

    /// <summary>
    /// Specifies smooth scrolling to the top of the page.
    /// </summary>
    Smooth,

    /// <summary>
    /// Specifies instant scrolling with no animation.
    /// Supported only in HTMX 2.x.
    /// </summary>
    Instant
}
