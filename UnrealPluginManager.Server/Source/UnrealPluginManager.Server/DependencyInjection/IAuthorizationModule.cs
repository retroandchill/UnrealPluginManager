using Jab;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Auth.Policies;
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
[Transient<IAuthorizationService, DefaultAuthorizationService>]
[Transient<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>]
[Transient<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>]
[Transient<IAuthorizationEvaluator, DefaultAuthorizationEvaluator>]
[Transient<IAuthorizationHandlerContextFactory, DefaultAuthorizationHandlerContextFactory>]
[Transient<IAuthorizationHandler, PassThroughAuthorizationHandler>]
[Transient<IPolicyEvaluator, PolicyEvaluator>]
[Transient<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>]
[Scoped<IApiKeyValidator, ApiKeyValidator>]
[Scoped<IPluginAuthValidator, PluginAuthValidator>]
[Transient<IAuthorizationHandler, CanSubmitPluginHandler>]
[Transient<IAuthorizationHandler, CanEditPluginHandler>]
[Transient<IAuthorizationHandler, CallingUserHandler>]
public interface IAuthorizationModule {
}