using System.IO.Abstractions;
using LanguageExt;
using UnrealPluginManager.Cli.Config;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// RemoteService is a service that manages remote configurations, allowing for the retrieval, addition,
/// removal, and updating of remote entries. Each remote is stored as a key-value pair where the key is a name,
/// and the value is a URI.
/// </summary>
public class RemoteService : IRemoteService {
    private readonly IStorageService _storageService;
    private readonly OrderedDictionary<string, RemoteConfig> _remoteConfigs;
    private readonly OrderedDictionary<string, Configuration> _apiConfigs;
    private readonly Dictionary<Type, IApiAccessor> _apiAccessors;

    private const string RemoteFile = "remotes.yaml";

    private static OrderedDictionary<string, RemoteConfig> DefaultRemote { get; } = new() {
        {
            "default", new Uri("https://localhost:7231")
        }
    };

    /// Provides operations to manage remote configurations and API accessors
    /// within the Unreal Plugin Manager CLI. Manages storage and retrieval
    /// of remote configurations, and facilitates API communication through
    /// resolved accessors.
    public RemoteService(IApiTypeResolver typeResolver, IStorageService storageService, IEnumerable<IApiAccessor> apiAccessors) {
        _storageService = storageService;
        _remoteConfigs = storageService.GetConfig(RemoteFile, DefaultRemote);
        _apiConfigs = _remoteConfigs.ToOrderedDictionary(x => new Configuration {
            BasePath = x.Url.ToString()
        });
        _apiAccessors = apiAccessors.ToDictionary(typeResolver.GetInterfaceType);
    }

    /// <inheritdoc />
    public OrderedDictionary<string, RemoteConfig> GetAllRemotes() {
        return _remoteConfigs;
    }

    /// <inheritdoc />
    public Option<RemoteConfig> GetRemote(string name) {
        return _remoteConfigs.TryGetValue(name, out var uri) ? uri : Option<RemoteConfig>.None;
    }

    /// <inheritdoc />
    public async Task AddRemote(string name, RemoteConfig uri) {
        if (!_remoteConfigs.TryAdd(name, uri)) {
            throw new ArgumentException($"Remote with name {name} already exists.");
        }

        await _storageService.SaveConfigAsync(RemoteFile, _remoteConfigs);
    }

    /// <inheritdoc />
    public async Task RemoveRemote(string name) {
        if (!_remoteConfigs.Remove(name)) {
            throw new ArgumentException($"Remote with name {name} does not exist.");
        }

        await _storageService.SaveConfigAsync(RemoteFile, _remoteConfigs);
    }

    /// <inheritdoc />
    public async Task UpdateRemote(string name, RemoteConfig uri) {
        if (!_remoteConfigs.ContainsKey(name)) {
            throw new ArgumentException($"Remote with name {name} does not exist.");
        }

        _remoteConfigs[name] = uri;
        await _storageService.SaveConfigAsync(RemoteFile, _remoteConfigs);
    }

    /// <inheritdoc />
    public Option<IReadableConfiguration> GetRemoteConfig(string name) {
        return _apiConfigs.TryGetValue(name, out var config) ? config : Option<IReadableConfiguration>.None;
    }

    /// <inheritdoc />
    public T GetApiAccessor<T>(string name) where T : IApiAccessor {
        if (!_apiAccessors.TryGetValue(typeof(T), out var accessor)) {
            throw new ArgumentException($"No API accessor for type {nameof(T)}");
        }


        if (!_apiConfigs.TryGetValue(name, out var config)) {
            throw new ArgumentException($"No API configuration for remote {name}");
        }
        
        accessor.Configuration = config;
        return (T) accessor;
    }

    /// <inheritdoc />
    public IEnumerable<Remote<T>> GetApiAccessors<T>() where T : IApiAccessor {
        if (!_apiAccessors.TryGetValue(typeof(T), out var accessor) || accessor is not T) {
            throw new ArgumentException($"No API accessor for type {nameof(T)}");
        }

        return GetApiAccessorsIterator((T) accessor);
    }

    private IEnumerable<Remote<T>> GetApiAccessorsIterator<T>(T accessor) where T : IApiAccessor {
        foreach (var (name, config) in _apiConfigs) {
            accessor.Configuration = config;
            yield return new Remote<T>(name, accessor);
        }
    }
}