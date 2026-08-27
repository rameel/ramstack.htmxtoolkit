using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents configuration for a specific major version of HTMX.
/// </summary>
public abstract class HtmxConfig
{
    /// <summary>
    /// Gets the configured HTMX major version.
    /// </summary>
    [JsonIgnore]
    public HtmxTargetVersion TargetVersion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxConfig"/> class with the specified target HTMX version.
    /// </summary>
    /// <param name="version">The target major version of HTMX that this configuration applies to.</param>
    internal HtmxConfig(HtmxTargetVersion version) =>
        TargetVersion = version;
}
