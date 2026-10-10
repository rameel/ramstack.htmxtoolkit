using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Exposes location options in the JSON format expected by HTMX.
/// </summary>
/// <param name="path">The path used for the AJAX request.</param>
/// <param name="options">The location options to serialize.</param>
internal readonly struct HtmxLocationOptionsJsonView(string path, HtmxLocationOptions options)
{
    /// <summary>
    /// Gets the path used for the AJAX request.
    /// </summary>
    public string Path => path;

    /// <summary>
    /// Gets the source selector.
    /// </summary>
    public string? Source => options.Source;

    /// <summary>
    /// Gets the target selector.
    /// </summary>
    public string? Target => options.Target;

    /// <summary>
    /// Gets the swap expression.
    /// </summary>
    public string? Swap => options.SwapExpression;

    /// <summary>
    /// Gets the form field values.
    /// </summary>
    public IDictionary<string, HtmxFieldValues>? Values => options.Values;

    /// <summary>
    /// Gets the request headers.
    /// </summary>
    public IDictionary<string, string>? Headers => options.Headers;

    /// <summary>
    /// Gets the content selector.
    /// </summary>
    public string? Select => options.Select;

    /// <summary>
    /// Gets the out-of-band content selectors.
    /// </summary>
    [JsonPropertyName("selectOOB")]
    public string? SelectOob => options.SelectOob;

    /// <summary>
    /// Gets the URL to push, or the string that disables pushing.
    /// </summary>
    public string Push => options.HistoryAction == HtmxHistoryAction.Push
        ? options.HistoryUrl ?? "true"
        : "false";

    /// <summary>
    /// Gets the URL to replace, or the string that disables replacement.
    /// </summary>
    public string Replace => options.HistoryAction == HtmxHistoryAction.Replace
        ? options.HistoryUrl ?? "true"
        : "false";
}
