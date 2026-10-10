using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Represents the response handling configuration for responses that match
/// a specific HTTP status code pattern.
/// </summary>
public sealed class ResponseHandlingConfig
{
    /// <summary>
    /// Gets or sets the regular expression used to match response status codes.
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
    /// Gets or sets a value indicating whether HTMX should ignore <c>&lt;title&gt;</c> tags in the response.
    /// </summary>
    public bool? IgnoreTitle { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector used to select content from the response.
    /// </summary>
    public string? Select { get; set; }

    /// <summary>
    /// Gets or sets a CSS selector specifying an alternative target for the response.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets an alternative swap style for the response.
    /// </summary>
    /// <remarks>
    /// Reads the style from <see cref="SwapOverrideExpression" />, or returns <see langword="null" />
    /// if the style is not recognized. Assigning a style replaces the complete expression,
    /// including any modifiers. Assigning <see langword="null" /> clears the override.
    /// </remarks>
    [JsonIgnore]
    public HtmxSwap? SwapOverride
    {
        get => EnumHelper.ParseHtmxSwap(SwapOverrideExpression);
        set => SwapOverrideExpression = value.GetSwapValue();
    }

    /// <summary>
    /// Gets or sets the complete swap override expression, including any swap modifiers.
    /// </summary>
    [JsonPropertyName("swapOverride")]
    public string? SwapOverrideExpression { get; set; }
}
