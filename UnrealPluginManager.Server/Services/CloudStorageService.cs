using System.IO.Compression;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Config;

namespace UnrealPluginManager.Server.Services;

public class CloudStorageService : IStorageService {
    
    private readonly StorageMetadata _storageMetadata;

    public CloudStorageService(IConfiguration config) {
        _storageMetadata = new StorageMetadata();
        config.GetSection(StorageMetadata.Name).Bind(_storageMetadata);
        
        var directoryInfo = new DirectoryInfo(_storageMetadata.BaseDirectory);
        if (!directoryInfo.Exists) {
            directoryInfo.Create();
        }
    }
    
    public async Task<FileInfo> StorePlugin(Stream fileData) {
        using var archive = new ZipArchive(fileData);
        
        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }
        
        var fileName = $"{Path.GetFileNameWithoutExtension(archiveEntry.FullName)}{Path.GetRandomFileName()}.zip";
        var fullPath = Path.Combine(_storageMetadata.BaseDirectory, fileName);
        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        fileData.Seek(0, SeekOrigin.Begin);
        await fileData.CopyToAsync(fileStream);
        
        return new FileInfo(fullPath);
    }

    public Stream RetrievePlugin(FileInfo fileInfo) {
        return fileInfo.OpenRead();
    }
}