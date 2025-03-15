using System.IO.Abstractions;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Config;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Represents a storage service for managing cloud-based plugin datastore operations.
/// Implements methods to store and retrieve plugin files.
/// </summary>
public class CloudStorageService : StorageServiceBase {

  /// <inheritdoc />
  public sealed override string BaseDirectory { get; }

  /// <inheritdoc />
  public sealed override string ResourceDirectory { get; }

  /// <summary>
  /// Provides a cloud storage service implementation for managing plugin files.
  /// This service handles storing and retrieving plugins in a configurable cloud-based directory.
  /// </summary>
  public CloudStorageService(IFileSystem filesystem, IConfiguration config, IJsonService jsonService)
      : base(filesystem, jsonService) {
    var storageMetadata = new StorageMetadata();
    config.GetSection(StorageMetadata.Name).Bind(storageMetadata);

    var directoryInfo =
        FileSystem.DirectoryInfo.New(Path.GetFullPath(storageMetadata.BaseDirectory,
            FileSystem.Directory.GetCurrentDirectory()));
    if (!directoryInfo.Exists) {
      directoryInfo.Create();
    }

    var resourceDirectory = Path.Combine(directoryInfo.FullName, storageMetadata.ResourceDirectory);
    if (!FileSystem.Directory.Exists(resourceDirectory)) {
      FileSystem.Directory.CreateDirectory(resourceDirectory);
    }
  }
}