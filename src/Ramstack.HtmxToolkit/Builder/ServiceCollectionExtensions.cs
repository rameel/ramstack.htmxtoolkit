using Microsoft.Extensions.DependencyInjection;

namespace Ramstack.HtmxToolkit.Builder;

/// <summary>
/// Provides registration methods for HTMX Toolkit services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers HTMX Toolkit configuration and its startup configuration cache.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An optional delegate used to configure HTMX Toolkit.</param>
    /// <returns>
    /// The same service collection.
    /// </returns>
    public static IServiceCollection AddHtmxToolkit(this IServiceCollection services, Action<HtmxToolkitOptions>? configure = null)
    {
        services.AddOptions<HtmxToolkitOptions>();

        if (configure is not null)
            services.Configure(configure);

        return services;
    }
}
