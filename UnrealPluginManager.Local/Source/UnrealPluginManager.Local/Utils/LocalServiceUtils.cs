using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Factories;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Utils;

/// <summary>
/// Provides utility methods to register CLI services into the dependency injection container
/// for platform-specific and general service implementations.
/// </summary>
public static class LocalServiceUtils {
  /// <summary>
  /// Registers CLI services into the dependency injection container. It configures platform-specific
  /// implementations based on the operating system and adds general service dependencies.
  /// </summary>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the CLI services are registered.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
  /// <exception cref="PlatformNotSupportedException">
  /// Thrown if the operating system is not Windows, macOS, or Linux.
  /// </exception>
  public static IServiceCollection AddLocalServices(this IServiceCollection services) {
    if (OperatingSystem.IsWindows()) {
      services.AddScoped<IRegistry, WindowsRegistry>()
          .AddScoped<IEnginePlatformService, WindowsEnginePlatformService>();
    } else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()) {
      services.AddScoped<IEnginePlatformService, PosixEnginePlatformService>();
    } else {
      throw new PlatformNotSupportedException("The given platform is not supported.");
    }

    var jsonSerializationOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    return services.AddScoped<IEngineService, EngineService>()
        .AddScoped<IStorageService, LocalStorageService>()
        .AddScoped<IRemoteService, RemoteService>()
        .AddScoped<IPluginManagementService, PluginManagementService>()
        .AddScoped<IInstallService, InstallService>()
        .AddSingleton<IJsonService>(_ => new JsonService(jsonSerializationOptions));
  }

  /// <summary>
  /// Registers a scoped factory for creating API accessor instances. This allows dependency injection
  /// to resolve instances of API accessors with a specified implementation type.
  /// </summary>
  /// <typeparam name="T">The type of the API accessor interface.</typeparam>
  /// <typeparam name="TImpl">The implementation type of the API accessor interface.</typeparam>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the accessor factory is added.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
  private static IServiceCollection AddApi<T, TImpl>(this IServiceCollection services)
      where T : IApiAccessor where TImpl : class, T {
    return services.AddSingleton<IApiClientFactory<T>, ApiClientFactory<T, TImpl>>();
  }

  /// <summary>
  /// Registers API factories into the dependency injection container. This method adds specific
  /// API accessor factories for plugins and storage operations.
  /// </summary>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the API factories are registered.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
  public static IServiceCollection AddApis(this IServiceCollection services) {
    return services.AddApi<IPluginsApi, PluginsApi>()
        .AddApi<IStorageApi, StorageApi>();
  }
}