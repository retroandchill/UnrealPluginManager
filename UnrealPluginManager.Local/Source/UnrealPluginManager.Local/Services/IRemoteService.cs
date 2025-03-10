using LanguageExt;
using UnrealPluginManager.Local.Config;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Represents a remote resource with a specific name and API accessor.
/// </summary>
/// <typeparam name="T">The type of the API accessor, constrained to implement <see cref="IApiAccessor"/>.</typeparam>
/// <param name="Name">The name of the remote</param>
/// <param name="Api">The API accessor</param>
public record struct Remote<T>(string Name, T Api) where T : IApiAccessor;

/// <summary>
/// Represents a service for managing remote resources.
/// </summary>
public interface IRemoteService {
  
  public string? DefaultRemoteName { get; }
  
  /// <summary>
  /// Retrieves all configured remotes.
  /// </summary>
  /// <returns>A task representing the asynchronous operation. The task result contains a dictionary mapping remote names to their respective URIs.</returns>
  OrderedDictionary<string, RemoteConfig> GetAllRemotes();

  /// <summary>
  /// Retrieves the URI of a specified remote by its name.
  /// </summary>
  /// <param name="name">The name of the remote to retrieve.</param>
  /// <returns>A task representing the asynchronous operation. The task result contains an optional URI representing the remote address, or none if the remote does not exist.</returns>
  Option<RemoteConfig> GetRemote(string name);

  /// <summary>
  /// Adds a new remote with the specified name and URI.
  /// </summary>
  /// <param name="name">The name of the remote to be added.</param>
  /// <param name="uri">The URI associated with the remote.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task AddRemote(string name, RemoteConfig uri);

  /// <summary>
  /// Removes the remote configuration with the specified name.
  /// </summary>
  /// <param name="name">The name of the remote to be removed.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task RemoveRemote(string name);

  /// <summary>
  /// Updates the URI of an existing remote with the specified name.
  /// </summary>
  /// <param name="name">The name of the remote to be updated.</param>
  /// <param name="uri">The new URI to associate with the remote.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task UpdateRemote(string name, RemoteConfig uri);

  /// <summary>
  /// Retrieves the remote configuration associated with the specified remote name, if available.
  /// </summary>
  /// <param name="name">The name of the remote for which the configuration is being retrieved.</param>
  /// <returns>An option containing the readable configuration if the remote is found; otherwise, an empty option.</returns>
  Option<IReadableConfiguration> GetRemoteConfig(string name);

  /// <summary>
  /// Retrieves an API accessor for a specified remote.
  /// </summary>
  /// <typeparam name="T">The type of the API accessor, which must implement <see cref="IApiAccessor"/>.</typeparam>
  /// <param name="name">The name of the remote for which the API accessor is requested.</param>
  /// <returns>An instance of the requested API accessor configured for the specified remote.</returns>
  T GetApiAccessor<T>(string name) where T : IApiAccessor;

  /// <summary>
  /// Retrieves all API accessors of a specified type for the configured remotes.
  /// </summary>
  /// <typeparam name="T">The type of the API accessor, constrained to implement <see cref="IApiAccessor"/>.</typeparam>
  /// <returns>An enumerable collection of remotes, each containing a name and the corresponding API accessor.</returns>
  IEnumerable<Remote<T>> GetApiAccessors<T>() where T : IApiAccessor;
}

/// <summary>
/// Provides extension methods for <see cref="IRemoteService"/> and its associated operations.
/// </summary>
public static class RemoteServiceExtensions {
  /// <summary>
  /// Converts a collection of remotes into an ordered dictionary, mapping the remote names to values retrieved by the provided asynchronous selector function.
  /// </summary>
  /// <typeparam name="T">The type of the API accessor, constrained to implement <see cref="IApiAccessor"/>.</typeparam>
  /// <typeparam name="TValue">The type of the value that will be associated with each remote name in the resulting dictionary.</typeparam>
  /// <param name="source">The collection of remotes to process.</param>
  /// <param name="selector">An asynchronous function that retrieves the value for a given API accessor.</param>
  /// <returns>A task representing the asynchronous operation. The task result is an <see cref="OrderedDictionary{TKey, TValue}"/> mapping remote names to their respective selected values.</returns>
  public static async Task<OrderedDictionary<string, TValue>> ToOrderedDictionaryAsync<T, TValue>(
      this IEnumerable<Remote<T>> source,
      Func<T, Task<TValue>> selector) where T : IApiAccessor {
    OrderedDictionary<string, TValue> result = new();
    foreach (var remote in source) {
      result.Add(remote.Name, await selector(remote.Api));
    }

    return result;
  }
}