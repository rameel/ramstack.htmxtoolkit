namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies when client-side events are triggered, by selecting the response header that carries them.
/// </summary>
public enum HtmxTriggerTiming
{
    /// <summary>
    /// Maps to the <c>HX-Trigger</c> header.
    /// </summary>
    /// <remarks>
    /// <para>HTMX 1.9.x and 2.x trigger these events when the response is received, before the swap.</para>
    /// <para>
    ///   HTMX 4.x triggers them when the request completes, which is after the swap and settle steps
    ///   whenever a swap is performed. To act before the swap in HTMX 4.x, handle <c>htmx:before:swap</c>
    ///   on the client.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// </remarks>
    Receive,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Swap</c> header, which triggers events after the swap step.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   HTMX 1.9.x and 2.x trigger these events only when the response is swapped,
    ///   so they do not fire for a status code such as 204.
    /// </para>
    /// <para>
    ///   HTMX 4.x has no <c>HX-Trigger-After-Swap</c> header, so these events are emitted through
    ///   <c>HX-Trigger</c>. They fire when the request completes: after the swap and settle steps
    ///   whenever a swap is performed, and also when the response is not swapped.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// </remarks>
    AfterSwap,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Settle</c> header, which triggers events
    /// after the settle step.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   HTMX 1.9.x and 2.x trigger these events only when the response is swapped,
    ///   so they do not fire for a status code such as 204.
    /// </para>
    /// <para>
    ///   HTMX 4.x has no <c>HX-Trigger-After-Settle</c> header, so these events are emitted through
    ///   <c>HX-Trigger</c>. They fire when the request completes: after the swap and settle steps
    ///   whenever a swap is performed, and also when the response is not swapped.
    ///   See <see href="https://github.com/bigskysoftware/htmx/pull/3900">PR #3900</see>.
    /// </para>
    /// </remarks>
    AfterSettle
}
