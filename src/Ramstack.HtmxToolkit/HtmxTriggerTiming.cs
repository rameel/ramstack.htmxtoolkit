namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the time at which an event will be triggered in HTMX.
/// </summary>
public enum HtmxTriggerTiming
{
    /// <summary>
    /// Maps to the <c>HX-Trigger</c> header that is used to trigger an event
    /// on the client side after the server response is processed.
    /// </summary>
    Receive,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Swap</c> header that is used to trigger an event
    /// on the client side after the response content has been swapped into the DOM.
    /// </summary>
    AfterSwap,

    /// <summary>
    /// Maps to the <c>HX-Trigger-After-Settle</c> header that is used to trigger an event
    /// on the client side after the HTMX request has settled.
    /// </summary>
    AfterSettle
}
