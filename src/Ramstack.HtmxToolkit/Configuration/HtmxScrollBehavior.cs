namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Specifies how HTMX scrolls elements into view with the <c>show</c> swap modifier.
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
    /// </summary>
    Instant
}
