using System.IO.Abstractions;
using System.IO.Compression;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Core.Services;

public abstract class StorageServiceBase(IFileSystem fileSystem) : IStorageService {
    
    protected IFileSystem FileSystem => fileSystem;
    
    public abstract string BaseDirectory { get; }

    private string PluginDirectory => Path.Join(BaseDirectory, "Plugins");

    public async Task<IFileInfo> StorePlugin(Stream fileData) {
        using var archive = new ZipArchive(fileData);

        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var fileName =
            $"{fileSystem.Path.GetFileNameWithoutExtension(archiveEntry.FullName)}{fileSystem.Path.GetRandomFileName()}.zip";
        var fullPath = fileSystem.Path.Combine(PluginDirectory, fileName);
        await using var fileStream = fileSystem.FileStream.New(fullPath, FileMode.Create);
        fileData.Seek(0, SeekOrigin.Begin);
        await fileData.CopyToAsync(fileStream);

        return fileSystem.FileInfo.New(fullPath);
    }

    public Stream RetrievePlugin(IFileInfo fileInfo) {
        return fileInfo.OpenRead();
    }
}