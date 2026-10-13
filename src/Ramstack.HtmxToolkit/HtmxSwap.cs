namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies HTMX swap styles.
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
    /// <remarks>
    /// <para>
    ///   Supported natively by HTMX 4.x.
    /// </para>
    /// <para>
    ///   With HTMX 1.x or 2.x, activate the <c>ramstack-swap</c> extension and load Idiomorph.
    ///   Without Idiomorph, the extension logs a warning and falls back to <see cref="InnerHtml" />.
    /// </para>
    /// </remarks>
    InnerMorph,

    /// <summary>
    /// Morphs the target element itself.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Supported natively by HTMX 4.x.
    /// </para>
    /// <para>
    ///   With HTMX 1.x or 2.x, activate the <c>ramstack-swap</c> extension and load Idiomorph.
    ///   Without Idiomorph, the extension logs a warning and falls back to <see cref="OuterHtml" />.
    /// </para>
    /// </remarks>
    OuterMorph,

    /// <summary>
    /// Keeps the target element, replaces its attributes with those of the response's root element,
    /// and replaces its children with the root element's children.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Supported natively by HTMX 4.x.
    /// </para>
    /// <para>
    ///   With HTMX 1.x or 2.x, activate the <c>ramstack-swap</c> extension,
    ///   which performs the same steps.
    /// </para>
    /// </remarks>
    OuterSync,

    /// <summary>
    /// Replaces the text content of the target element.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Supported natively by HTMX 2.x and 4.x.
    /// </para>
    /// <para>
    ///   With HTMX 1.x, activate the <c>ramstack-swap</c> extension.
    ///   Idiomorph is not required for this style.
    /// </para>
    /// </remarks>
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
