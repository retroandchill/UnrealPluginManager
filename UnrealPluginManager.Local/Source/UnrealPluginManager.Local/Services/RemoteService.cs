﻿using LanguageExt;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Config;
using UnrealPluginManager.Local.Factories;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// RemoteService is a service that manages remote configurations, allowing for the retrieval, addition,
/// removal, and updating of remote entries. Each remote is stored as a key-value pair where the key is a name,
/// and the value is a URI.
/// </summary>
public class RemoteService : IRemoteService {
  private readonly IStorageService _storageService;
  private readonly OrderedDictionary<string, RemoteConfig> _remoteConfigs;
  private readonly Dictionary<Type, IApiClientFactory> _clientFactories;
  private readonly Dictionary<Type, Dictionary<string, IApiAccessor>> _apiAccessors;

  private const string RemoteFile = "remotes.json";

  private static OrderedDictionary<string, RemoteConfig> DefaultRemotes { get; } = new() {
      ["default"] = new Uri("https://localhost:7231")
  };

  /// Provides operations to manage remote configurations and API accessors
  /// within the Unreal Plugin Manager CLI. Manages storage and retrieval
  /// of remote configurations, and facilitates API communication through
  /// resolved accessors.
  public RemoteService(IStorageService storageService, IEnumerable<IApiClientFactory> clientFactories) {
    _storageService = storageService;
    _remoteConfigs = storageService.GetConfig(RemoteFile, DefaultRemotes);
    _clientFactories = clientFactories.ToDictionary(f => f.InterfaceType);
    _apiAccessors = _clientFactories.ToDictionary(f => f.Key,
                                                  f => _remoteConfigs.ToDictionary(x => x.Key,
                                                    x => f.Value.Create(x.Value)));
  }

  /// <inheritdoc />
  public string? DefaultRemoteName => _remoteConfigs.Keys.FirstOrDefault();

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

    foreach (var (type, factory) in _clientFactories) {
      _apiAccessors[type][name] = factory.Create(uri);
    }

    await _storageService.SaveConfigAsync(RemoteFile, _remoteConfigs);
  }

  /// <inheritdoc />
  public async Task RemoveRemote(string name) {
    if (!_remoteConfigs.Remove(name)) {
      throw new ArgumentException($"Remote with name {name} does not exist.");
    }

    foreach (var (type, _) in _clientFactories) {
      _apiAccessors[type].Remove(name);
    }

    await _storageService.SaveConfigAsync(RemoteFile, _remoteConfigs);
  }

  /// <inheritdoc />
  public async Task UpdateRemote(string name, RemoteConfig uri) {
    if (!_remoteConfigs.ContainsKey(name)) {
      throw new ArgumentException($"Remote with name {name} does not exist.");
    }

    _remoteConfigs[name] = uri;
    foreach (var (type, factory) in _clientFactories) {
      _apiAccessors[type][name] = factory.Create(uri);
    }

    await _storageService.SaveConfigAsync(RemoteFile, _remoteConfigs);
  }

  /// <inheritdoc />
  public T GetApiAccessor<T>(string name) where T : IApiAccessor {
    if (!_apiAccessors.TryGetValue(typeof(T), out var accessors)) {
      throw new ArgumentException($"No API accessor for type {typeof(T).Name}");
    }


    if (!accessors.TryGetValue(name, out var api)) {
      throw new ArgumentException($"No API configuration for remote {name}");
    }

    return (T)api;
  }

  /// <inheritdoc />
  public IEnumerable<Remote<T>> GetApiAccessors<T>() where T : IApiAccessor {
    if (!_apiAccessors.TryGetValue(typeof(T), out var accessors)) {
      throw new ArgumentException($"No API accessor for type {typeof(T).Name}");
    }

    return IterateOverApiAccessors<T>(accessors);
  }

  private static IEnumerable<Remote<T>> IterateOverApiAccessors<T>(Dictionary<string, IApiAccessor> accessors)
      where T : IApiAccessor {
    foreach (var (name, api) in accessors) {
      yield return new Remote<T>(name, (T)api);
    }
  }
}