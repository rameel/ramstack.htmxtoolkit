using Microsoft.Extensions.DependencyInjection;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Hosting;

/// <summary>
/// Provides HtmxToolkit service registration for an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and configures HtmxToolkit services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The delegate used to configure HtmxToolkit.</param>
    /// <returns>
    /// The same <paramref name="services" /> collection.
    /// </returns>
    /// <remarks>
    /// The delegate must select the HTMX version with <see cref="HtmxToolkitOptions.UseHtmxV1" />,
    /// <see cref="HtmxToolkitOptions.UseHtmxV2" />, or <see cref="HtmxToolkitOptions.UseHtmxV4" />;
    /// otherwise, the application fails to start.
    /// </remarks>
    public static IServiceCollection AddHtmxToolkit(this IServiceCollection services, Action<HtmxToolkitOptions> configure)
    {
        services
            .AddOptions<HtmxToolkitOptions>()
            .Validate(
                static options => options.IsVersionSelected,
                HtmxToolkitOptions.VersionNotSelectedMessage)
            .ValidateOnStart();

        services.Configure(configure);

        return services;
    }
}
