using System.IO.Abstractions;
using LanguageExt;
using UnrealPluginManager.Cli.Config;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// RemoteService is a service that manages remote configurations, allowing for the retrieval, addition,
/// removal, and updating of remote entries. Each remote is stored as a key-value pair where the key is a name,
/// and the value is a URI.
/// </summary>
[AutoConstructor]
public partial class RemoteService : IRemoteService {
    private readonly IStorageService _storageService;

    private const string RemoteFile = "remotes.yaml";

    private static OrderedDictionary<string, RemoteConfig> DefaultRemote { get; } = new() {
        {
            "default", new Uri("https://localhost:7231")
        }
    };
    
    /// <inheritdoc />
    public Task<OrderedDictionary<string, RemoteConfig>> GetAllRemotes() {
        return _storageService.GetConfig(RemoteFile, DefaultRemote);
    }

    /// <inheritdoc />
    public async Task<Option<RemoteConfig>> GetRemote(string name) {
        var allRemotes = await GetAllRemotes();
        return allRemotes.TryGetValue(name, out var uri) ? uri : Option<RemoteConfig>.None;
    }

    /// <inheritdoc />
    public async Task AddRemote(string name, RemoteConfig uri) {
        var allRemotes = await GetAllRemotes();
        if (!allRemotes.TryAdd(name, uri)) {
            throw new ArgumentException($"Remote with name {name} already exists.");
        }
        await _storageService.SaveConfig(RemoteFile, allRemotes);
    }

    /// <inheritdoc />
    public async Task RemoveRemote(string name) {
        var allRemotes = await GetAllRemotes();
        if (!allRemotes.Remove(name)) {
            throw new ArgumentException($"Remote with name {name} does not exist.");
        }
        await _storageService.SaveConfig(RemoteFile, allRemotes);
    }

    /// <inheritdoc />
    public async Task UpdateRemote(string name, RemoteConfig uri) {
        var allRemotes = await GetAllRemotes();
        if (!allRemotes.ContainsKey(name)) {
            throw new ArgumentException($"Remote with name {name} does not exist.");
        }
        
        allRemotes[name] = uri;
        await _storageService.SaveConfig(RemoteFile, allRemotes);
    }
}