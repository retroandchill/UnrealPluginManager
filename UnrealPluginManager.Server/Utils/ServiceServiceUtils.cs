using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Semver;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Services;
using UnrealPluginManager.Server.Swagger;

namespace UnrealPluginManager.Server.Utils;

/// <summary>
/// Provides utility methods for registering and configuring services within the application.
/// </summary>
public static class ServiceServiceUtils {

    private const string SemVerPattern =
        @"^(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)(?:-(?:(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?:[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";

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

    /// <summary>
    /// Configures and adds Swagger services to the application for API documentation generation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the Swagger services are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with Swagger configurations applied.</returns>
    public static IServiceCollection SetUpSwagger(this IServiceCollection services) {
        return services.AddSwaggerGen(options => {
            options.MapType<SemVersion>(() => new OpenApiSchema {
                Type = "string",
                Pattern = SemVerPattern,
                Example = new OpenApiString("1.0.0")
            });
            options.MapType<SemVersionRange>(() => new OpenApiSchema {
                Type = "string",
                Example = new OpenApiString(">=1.0.0")
            });
            options.AddSchemaFilterInstance(new CollectionPropertyFilter());
            options.AddOperationFilterInstance(new PageableParameterFilter());
            options.AddSchemaFilterInstance(new PagePropertyFilter());
        });
    }
}