using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Represents configuration options for services provided by HTMX Toolkit.
/// </summary>
public sealed class HtmxToolkitOptions
{
    private HtmxConfig? _config;

    /// <summary>
    /// Gets the configuration for the selected HTMX major version.
    /// </summary>
    /// <remarks>
    /// If no version has been explicitly configured, this property defaults to HTMX 2.x.
    /// </remarks>
    public HtmxConfig HtmxConfig
    {
        get
        {
            if (_config is null)
                UseHtmxV2();

            return _config;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether antiforgery request metadata is rendered
    /// by the configuration tag helper.
    /// Defaults to <see langword="true" />.
    /// </summary>
    public bool IncludeAntiforgeryToken { get; set; } = true;

    /// <summary>
    /// Gets the HTMX major version used for version-sensitive generated markup.
    /// </summary>
    public HtmxTargetVersion TargetVersion => HtmxConfig.TargetVersion;

    /// <summary>
    /// Selects and configures HTMX 1.x.
    /// </summary>
    /// <param name="configure">The optional delegate used to configure HTMX 1.x.</param>
    /// <returns>
    /// The current options instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// A different HTMX major version has already been selected.
    /// </exception>
    [MemberNotNull(nameof(_config))]
    public HtmxToolkitOptions UseHtmxV1(Action<HtmxV1Config>? configure = null)
    {
        var config = SelectVersion(HtmxTargetVersion.V1, static () => new HtmxV1Config());
        configure?.Invoke(config);
        return this;
    }

    /// <summary>
    /// Selects and configures HTMX 2.x.
    /// </summary>
    /// <param name="configure">The optional delegate used to configure HTMX 2.x.</param>
    /// <returns>
    /// The current options instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// A different HTMX major version has already been selected.
    /// </exception>
    [MemberNotNull(nameof(_config))]
    public HtmxToolkitOptions UseHtmxV2(Action<HtmxV2Config>? configure = null)
    {
        var config = SelectVersion(HtmxTargetVersion.V2, static () => new HtmxV2Config());
        configure?.Invoke(config);
        return this;
    }

    /// <summary>
    /// Selects and configures HTMX 4.x.
    /// </summary>
    /// <param name="configure">The optional delegate used to configure HTMX 4.x.</param>
    /// <returns>
    /// The current options instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// A different HTMX major version has already been selected.
    /// </exception>
    [MemberNotNull(nameof(_config))]
    public HtmxToolkitOptions UseHtmxV4(Action<HtmxV4Config>? configure = null)
    {
        var config = SelectVersion(HtmxTargetVersion.V4, static () => new HtmxV4Config());
        configure?.Invoke(config);
        return this;
    }

    /// <summary>
    /// Returns the selected version-specific HTMX configuration.
    /// </summary>
    /// <typeparam name="TConfig">The expected configuration type.</typeparam>
    /// <returns>
    /// The requested configuration instance.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="TConfig" /> does not match the selected HTMX major version.
    /// </exception>
    public TConfig GetHtmxConfig<TConfig>() where TConfig : HtmxConfig
    {
        if (HtmxConfig is TConfig config)
            return config;

        Error_ConfigTypeMismatch(TargetVersion);
        return null;
    }

    /// <summary>
    /// Selects an HTMX major version and registers its configuration instance.
    /// </summary>
    /// <typeparam name="TConfig">The version-specific configuration type.</typeparam>
    /// <param name="version">The HTMX major version to select.</param>
    /// <param name="factory">The factory used to create the configuration instance.</param>
    /// <returns>
    /// The configuration instance for the selected version.
    /// </returns>
    [MemberNotNull(nameof(_config))]
    private TConfig SelectVersion<TConfig>(HtmxTargetVersion version, Func<TConfig> factory) where TConfig : HtmxConfig
    {
        EnsureCanSelectVersion(version);

        _config ??= factory();

        Debug.Assert(_config.TargetVersion == version);
        Debug.Assert(
            version == HtmxTargetVersion.V1 && _config is HtmxV1Config
            || version == HtmxTargetVersion.V2 && _config is HtmxV2Config
            || version == HtmxTargetVersion.V4 && _config is HtmxV4Config);

        return (TConfig)_config;
    }

    /// <summary>
    /// Ensures that the specified HTMX version does not conflict with an already selected version.
    /// </summary>
    /// <param name="version">The HTMX target version to validate.</param>
    private void EnsureCanSelectVersion(HtmxTargetVersion version)
    {
        if (_config is { TargetVersion: var current })
            if (current != version)
                Error_ReconfigureTargetVersion(current, version);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when the requested HTMX
    /// configuration type does not match the configured target version.
    /// </summary>
    /// <param name="version">The current HTMX target version.</param>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_ConfigTypeMismatch(HtmxTargetVersion version) =>
        throw new InvalidOperationException($"HTMX configuration version '{version}' does not match the requested configuration type.");

    /// <summary>
    /// Throws an <see cref="InvalidOperationException" /> when attempting to reconfigure
    /// the HTMX target version.
    /// </summary>
    /// <param name="currentVersion">The version that has already been configured.</param>
    /// <param name="newVersion">The new version being attempted.</param>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_ReconfigureTargetVersion(HtmxTargetVersion currentVersion, HtmxTargetVersion newVersion) =>
        throw new InvalidOperationException($"HTMX has already been configured for version {currentVersion}. Cannot reconfigure to {newVersion}.");
}
