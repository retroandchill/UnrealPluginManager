using System.Reflection;
using System.Text.Json;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Retro.SimplePage.Requests;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Auth.Policies;
using UnrealPluginManager.Server.Binding;
using UnrealPluginManager.Server.DependencyInjection;
using UnrealPluginManager.Server.Formatters;

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
  /// Configures the application for production use by applying common configuration,
  /// registering essential services, and configuring database contexts.
  /// </summary>
  /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
  /// <returns>The updated <see cref="WebApplicationBuilder"/> configured for production deployment.</returns>
  public static WebApplicationBuilder SetUpProductionApplication(this WebApplicationBuilder builder) {
    builder.SetUpCommonConfiguration();
    builder.Services.AddProblemDetails()
        .AddServiceConfigs()
        .AddJabServices();
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


    builder.Services.AddDistributedMemoryCache()
        .AddClientCredentialsTokenManagement()
        .AddClient(
            "Keycloak",
            client => {
              var options = builder.Configuration.GetKeycloakOptions<KeycloakAdminClientOptions>()!;

              client.ClientId = options.Resource;
              client.ClientSecret = options.Credentials.Secret;
              client.TokenEndpoint = options.KeycloakTokenEndpoint;
            }
        );

    const string smartScheme = "Smart";

    builder.Services.AddAuthentication(options => {
          options.DefaultScheme = smartScheme;
          options.DefaultChallengeScheme = smartScheme;
        })
        .AddPolicyScheme(smartScheme, "Authorize JWT or API Key", options => {
          options.ForwardDefaultSelector = context => {
            var authHeader = context.Request.Headers.Authorization;
            if (authHeader.ToString().StartsWith("Bearer ")) {
              return JwtBearerDefaults.AuthenticationScheme;
            }

            return context.Request.Headers.TryGetValue(ApiKeyAuth.HeaderName, out _)
                ? AuthenticationSchemes.ApiKey
                : JwtBearerDefaults.AuthenticationScheme;

          };
        })
        .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(AuthenticationSchemes.ApiKey, _ => { })
        .AddKeycloakWebApi(builder.Configuration);
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy(AuthorizationPolicies.CanSubmitPlugin, policy =>
            policy.Requirements.Add(new CanSubmitPluginRequirement()))
        .AddPolicy(AuthorizationPolicies.CanEditPlugin, policy =>
            policy.Requirements.Add(new CanEditPluginRequirement()))
        .AddPolicy(AuthorizationPolicies.CallingUser, policy =>
            policy.Requirements.Add(new CallingUserRequirement()));
    builder.Services.AddKeycloakAdminHttpClient(builder.Configuration)
        .AddClientCredentialsTokenHandler("Keycloak");

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
    app.UseExceptionHandler();

    app.UseDefaultFiles();
    app.MapStaticAssets();
    app.UseHttpsRedirection();
    app.UsePathBase("/api");
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapFallbackToFile("/index.html");

    return app;
  }
}