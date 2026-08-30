namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the scrolling behavior for a boosted link during page transitions.
/// </summary>
public enum HtmxScrollBehavior
{
    /// <summary>
    /// Uses the <c>auto</c> scrolling behavior.
    /// </summary>
    Auto,

    /// <summary>
    /// Uses the <c>smooth</c> scrolling behavior.
    /// </summary>
    Smooth,

    /// <summary>
    /// Uses the <c>instant</c> scrolling behavior.
    /// Supported only in HTMX 2.x.
    /// </summary>
    Instant
}
