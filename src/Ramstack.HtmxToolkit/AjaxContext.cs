namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents AJAX context object.
/// </summary>
public sealed class AjaxContext
{
    internal string? Path { get; set; }

    /// <summary>
    /// Gets or sets the source element of the request.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets an event that "triggered" the request.
    /// </summary>
    public string? Event { get; set; }

    /// <summary>
    /// Gets or sets a callback that will handle the response HTML.
    /// </summary>
    public string? Handler { get; set; }

    /// <summary>
    /// Gets or sets the target to swap the response into.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets how the response will be swapped in relative to the target.
    /// </summary>
    public HtmxSwap? Swap { get; set; }

    /// <summary>
    /// Gets or sets values to submit with the request.
    /// </summary>
    public IDictionary<string, string?>? Values { get; set; }

    /// <summary>
    /// Gets or sets headers to submit with the request.
    /// </summary>
    public IDictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets allows you to select the content you want swapped from a response.
    /// </summary>
    public string? Select { get; set; }
}