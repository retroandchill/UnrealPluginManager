using Jab;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Server.Database;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Defines the interface for configuring cloud database dependencies
/// in the UnrealPluginManager application.
/// </summary>
/// <remarks>
/// This interface is used to register and manage scoped dependencies related
/// to database contexts in a cloud-based environment.
/// It includes specialized dependency injection support for cloud database
/// operations within the application.
/// </remarks>
[ServiceProviderModule]
[Scoped<CloudUnrealPluginManagerContext>]
[Scoped<UnrealPluginManagerContext>(Factory = nameof(GetUnrealPluginManagerContext))]
public interface ICloudDatabaseModule {
  /// Retrieves an instance of the CloudUnrealPluginManagerContext.
  /// <param name="unrealPluginManagerContext">An instance of CloudUnrealPluginManagerContext that will be returned by the method.</param>
  /// <return>An instance of CloudUnrealPluginManagerContext.</return>
  static CloudUnrealPluginManagerContext GetUnrealPluginManagerContext(
      CloudUnrealPluginManagerContext unrealPluginManagerContext) {
    return unrealPluginManagerContext;
  }
  
}