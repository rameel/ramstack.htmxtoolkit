using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents configuration options for services provided by the HTMX toolkit.
/// </summary>
public sealed class HtmxToolkitOptions
{
    private HtmxOptions? _options;

    /// <summary>
    /// Gets the configuration for the selected HTMX major version.
    /// </summary>
    /// <remarks>
    /// If no version has been explicitly configured, this property defaults to HTMX 2.x.
    /// </remarks>
    public HtmxOptions Htmx
    {
        get
        {
            if (_options is null)
                UseHtmxV2();

            return _options;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether antiforgery request metadata is rendered by the configuration tag helper.
    /// </summary>
    public bool IncludeAntiforgeryToken { get; set; }

    /// <summary>
    /// Gets the HTMX major version used for version-sensitive generated markup.
    /// </summary>
    public HtmxTargetVersion TargetVersion => Htmx.TargetVersion;

    /// <summary>
    /// Selects and configures HTMX 1.x.
    /// </summary>
    /// <param name="configure">The optional delegate used to configure HTMX 1.x.</param>
    /// <returns>
    /// The current options instance.
    /// </returns>
    [MemberNotNull(nameof(_options))]
    public HtmxToolkitOptions UseHtmxV1(Action<HtmxV1Options>? configure = null)
    {
        var options = SelectVersion(HtmxTargetVersion.V1, static () => new HtmxV1Options());
        configure?.Invoke(options);
        return this;
    }

    /// <summary>
    /// Selects and configures HTMX 2.x.
    /// </summary>
    /// <param name="configure">The optional delegate used to configure HTMX 2.x.</param>
    /// <returns>
    /// The current options instance.
    /// </returns>
    [MemberNotNull(nameof(_options))]
    public HtmxToolkitOptions UseHtmxV2(Action<HtmxV2Options>? configure = null)
    {
        var options = SelectVersion(HtmxTargetVersion.V2, static () => new HtmxV2Options());
        configure?.Invoke(options);
        return this;
    }

    /// <summary>
    /// Selects and configures HTMX 4.x.
    /// </summary>
    /// <param name="configure">The optional delegate used to configure HTMX 4.x.</param>
    /// <returns>
    /// The current options instance.
    /// </returns>
    [MemberNotNull(nameof(_options))]
    public HtmxToolkitOptions UseHtmxV4(Action<HtmxV4Options>? configure = null)
    {
        var options = SelectVersion(HtmxTargetVersion.V4, static () => new HtmxV4Options());
        configure?.Invoke(options);
        return this;
    }

    /// <summary>
    /// Returns the selected version-specific HTMX configuration,
    /// and throws an exception if the requested options type does not match the configured HTMX target version.
    /// </summary>
    /// <typeparam name="TOptions">The expected configuration type.</typeparam>
    /// <returns>
    /// The requested configuration instance.
    /// </returns>
    public TOptions GetHtmxOptions<TOptions>() where TOptions : HtmxOptions
    {
        if (Htmx is TOptions options)
            return options;

        Error_OptionsTypeMismatch(TargetVersion);
        return null;
    }

    /// <summary>
    /// Selects an HTMX major version and registers its configuration instance.
    /// </summary>
    /// <typeparam name="TOptions">The version-specific configuration type.</typeparam>
    /// <param name="version">The HTMX major version to select.</param>
    /// <param name="factory">The factory used to create the configuration instance.</param>
    /// <returns>
    /// The configuration instance for the selected version.
    /// </returns>
    [MemberNotNull(nameof(_options))]
    private TOptions SelectVersion<TOptions>(HtmxTargetVersion version, Func<TOptions> factory) where TOptions : HtmxOptions
    {
        EnsureCanSelectVersion(version);

        _options ??= factory();

        Debug.Assert(_options.TargetVersion == version);
        Debug.Assert(
            version == HtmxTargetVersion.V1 && _options is HtmxV1Options
            || version == HtmxTargetVersion.V2 && _options is HtmxV2Options
            || version == HtmxTargetVersion.V4 && _options is HtmxV4Options);

        return (TOptions)_options;
    }

    /// <summary>
    /// Ensures that the specified HTMX version does not conflict with an already selected version.
    /// </summary>
    /// <param name="version">The HTMX target version to validate.</param>
    private void EnsureCanSelectVersion(HtmxTargetVersion version)
    {
        if (_options is { TargetVersion: var current })
            if (current != version)
                Error_ReconfigureTargetVersion(current, version);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when the requested HTMX options type
    /// does not match the configured target version.
    /// </summary>
    /// <param name="version">The current HTMX target version.</param>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_OptionsTypeMismatch(HtmxTargetVersion version) =>
        throw new InvalidOperationException($"HTMX configuration version '{version}' does not match the requested options type.");

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when attempting to reconfigure the HTMX target version.
    /// </summary>
    /// <param name="currentVersion">The version that has already been configured.</param>
    /// <param name="newVersion">The new version being attempted.</param>
    /// <exception cref="InvalidOperationException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_ReconfigureTargetVersion(HtmxTargetVersion currentVersion, HtmxTargetVersion newVersion) =>
        throw new InvalidOperationException($"HTMX has already been configured for version {currentVersion}. Cannot reconfigure to {newVersion}.");
}
