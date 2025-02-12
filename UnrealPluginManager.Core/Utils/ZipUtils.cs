using System.IO.Abstractions;
using System.IO.Compression;

namespace UnrealPluginManager.Core.Utils;

public static class ZipUtils {
    public static async Task CreateZipFile(this IFileSystem fileSystem, string zipFilePath, string directoryPath) {
        var fromDirectory = fileSystem.DirectoryInfo.New(directoryPath);
        var toFile = fileSystem.FileInfo.New(zipFilePath);
        using (var targetZip = new ZipArchive(toFile.OpenWrite(), ZipArchiveMode.Create)) {
            await CreateZipEntryFromPath(fileSystem, targetZip, fromDirectory, directoryPath);
        }
    }

    private static async Task CreateZipEntryFromPath(IFileSystem fileSystem, ZipArchive targetZip,
        IDirectoryInfo rootDirectory, string directoryPath) {
        foreach (var path in rootDirectory.GetFileSystemInfos()) {
            var relativeName = fileSystem.Path.GetRelativePath(directoryPath, path.FullName);
            if (path is IDirectoryInfo directory) {
                targetZip.CreateEntry($"{relativeName}/");
                await CreateZipEntryFromPath(fileSystem, targetZip, directory, directoryPath);
            } else {
                var entry = targetZip.CreateEntry(relativeName);
                await using var entryStream = entry.Open();
                await using var sourceFile = fileSystem.FileStream.New(path.FullName, FileMode.Open);
                await sourceFile.CopyToAsync(entryStream);
            }
            
        }
    }
}