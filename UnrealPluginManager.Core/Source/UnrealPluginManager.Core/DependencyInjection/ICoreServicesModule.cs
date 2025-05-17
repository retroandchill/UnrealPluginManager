using Jab;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.DependencyInjection;

/// <summary>
/// Represents a module that provides core services for the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// The ICoreServicesModule interface is designed as a service provider module that registers and manages
/// scoped services related to plugin structure and plugin operations. It primarily handles the dependency
/// injection setup for services such as IPluginStructureService and IPluginService.
/// </remarks>
/// <remarks>
/// This module ensures that the required core services are available and properly scoped within
/// the application's runtime dependency injection container.
/// </remarks>
[ServiceProviderModule]
[Scoped<IPluginStructureService, PluginStructureService>]
[Scoped<IPluginService, PluginService>]
public interface ICoreServicesModule;