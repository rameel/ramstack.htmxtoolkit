using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Html;

namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Represents configuration for a specific major version of HTMX.
/// </summary>
public abstract class HtmxConfig
{
    private HtmlString? _json;

    /// <summary>
    /// Gets the configured HTMX major version.
    /// </summary>
    [JsonIgnore]
    public HtmxTargetVersion TargetVersion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxConfig" /> class for
    /// the specified target HTMX version.
    /// </summary>
    /// <param name="version">The target HTMX major version
    /// to which this configuration applies.</param>
    internal HtmxConfig(HtmxTargetVersion version) =>
        TargetVersion = version;

    /// <summary>
    /// Returns this configuration serialized as JSON,
    /// cached and reused until the configuration changes.
    /// </summary>
    /// <returns>
    /// An <see cref="HtmlString" /> containing the configuration serialized as JSON.
    /// </returns>
    internal HtmlString ToJson() =>
        _json ??= new HtmlString(Serialize());

    /// <summary>
    /// Serializes this configuration to a JSON string.
    /// </summary>
    /// <returns>
    /// A JSON string representing this configuration.
    /// </returns>
    protected abstract string Serialize();

    /// <summary>
    /// Assigns <paramref name="value" /> to <paramref name="field" /> and invalidates
    /// the cached JSON so it is regenerated on the next serialization.
    /// </summary>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <param name="field">The backing field to update.</param>
    /// <param name="value">The value to assign.</param>
    protected void SetField<T>(ref T field, T value)
    {
        field = value;
        _json = null;
    }
}
