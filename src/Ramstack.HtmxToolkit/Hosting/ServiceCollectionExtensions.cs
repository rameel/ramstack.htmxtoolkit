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
    /// <param name="configure">An optional delegate used to configure HtmxToolkit.</param>
    /// <returns>
    /// The same <paramref name="services" /> collection.
    /// </returns>
    public static IServiceCollection AddHtmxToolkit(this IServiceCollection services, Action<HtmxToolkitOptions>? configure = null)
    {
        services
            .AddOptions<HtmxToolkitOptions>()
            .ValidateOnStart();

        if (configure is not null)
            services.Configure(configure);

        return services;
    }
}
