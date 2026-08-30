namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents the response handling configuration for responses matching a specific HTTP status code pattern.
/// </summary>
public sealed class ResponseHandlingConfig
{
    /// <summary>
    /// Gets or sets a regular expression that will be tested against response status codes.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the response should be swapped into the DOM.
    /// </summary>
    public bool? Swap { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX should treat this response as an error.
    /// </summary>
    public bool? Error { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTMX should ignore title tags in the response.
    /// </summary>
    public bool? IgnoreTitle { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector to use to select content from the response.
    /// </summary>
    public string? Select { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector specifying an alternative target for the response.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets an alternative swap mechanism for the response.
    /// </summary>
    public string? SwapOverride { get; set; }
}
