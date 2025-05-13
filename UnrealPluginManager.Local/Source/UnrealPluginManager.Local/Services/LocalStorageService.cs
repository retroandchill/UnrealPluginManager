using System.IO.Abstractions;
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
public class LocalStorageService : StorageServiceBase {

  /// <inheritdoc />
  public sealed override string BaseDirectory { get; }

  /// <inheritdoc />
  public sealed override string ResourceDirectory { get; }

  /// Provides functionality for local file storage, managing directories
  /// and resources required for the Unreal Plugin Manager. This service
  /// determines the base directory by retrieving an environment variable
  /// or defaulting to a user-specific location. Ensures necessary directory
  /// creation operations during initialization.
  public LocalStorageService(IEnvironment environment, IFileSystem fileSystem, IJsonService jsonService) : base(
      fileSystem, jsonService) {

    BaseDirectory = environment.GetEnvironmentVariable(EnvironmentVariables.StorageDirectory) ??
                    Path.Join(environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        ".unrealpluginmanager");
    ResourceDirectory = Path.Join(BaseDirectory, "resources");

    FileSystem.Directory.CreateDirectory(BaseDirectory);
    FileSystem.Directory.CreateDirectory(ResourceDirectory);
  }
}