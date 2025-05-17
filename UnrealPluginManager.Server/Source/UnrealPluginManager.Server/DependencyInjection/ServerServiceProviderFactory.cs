using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// A factory class for creating and configuring the <see cref="IServiceProvider"/>
/// used for dependency injection in the server application.
/// </summary>
/// <remarks>
/// This factory ensures that the required services are added and properly configured
/// for the server's dependency injection container. It integrates with Jab extensions
/// and custom modules used in the server.
/// </remarks>
public class ServerServiceProviderFactory : IServiceProviderFactory<IServiceCollection> {
  /// <summary>
  /// Creates a builder for the <see cref="IServiceCollection"/> that can be used to configure
  /// services for the dependency injection container in the server application.
  /// </summary>
  /// <param name="services">
  /// The initial <see cref="IServiceCollection"/> to be configured.
  /// </param>
  /// <returns>
  /// The same <see cref="IServiceCollection"/> instance to allow for further configuration.
  /// </returns>
  public IServiceCollection CreateBuilder(IServiceCollection services) {
    return services;
  }

  /// <summary>
  /// Creates a service provider from the specified <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="containerBuilder">
  /// The <see cref="IServiceCollection"/> to build the service provider from.
  /// </param>
  /// <returns>
  /// An <see cref="IServiceProvider"/> instance created from the specified container builder.
  /// </returns>
  public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) {
    return containerBuilder
        .AddJabServices(p => new ServerServiceProvider(p))
        .ConfigureAuth()
        .BuildServiceProvider();
  }
}