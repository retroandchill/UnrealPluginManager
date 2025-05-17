using System.IO.Abstractions;
using System.Text.Json;
using Jab;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database;
using UnrealPluginManager.Local.Factories;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.WebClient.Api;

namespace UnrealPluginManager.Local.DependencyInjection;

/// <summary>
/// Represents a local service provider module for dependency injection in the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// This interface is annotated with service provider attributes, defining the provisioning of various services
/// for local usage within the application. These services include scoped and singleton dependencies aimed at
/// managing plugins, storage, API interaction, and other core functionalities of the application.
/// </remarks>
[ServiceProviderModule]
[Import<ISystemAbstractionsModule>]
[Import<ICoreServicesModule>]
[Import<ILocalIoModule>]
[Import<ILocalDatabaseModule>]
[Import<IServerInteractionModule>]
[Import<IEngineInteractionModule>]
public interface ILocalServiceProviderModule;