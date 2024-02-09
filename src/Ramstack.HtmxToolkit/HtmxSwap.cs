namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines the values for the <c>hx-swap</c> attribute.
/// </summary>
public enum HtmxSwap
{
    /// <summary>
    /// Replace the inner html of the target element.
    /// </summary>
    InnerHtml,

    /// <summary>
    /// Replace the entire target element with the response.
    /// </summary>
    OuterHtml,

    /// <summary>
    /// Insert the response before the target element.
    /// </summary>
    BeforeBegin,

    /// <summary>
    /// Insert the response before the first child of the target element.
    /// </summary>
    AfterBegin,

    /// <summary>
    /// Insert the response after the last child of the target element.
    /// </summary>
    BeforeEnd,

    /// <summary>
    /// Insert the response after the target element.
    /// </summary>
    AfterEnd,

    /// <summary>
    /// Deletes the target element regardless of the response.
    /// </summary>
    Delete,

    /// <summary>
    /// Does not append content from response (out of band items will still be processed).
    /// </summary>
    None
}
