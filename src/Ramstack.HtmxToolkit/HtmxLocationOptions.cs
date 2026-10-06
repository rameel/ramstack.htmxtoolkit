namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents the options for an <c>HX-Location</c> request.
/// </summary>
public sealed class HtmxLocationOptions
{
    /// <summary>
    /// Gets the path used for the AJAX request.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Path { get; internal set; }

    /// <summary>
    /// Gets or sets the CSS selector for the element used as the source of the new request.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the selector for the target element into which the response will be swapped.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets how the response will be swapped relative to the target element.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public HtmxSwap? Swap { get; set; }

    /// <summary>
    /// Gets or sets the form field values to submit with the request.
    /// </summary>
    /// <remarks>
    /// <para>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</para>
    /// <para>
    ///   HTMX 1.9.x and 2.x submit array values as repeated parameters.
    ///   HTMX 4.0.0 converts an array to a single comma-separated field value.
    /// </para>
    /// </remarks>
    public IDictionary<string, HtmxFieldValues>? Values { get; set; }

    /// <summary>
    /// Gets or sets the headers to include with the request.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.
    /// </para>
    /// <para>
    ///   Header values must be strings.
    ///   Pass complex data as a pre-serialized JSON string.
    /// </para>
    /// </remarks>
    public IDictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets a selector used to filter the content to swap from the response.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Select { get; set; }

    /// <summary>
    /// Gets or sets a comma-separated list of selectors, optionally followed by swap styles,
    /// used to select content for out-of-band swaps from the response.
    /// </summary>
    /// <remarks>Supported in HTMX 2.0.8 and later 2.x releases, and in HTMX 4.x.</remarks>
    public string? SelectOob { get; set; }

    /// <summary>
    /// Gets or sets the URL used to update the browser history.
    /// If <see langword="null" />, the URL of the loaded page is used.
    /// </summary>
    /// <remarks>
    /// Ignored when <see cref="HistoryAction" /> is <see cref="HtmxHistoryAction.None" />.
    /// Custom history URLs are supported in HTMX 2.0.8 and later 2.x releases, and in HTMX 4.x.
    /// </remarks>
    public string? HistoryUrl { get; set; }

    /// <summary>
    /// Gets or sets how the browser history is updated.
    /// The default is <see cref="HtmxHistoryAction.Push" />.
    /// </summary>
    public HtmxHistoryAction HistoryAction { get; set; }
}
