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
    /// <para>
    ///   HTMX 4.x triggers them after the swap completes.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// </remarks>
    Receive,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Swap</c> header, which triggers events after the swap step.
    /// </summary>
    /// <remarks>
    /// <para>HTMX 1.x and 2.x emit these events through <c>HX-Trigger-After-Swap</c>.</para>
    /// <para>
    ///   HTMX 4.x emits them through <c>HX-Trigger</c>, which also runs after the swap.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// </remarks>
    AfterSwap,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Settle</c> header, which triggers events
    /// after the settle step.
    /// </summary>
    /// <remarks>
    /// <para>HTMX 1.x and 2.x emit these events through <c>HX-Trigger-After-Settle</c>.</para>
    /// <para>
    ///   HTMX 4.x emits them through <c>HX-Trigger</c> after the swap; the requested
    ///   after-settle timing cannot be preserved.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// </remarks>
    AfterSettle
}
