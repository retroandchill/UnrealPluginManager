using UnrealPluginManager.Local.Config;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Factories;

/// <summary>
/// A generic factory class for creating instances of API accessors based on a specified configuration.
/// </summary>
/// <typeparam name="TInterface">
/// The interface type of the API accessor. Must implement the <see cref="IApiAccessor"/> interface.
/// </typeparam>
/// <typeparam name="TImpl">
/// The concrete implementation type of the API accessor. Must inherit from <typeparamref name="TInterface"/>.
/// </typeparam>
public class ApiClientFactory<TInterface, TImpl> : IApiClientFactory<TInterface> where TInterface : IApiAccessor where TImpl : class, TInterface {

  /// <inheritdoc />
  public Type InterfaceType => typeof(TInterface);

  IApiAccessor IApiClientFactory.Create(RemoteConfig config) {
    return Create(config);
  }

  /// <inheritdoc />
  public TInterface Create(RemoteConfig config) {
    var constructor = typeof(TImpl).GetConstructor([typeof(string)]);
    ArgumentNullException.ThrowIfNull(constructor);
    return (TInterface) constructor.Invoke([config.Url.ToString()]);
  }
}