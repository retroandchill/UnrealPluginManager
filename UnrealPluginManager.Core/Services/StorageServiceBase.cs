using System.IO.Abstractions;
using System.IO.Compression;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides a base implementation for storage services managing plugin files.
/// This abstract class handles common functionality for storage operations such as
/// storing and retrieving plugin files while relying on derived classes for
/// implementation of the base directory logic.
/// </summary>
[AutoConstructor]
public abstract partial class StorageServiceBase : IStorageService {
    /// <summary>
    /// Provides access to the file system abstraction used by storage services.
    /// This property is used to perform file system operations without directly
    /// depending on the standard System.IO classes, facilitating unit testing
    /// and custom implementations.
    /// </summary>
    [field: AutoConstructorInject(initializer: "filesystem", 
        injectedType: typeof(IFileSystem), parameterName: "filesystem")]
    protected IFileSystem FileSystem { get; }

    /// <inheritdoc />
    public abstract string BaseDirectory { get; }

    private string PluginDirectory => Path.Join(BaseDirectory, "Plugins");

    /// <inheritdoc />
    public async Task<IFileInfo> StorePlugin(Stream fileData) {
        using var archive = new ZipArchive(fileData);

        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var fileName =
            $"{Path.GetFileNameWithoutExtension(archiveEntry.FullName)}{Path.GetRandomFileName()}.zip";
        Directory.CreateDirectory(PluginDirectory);
        var fullPath = Path.Combine(PluginDirectory, fileName);
        FileSystem.Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var fileStream = FileSystem.FileStream.New(fullPath, FileMode.Create);
        fileData.Seek(0, SeekOrigin.Begin);
        await fileData.CopyToAsync(fileStream);

        return FileSystem.FileInfo.New(fullPath);
    }

    /// <inheritdoc />
    public Stream RetrievePlugin(IFileInfo fileInfo) {
        return fileInfo.OpenRead();
    }
}