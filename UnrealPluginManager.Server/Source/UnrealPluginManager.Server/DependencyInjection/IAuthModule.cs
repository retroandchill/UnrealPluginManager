using Jab;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Auth.Validators;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Defines an authentication module for managing authentication and authorization services
/// related to API key validation and plugin authentication validation.
/// </summary>
/// <remarks>
/// This interface serves as a Dependency Injection module and is used to register
/// authentication-related services such as <see cref="IApiKeyValidator"/> and
/// <see cref="IPluginAuthValidator"/> with the service provider.
/// </remarks>
/// <seealso cref="IApiKeyValidator"/>
/// <seealso cref="IPluginAuthValidator"/>
[ServiceProviderModule]
[Scoped<IApiKeyValidator, ApiKeyValidator>]
[Scoped<IPluginAuthValidator, PluginAuthValidator>]
public interface IAuthModule;