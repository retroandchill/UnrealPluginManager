using Jab;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Local.Database;

namespace UnrealPluginManager.Local.DependencyInjection;

/// <summary>
/// Represents a module for handling dependency injection and configuration of local database contexts
/// specific to managing Unreal plugin data.
/// </summary>
[ServiceProviderModule]
[Scoped<UnrealPluginManagerContext>(Factory = nameof(GetUnrealPluginManagerContext))]
[Scoped<LocalUnrealPluginManagerContext>]
public interface ILocalDatabaseModule {
  /// <summary>
  /// Retrieves an instance of <see cref="LocalUnrealPluginManagerContext"/> for managing and accessing local Unreal plugin data.
  /// </summary>
  /// <param name="dbContext">
  /// An instance of <see cref="LocalUnrealPluginManagerContext"/> used for local database interactions.
  /// </param>
  /// <returns>
  /// The provided <see cref="LocalUnrealPluginManagerContext"/> instance.
  /// </returns>
  static LocalUnrealPluginManagerContext GetUnrealPluginManagerContext(LocalUnrealPluginManagerContext dbContext) {
    return dbContext;
  }
}