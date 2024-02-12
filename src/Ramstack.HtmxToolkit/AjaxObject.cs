using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a wrapper of the <see cref="AjaxContext"/> for the serialization purpose.
/// </summary>
internal readonly struct AjaxObject
{
    private readonly AjaxContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AjaxObject"/>.
    /// </summary>
    /// <param name="path">The path of the AJAX request.</param>
    /// <param name="context">The <see cref="AjaxContext"/>.</param>
    public AjaxObject(string path, AjaxContext context)
    {
        context.Path = path;
        _context = context;
    }

    [JsonPropertyName("path")]    public string? Path => _context.Path;
    [JsonPropertyName("source")]  public string? Source => _context.Source;
    [JsonPropertyName("event")]   public string? Event => _context.Event;
    [JsonPropertyName("handler")] public string? Handler => _context.Handler;
    [JsonPropertyName("target")]  public string? Target => _context.Target;
    [JsonPropertyName("swap")]    public string? Swap => _context.Swap?.GetSwapValue();
    [JsonPropertyName("values")]  public IDictionary<string, string?>? Values => _context.Values;
    [JsonPropertyName("headers")] public IDictionary<string, string>? Headers => _context.Headers;
    [JsonPropertyName("select")]  public string? Select => _context.Select;
}
