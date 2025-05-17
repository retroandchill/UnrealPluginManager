
using Jab;
using UnrealPluginManager.Local.Factories;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.WebClient.Api;

namespace UnrealPluginManager.Local.DependencyInjection;

/// <summary>
/// Defines the contract for the server interaction module required for managing
/// core functionalities related to communication with remote services within
/// the Unreal Plugin Manager ecosystem.
/// </summary>
/// <remarks>
/// This interface facilitates the integration of client-server interactions,
/// by providing required dependencies and implementations, ensuring proper
/// configuration, and allowing extensibility for remote services and API management.
/// </remarks>
/// <example>
/// The interface automatically integrates with dependency injection, where specific
/// implementations and lifetimes are defined. It includes:
/// - Singleton configurations for HTTP client and API client factories.
/// - Scoped services for interacting with remote systems.
/// </example>
/// <seealso cref="IApiClientFactory"/>
/// <seealso cref="IRemoteService"/>
/// <seealso cref="PluginsApi"/>
[ServiceProviderModule]
[Singleton<HttpClient>]
[Singleton<IApiClientFactory, ApiClientFactory<IPluginsApi, PluginsApi>>]
[Scoped<IRemoteService, RemoteService>]
public interface IServerInteractionModule;