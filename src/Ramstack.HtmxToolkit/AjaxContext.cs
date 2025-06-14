namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents the context for an AJAX request.
/// </summary>
public sealed class AjaxContext
{
    /// <summary>
    /// Gets or sets the path used for the AJAX request.
    /// </summary>
    internal string? Path { get; set; }

    /// <summary>
    /// Gets or sets the source element that initiated the request.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the event that triggered the request.
    /// </summary>
    public string? Event { get; set; }

    /// <summary>
    /// Gets or sets the name of the client-side callback function that will handle the response HTML.
    /// </summary>
    public string? Handler { get; set; }

    /// <summary>
    /// Gets or sets the target element into which the response will be swapped.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets how the response will be swapped relative to the target element.
    /// </summary>
    public HtmxSwap? Swap { get; set; }

    /// <summary>
    /// Gets or sets the values to submit with the request.
    /// </summary>
    public object? Values { get; set; }

    /// <summary>
    /// Gets or sets the headers to include with the request.
    /// </summary>
    public object? Headers { get; set; }

    /// <summary>
    /// Gets or sets a selector used to filter the content to swap from the response.
    /// </summary>
    public string? Select { get; set; }
}
