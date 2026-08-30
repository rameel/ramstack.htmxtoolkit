using Microsoft.Extensions.DependencyInjection;

namespace Ramstack.HtmxToolkit.Builder;

/// <summary>
/// Provides HTMX Toolkit service registration for an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and configures HTMX Toolkit services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional delegate used to configure HTMX Toolkit.</param>
    /// <returns>
    /// The same service collection.
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
