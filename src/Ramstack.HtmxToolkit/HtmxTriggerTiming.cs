namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the response header used to trigger client-side events.
/// </summary>
public enum HtmxTriggerTiming
{
    /// <summary>
    /// Maps to the <c>HX-Trigger</c> header.
    /// </summary>
    /// <remarks>
    /// <para>HTMX 1.x and 2.x trigger these events when the response is received.</para>
    /// <para>HTMX 4.x triggers them after the swap completes.</para>
    /// </remarks>
    Receive,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Swap</c> header, which triggers events after the swap step.
    /// </summary>
    /// <remarks>Supported only in HTMX 1.x and 2.x.</remarks>
    AfterSwap,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Settle</c> header, which triggers events
    /// after the settle step.
    /// </summary>
    /// <remarks>Supported only in HTMX 1.x and 2.x.</remarks>
    AfterSettle
}
