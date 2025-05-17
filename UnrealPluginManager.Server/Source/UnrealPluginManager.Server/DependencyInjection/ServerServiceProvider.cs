using Jab;
using Microsoft.Extensions.Options;
using UnrealPluginManager.Core.DependencyInjection;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// The ServerServiceProvider class is a specialized service provider for managing dependency injection
/// and resolving services within the Unreal Plugin Manager server application. It serves as a
/// centralized platform for handling application-level dependencies, configuring modules, and
/// managing service lifetimes.
/// <remarks>
/// This class integrates various modules essential for the server application, including system abstractions,
/// authentication services, cloud database support, Keycloak client handling, and server-specific modules.
/// Custom configuration and lifecycle handling mechanisms are implemented for optimal service management.
/// </remarks>
/// </summary>
[ServiceProvider]
[Import<ISystemAbstractionsModule>]
[Import<IWebContextModule>]
[Import<IAuthModule>]
[Import<IServerIoModule>]
[Import<ICoreServicesModule>]
[Import<ICloudDatabaseModule>]
[Import<IKeycloakClientModule>]
[Import<IServerServiceModule>]
[Singleton<ServiceProviderWrapper>(Instance = nameof(RuntimeServiceProvider))]
[JabCopyExclude(typeof(IConfiguration), typeof(IHostEnvironment), typeof(HttpClient), typeof(ILogger<>),
                typeof(IOptions<>))]
public sealed partial class ServerServiceProvider(IServiceProvider runtimeServiceProvider) : IServerServiceProvider {
  private ServiceProviderWrapper RuntimeServiceProvider { get; } = new(runtimeServiceProvider);

  IServerServiceProvider.IScope IServerServiceProvider.CreateScope() {
    return CreateScope();
  }

  /// <summary>
  /// The Scope class represents a nested, scoped service provider within the Unreal Plugin Manager
  /// server application. It is designed to manage service lifetimes for specific contexts, providing
  /// dependency resolutions that are limited to the defined scope lifecycle.
  /// <remarks>
  /// This class implements the IServerServiceProvider.IScope interface, which includes IServiceProvider
  /// and IServiceScope, offering support for scoped dependency injection. It allows for encapsulated
  /// resource management and ensures proper disposal of resources tied to a specific service scope.
  /// </remarks>
  /// </summary>
  public sealed partial class Scope : IServerServiceProvider.IScope;
}