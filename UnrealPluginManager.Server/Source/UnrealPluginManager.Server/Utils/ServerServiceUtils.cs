using System.Reflection;
using System.Text.Json;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Retro.SimplePage.Requests;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Binding;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Exceptions;
using UnrealPluginManager.Server.Formatters;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Utils;

/// <summary>
/// Provides utility methods for registering and configuring services within the application.
/// </summary>
public static class ServerServiceUtils {
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
        .AddExceptionHandler<ServerExceptionHandler>()
        .AddSingleton<IStorageService, CloudStorageService>()
        .AddSingleton<IJsonService>(provider => {
          var options = provider.GetRequiredService<IOptions<JsonOptions>>();
          return new JsonService(options.Value.JsonSerializerOptions);
        });
  }

  /// <summary>
  /// Configures the application for production use by applying common configuration,
  /// registering essential services, and configuring database contexts.
  /// </summary>
  /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
  /// <returns>The updated <see cref="WebApplicationBuilder"/> configured for production deployment.</returns>
  public static WebApplicationBuilder SetUpProductionApplication(this WebApplicationBuilder builder) {
    builder.SetUpCommonConfiguration();
    builder.Services.AddSystemAbstractions()
        .AddServiceConfigs()
        .AddDbContext<UnrealPluginManagerContext, CloudUnrealPluginManagerContext>();
    return builder;
  }

  /// <summary>
  /// Configures common settings and services for the application.
  /// </summary>
  /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance to configure.</param>
  /// <returns>The updated <see cref="WebApplicationBuilder"/> with common configurations applied.</returns>
  public static WebApplicationBuilder SetUpCommonConfiguration(this WebApplicationBuilder builder) {
    builder.Services.AddControllers(options => {
          options.ModelBinderProviders.Insert(0, new SemVersionModelBinderProvider());
          options.InputFormatters.Insert(0, new TextMarkdownInputFormatter());
          options.OutputFormatters.Insert(0, new TextMarkdownOutputFormatter());
        })
        .AddPagination()
        .AddJsonOptions(o => {
          o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
          o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
          o.JsonSerializerOptions.WriteIndented = true;
          o.JsonSerializerOptions.AllowTrailingCommas = true;
        });

    builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();
    builder.Services.AddKeycloakAdminHttpClient(builder.Configuration);

    builder.Services.AddCoreServices()
        .AddServerServices();

    builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = null);

    return builder;
  }

  /// <summary>
  /// Configures the application by applying middleware and mapping routes.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> instance to be configured.</param>
  /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
  public static WebApplication Configure(this WebApplication app) {
    app.Environment.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name ?? "MyApplication";
    app.UseDefaultFiles();
    app.MapStaticAssets();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRouting();
    app.MapControllers();
    app.MapFallbackToFile("/index.html");

    return app;
  }
}