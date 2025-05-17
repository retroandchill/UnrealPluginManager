using Jab;
using Microsoft.AspNetCore.Authentication;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Represents a module responsible for defining authentication services and dependencies
/// used within the server application context.
/// </summary>
/// <remarks>
/// The IAuthenticationModule interface is primarily utilized to register and configure
/// authentication-related services. This includes the setup of authentication handlers,
/// schemes, and claims transformations to ensure proper handling of authentication flows
/// within the application. It acts as a central point for authentication functionality and
/// integrates with other service modules.
/// </remarks>
[ServiceProviderModule]
[Scoped<IAuthenticationService, AuthenticationService>]
[Singleton<IClaimsTransformation, NoopClaimsTransformation>]
[Scoped<IAuthenticationHandlerProvider, AuthenticationHandlerProvider>]
[Scoped<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>]
public interface IAuthenticationModule;