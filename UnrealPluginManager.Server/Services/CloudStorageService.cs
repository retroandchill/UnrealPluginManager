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
public class CloudStorageService : IStorageService {
    private readonly StorageMetadata _storageMetadata;
    private readonly IFileSystem _filesystem;

    /// <summary>
    /// Provides a cloud storage service implementation for managing plugin files.
    /// This service handles storing and retrieving plugins in a configurable cloud-based directory.
    /// </summary>
    public CloudStorageService(IFileSystem filesystem, IConfiguration config) {
        _filesystem = filesystem;
        _storageMetadata = new StorageMetadata();
        config.GetSection(StorageMetadata.Name).Bind(_storageMetadata);

        var directoryInfo = _filesystem.DirectoryInfo.New(_storageMetadata.BaseDirectory);
        if (!directoryInfo.Exists) {
            directoryInfo.Create();
        }
    }

    /// <inheritdoc/>
    public async Task<IFileInfo> StorePlugin(Stream fileData) {
        using var archive = new ZipArchive(fileData);

        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var fileName =
            $"{_filesystem.Path.GetFileNameWithoutExtension(archiveEntry.FullName)}{_filesystem.Path.GetRandomFileName()}.zip";
        var fullPath = _filesystem.Path.Combine(_storageMetadata.BaseDirectory, fileName);
        await using var fileStream = _filesystem.FileStream.New(fullPath, FileMode.Create);
        fileData.Seek(0, SeekOrigin.Begin);
        await fileData.CopyToAsync(fileStream);

        return _filesystem.FileInfo.New(fullPath);
    }

    /// <inheritdoc/>
    public Stream RetrievePlugin(IFileInfo fileInfo) {
        return fileInfo.OpenRead();
    }
}