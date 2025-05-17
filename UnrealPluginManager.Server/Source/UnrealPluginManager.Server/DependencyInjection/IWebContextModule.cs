using Jab;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using UnrealPluginManager.Server.Exceptions;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Represents a dependency injection module for configuring services within a web context.
/// </summary>
/// <remarks>
/// This interface is decorated with various attributes to provide singleton service registrations
/// and factories for resolving specific dependencies commonly used in web applications.
/// It includes configurations for services such as <see cref="IHttpContextAccessor"/>,
/// <see cref="IExceptionHandler"/>, <see cref="IConfiguration"/>, <see cref="IHostEnvironment"/>,
/// and <see cref="ILogger{T}"/>.
/// </remarks>
[ServiceProviderModule]
[Singleton<IHttpContextAccessor, HttpContextAccessor>]
[Singleton<IExceptionHandler, ServerExceptionHandler>]
[Singleton<IConfiguration>(Factory = nameof(GetConfiguration))]
[Singleton<IHostEnvironment>(Factory = nameof(GetHostEnvironment))]
[Singleton(typeof(ILogger<>), Factory = nameof(GetLogger))]
[Singleton(typeof(IOptions<>), Factory = nameof(GetOptions))]
public interface IWebContextModule {
  /// Retrieves an `IConfiguration` instance from the provided `ServiceProviderWrapper`.
  /// <param name="serviceProvider">
  /// The `ServiceProviderWrapper` instance used to obtain the `IConfiguration` service.
  /// </param>
  /// <return>
  /// Returns the `IConfiguration` instance resolved from the service provider.
  /// </return>
  static IConfiguration GetConfiguration(ServiceProviderWrapper serviceProvider) {
    return serviceProvider.GetRequiredService<IConfiguration>();
  }

  /// Retrieves an `IHostEnvironment` instance from the provided `ServiceProviderWrapper`.
  /// <param name="serviceProvider">
  /// The `ServiceProviderWrapper` instance used to obtain the `IHostEnvironment` service.
  /// </param>
  /// <return>
  /// Returns the `IHostEnvironment` instance resolved from the service provider.
  /// </return>
  static IHostEnvironment GetHostEnvironment(ServiceProviderWrapper serviceProvider) {
    return serviceProvider.GetRequiredService<IHostEnvironment>();
  }

  /// Resolves and retrieves an `ILogger<T>` instance using the provided `ServiceProviderWrapper`.
  /// <param name="serviceProvider">
  /// The `ServiceProviderWrapper` instance used to obtain the `ILogger<T>` service.
  /// </param>
  /// <return>
  /// Returns the `ILogger<T>` instance resolved from the service provider.
  /// </return>
  static ILogger<T> GetLogger<T>(ServiceProviderWrapper serviceProvider) {
    return serviceProvider.GetRequiredService<ILogger<T>>();
  }

  static IOptions<T> GetOptions<T>(ServiceProviderWrapper serviceProvider) where T : class {
    return serviceProvider.GetRequiredService<IOptions<T>>();
  }
}