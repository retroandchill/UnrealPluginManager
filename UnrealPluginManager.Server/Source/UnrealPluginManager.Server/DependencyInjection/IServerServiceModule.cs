using Jab;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Defines a service provider module for configuring and injecting server-side
/// services used by the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// This module is annotated with dependency injection configuration attributes
/// to specify the scoped implementation of the <see cref="IUserService"/>
/// as <see cref="UserService"/>.
/// </remarks>
[ServiceProviderModule]
[Scoped<IUserService, UserService>]
[Scoped<IPluginUserService, PluginUserService>]
[Transient<IPluginOwnerService>(Factory = nameof(GetPluginUserService))]
public interface IServerServiceModule {
  /// <summary>
  /// Retrieves an instance of <see cref="IPluginUserService"/>.
  /// </summary>
  /// <param name="pluginUserService">An instance of <see cref="IPluginUserService"/> to be returned.</param>
  /// <returns>Returns the provided instance of <see cref="IPluginUserService"/>.</returns>
  static IPluginUserService GetPluginUserService(IPluginUserService pluginUserService) {
    return pluginUserService;
  }
}