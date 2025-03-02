using UnrealPluginManager.Cli;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides an implementation of storage service for managing local storage specific to Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This service extends the functionality of <see cref="StorageServiceBase"/> by defining a base directory
/// for local file storage. The base directory is determined by:
/// 1. The value of the environment variable specified by <see cref="EnvironmentVariables.StorageDirectory"/>, if set.
/// 2. Otherwise, a default directory under the user's profile folder.
/// </remarks>
[AutoConstructor]
public partial class LocalStorageService : StorageServiceBase {
    private readonly IEnvironment _environment;

    /// <inheritdoc />
    public sealed override string BaseDirectory =>
        _environment.GetEnvironmentVariable(EnvironmentVariables.StorageDirectory) ??
        Path.Join(_environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".unrealpluginmanager");
}