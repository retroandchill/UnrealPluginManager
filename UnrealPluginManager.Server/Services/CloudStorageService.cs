using System.IO.Abstractions;
using System.IO.Compression;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Config;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Represents a storage service for managing cloud-based plugin datastore operations.
/// Implements methods to store and retrieve plugin files.
/// </summary>
public class CloudStorageService : StorageServiceBase {
    private readonly StorageMetadata _storageMetadata;

    public sealed override string BaseDirectory => _storageMetadata.BaseDirectory;

    /// <summary>
    /// Provides a cloud storage service implementation for managing plugin files.
    /// This service handles storing and retrieving plugins in a configurable cloud-based directory.
    /// </summary>
    public CloudStorageService(IFileSystem filesystem, IConfiguration config) : base(filesystem) {
        _storageMetadata = new StorageMetadata();
        config.GetSection(StorageMetadata.Name).Bind(_storageMetadata);

        var directoryInfo = FileSystem.DirectoryInfo.New(_storageMetadata.BaseDirectory);
        if (!directoryInfo.Exists) {
            directoryInfo.Create();
        }
    }
}