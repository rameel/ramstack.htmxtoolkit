using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents the options for an <c>HX-Location</c> request.
/// </summary>
public sealed class HtmxLocationOptions
{
    /// <summary>
    /// Gets or sets the path used for the AJAX request.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Path { get; internal set; }

    /// <summary>
    /// Gets or sets the source element that initiated the request.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the target element into which the response will be swapped.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets how the response will be swapped relative to the target element.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    [JsonConverter(typeof(HtmxSwapJsonConverter))]
    public HtmxSwap? Swap { get; set; }

    /// <summary>
    /// Gets or sets the form field values to submit with the request.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public IDictionary<string, HtmxFieldValues>? Values { get; set; }

    /// <summary>
    /// Gets or sets the headers to include with the request.
    /// Header values must be strings; complex data should be passed as a pre-serialized JSON string.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public IDictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets a selector used to filter the content to swap from the response.
    /// </summary>
    /// <remarks>Supported in HTMX 1.9.x, HTMX 2.x, and HTMX 4.x.</remarks>
    public string? Select { get; set; }

    /// <summary>
    /// Gets or sets a selector used to select content for out-of-band swaps from the response.
    /// </summary>
    /// <remarks>Supported in HTMX 2.x and HTMX 4.x.</remarks>
    public string? SelectOOB { get; set; }

    /// <summary>
    /// Gets or sets the path to push into the browser history.
    /// Set to <c>false</c> to prevent the URL from being pushed.
    /// </summary>
    /// <remarks>Supported in HTMX 2.x and HTMX 4.x.</remarks>
    public string? Push { get; set; }

    /// <summary>
    /// Gets or sets the path that replaces the current URL in the browser history.
    /// </summary>
    /// <remarks>Supported in HTMX 2.x and HTMX 4.x.</remarks>
    public string? Replace { get; set; }
}
