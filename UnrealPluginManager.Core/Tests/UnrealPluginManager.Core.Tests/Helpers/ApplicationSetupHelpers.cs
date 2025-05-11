using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Core.Database;

namespace UnrealPluginManager.Core.Tests.Helpers;

/// <summary>
/// Provides helper methods for configuring application services in a test environment.
/// This class includes functionality for setting up mock service providers, such as
/// mock file systems and database contexts, in an IServiceCollection.
/// </summary>
public static class ApplicationSetupHelpers {
  /// <summary>
  /// Configures the service collection with mock data providers for use in testing environments.
  /// This method replaces default implementations with mock services such as a mock file system
  /// and test database context for UnrealPluginManager-related features.
  /// </summary>
  /// <param name="services">The IServiceCollection instance to configure with mock services.</param>
  /// <returns>The updated IServiceCollection with the mock data providers configured.</returns>
  public static IServiceCollection SetUpMockDataProviders<TDataContextType>(this IServiceCollection services)
      where TDataContextType : UnrealPluginManagerContext {
    return services.AddMockSystemAbstractions()
        .AddDbContext<UnrealPluginManagerContext, TDataContextType>(ServiceLifetime.Singleton,
            ServiceLifetime.Singleton);
  }
}