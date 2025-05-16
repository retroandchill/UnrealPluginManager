using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Server.Database;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Represents a factory class responsible for creating instances of
/// <see cref="CloudUnrealPluginManagerContext"/>.
/// </summary>
/// <remarks>
/// This factory is designed to facilitate dependency injection for cloud-based
/// database contexts in the UnrealPluginManager server application.
/// </remarks>
public class CloudDatabaseFactory : IDatabaseFactory<CloudUnrealPluginManagerContext> {

  /// <inheritdoc />
  public CloudUnrealPluginManagerContext Create(IServiceProvider serviceProvider) {
    return new CloudUnrealPluginManagerContext(serviceProvider.GetRequiredService<IConfiguration>());
  }
}