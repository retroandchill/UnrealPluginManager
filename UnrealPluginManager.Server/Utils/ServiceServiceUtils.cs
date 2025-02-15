using Microsoft.AspNetCore.Http.Features;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Utils;

/// <summary>
/// Provides utility methods for registering and configuring services within the application.
/// </summary>
public static class ServiceServiceUtils {

    /// <summary>
    /// Configures and registers OpenAPI services for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the OpenAPI services are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with OpenAPI configurations applied.</returns>
    public static IServiceCollection AddOpenApiConfigs(this IServiceCollection services) {
        return services.AddOpenApi()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    /// <summary>
    /// Configures and applies service-specific configurations to the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the service configurations are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the service configurations applied.</returns>
    public static IServiceCollection AddServiceConfigs(this IServiceCollection services) {
        return services.Configure<FormOptions>(options => {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = long.MaxValue;
        });
    }

    /// <summary>
    /// Registers server-specific services within the application's service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where the server services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> containing the registered server services.</returns>
    public static IServiceCollection AddServerServices(this IServiceCollection services) {
        return services
            .AddScoped<IStorageService, CloudStorageService>();
    }
    
}