﻿using LanguageExt;
using UnrealPluginManager.Cli.Config;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Services;

public record struct Remote<T>(string Name, T Api) where T : IApiAccessor;

/// <summary>
/// Represents a service for managing remote resources.
/// </summary>
public interface IRemoteService {

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
    
    Option<IReadableConfiguration> GetRemoteConfig(string name);
    
    T GetApiAccessor<T>(string name) where T : IApiAccessor;
    
    IEnumerable<Remote<T>> GetApiAccessors<T>() where T : IApiAccessor;

}

public static class RemoteServiceExtensions {
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