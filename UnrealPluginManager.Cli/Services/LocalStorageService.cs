using System.IO.Abstractions;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Provides an implementation of storage service for managing local storage specific to Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This service extends the functionality of <see cref="StorageServiceBase"/> by defining a base directory
/// for local file storage. The base directory is determined by:
/// 1. The value of the environment variable specified by <see cref="EnvironmentVariables.StorageDirectory"/>, if set.
/// 2. Otherwise, a default directory under the user's profile folder.
/// </remarks>
/// <param name="fileSystem">
/// The file system abstraction, used for file operations in the service.
/// </param>
/// <param name="environment">
/// The environment abstraction, used to retrieve environment variables and user folder paths.
/// </param>
public class LocalStorageService(IFileSystem fileSystem, IEnvironment environment) : StorageServiceBase(fileSystem) {
    /// <inheritdoc />
    public sealed override string BaseDirectory =>
        environment.GetEnvironmentVariable(EnvironmentVariables.StorageDirectory) ??
        Path.Join(environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".unrealpluginmanager");
}