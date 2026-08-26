namespace Ramstack.HtmxToolkit;

/// <summary>
/// Configures services provided by the HTMX toolkit.
/// </summary>
public sealed class HtmxToolkitOptions
{
    /// <summary>
    /// Gets or sets the HTMX major version used for version-sensitive generated markup.
    /// </summary>
    public HtmxTargetVersion TargetVersion { get; set; } = HtmxTargetVersion.V2;
}
