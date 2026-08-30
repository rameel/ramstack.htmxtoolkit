namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines the values for the <c>hx-swap</c> attribute.
/// </summary>
public enum HtmxSwap
{
    /// <summary>
    /// Replaces the inner HTML of the target element.
    /// </summary>
    InnerHtml,

    /// <summary>
    /// Replaces the entire target element with the response.
    /// </summary>
    OuterHtml,

    /// <summary>
    /// Morphs the inner HTML of the target element.
    /// </summary>
    InnerMorph,

    /// <summary>
    /// Morphs the target element itself.
    /// </summary>
    OuterMorph,

    /// <summary>
    /// Synchronizes the target element with the response.
    /// </summary>
    OuterSync,

    /// <summary>
    /// Replaces the text content of the target element.
    /// </summary>
    TextContent,

    /// <summary>
    /// Inserts the response before the target element.
    /// </summary>
    BeforeBegin,

    /// <summary>
    /// Inserts the response before the first child of the target element.
    /// </summary>
    AfterBegin,

    /// <summary>
    /// Inserts the response after the last child of the target element.
    /// </summary>
    BeforeEnd,

    /// <summary>
    /// Inserts the response after the target element.
    /// </summary>
    AfterEnd,

    /// <summary>
    /// Deletes the target element, regardless of the response.
    /// </summary>
    Delete,

    /// <summary>
    /// Does not swap content from the response. Out-of-band items are still processed.
    /// </summary>
    None
}
