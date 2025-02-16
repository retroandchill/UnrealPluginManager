using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Binding;
using UnrealPluginManager.Server.Controllers;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Utils;

/// <summary>
/// Provides utility methods for registering and configuring services within the application.
/// </summary>
public static class ServerServiceUtils {

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

    public static WebApplicationBuilder SetUpProductApplication(this WebApplicationBuilder builder) {
        // Add services to the container.

        builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApiConfigs()
            .AddSystemAbstractions()
            .AddServiceConfigs()
            .AddDbContext<UnrealPluginManagerContext, CloudUnrealPluginManagerContext>()
            .AddCoreServices()
            .AddServerServices()
            .AddControllers(options => {
                options.ModelBinderProviders.Insert(0, new PaginationModelBinderProvider());
            });

        builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = null);
        
        return builder;
    }

    public static WebApplication Configure(this WebApplication app) {
        app.Environment.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
        app.UseDefaultFiles();
        app.MapStaticAssets();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapFallbackToFile("/index.html");
        
        return app;
    }
}