namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the time at which an event will be triggered in htmx.
/// </summary>
public enum HtmxTriggerTiming
{
    /// <summary>
    /// Maps to the <c>HX-Trigger</c> header that is used to trigger an event
    /// on the client side after the server response is processed.
    /// </summary>
    Receive,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Settle</c> header that is used to trigger an event
    /// on the client side after the htmx request has settled.
    /// </summary>
    AfterSettle,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Swap</c> header that is used to trigger an event
    /// on the client side after the htmx request has been swapped.
    /// </summary>
    AfterSwap
}
