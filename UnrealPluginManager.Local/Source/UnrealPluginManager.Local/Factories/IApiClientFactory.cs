using UnrealPluginManager.Local.Config;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Factories;

/// <summary>
/// Provides an interface for factory classes responsible for creating instances of API accessors
/// for a specific service type. The created API accessor instances are configured based on
/// the given remote configuration, enabling interaction with the associated service.
/// </summary>
public interface IApiClientFactory {
  /// <summary>
  /// Gets the type of the interface that an API client factory is responsible for creating.
  /// </summary>
  Type InterfaceType { get; }

  /// <summary>
  /// Creates an instance of an API accessor configured using the provided remote configuration.
  /// This method leverages the factory's implementation to establish instances of API accessors
  /// tailored to interact with specific remote services.
  /// </summary>
  /// <param name="baseUrl">The remote configuration providing the base URL and optional credentials for the API accessor.</param>
  /// <returns>An instance of <see cref="IApiAccessor"/> configured to interact with the designated remote service.</returns>
  IApiAccessor Create(RemoteConfig baseUrl);
  
}

/// <summary>
/// Defines a factory interface for creating typed instances of API accessors that interact
/// with remote services. Provides a method to create API accessors configured based
/// on the specified remote configuration.
/// </summary>
/// <typeparam name="T">
/// The type of API accessor that this factory is responsible for creating. Must implement the <see cref="IApiAccessor"/> interface.
/// </typeparam>
public interface IApiClientFactory<out T> : IApiClientFactory where T : IApiAccessor {

  /// <summary>
  /// Creates an instance of the API accessor configured using the provided remote configuration.
  /// This method ensures the generated API accessor is prepared to interact with the specified service using the given configuration.
  /// </summary>
  /// <param name="config">The remote configuration containing the necessary details, such as the base URL and credentials for the API accessor.</param>
  /// <returns>An instance of the API accessor type parameter <typeparamref name="T"/> tailored to communicate with the configured service.</returns>
  new T Create(RemoteConfig config);

}