using Jab;
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
public interface IServerServiceModule;