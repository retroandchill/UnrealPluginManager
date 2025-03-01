using System.IO.Abstractions;
using System.IO.Compression;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for creating and extracting ZIP archives using an abstracted file system.
/// </summary>
public static class ZipUtils {
    /// <summary>
    /// Creates a ZIP archive from a specified directory, including all its contents.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction to be used for file and directory operations.</param>
    /// <param name="zipFilePath">The full path where the ZIP file should be created.</param>
    /// <param name="directoryPath">The path of the directory whose contents are to be archived.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task<IFileInfo> CreateZipFile(this IFileSystem fileSystem, string zipFilePath,
        string directoryPath) {
        var fromDirectory = fileSystem.DirectoryInfo.New(directoryPath);
        var toFile = fileSystem.FileInfo.New(zipFilePath);
        using var targetZip = new ZipArchive(toFile.OpenWrite(), ZipArchiveMode.Create);
        await CreateZipEntryFromPath(fileSystem, targetZip, fromDirectory, directoryPath);
        return toFile;
    }

    public static async Task<IFileInfo> CreateZipFile(this IFileSystem fileSystem, string zipFilePath,
                                                      IEnumerable<IDirectoryInfo> directories, IEnumerable<IFileInfo> files) {
        var toFile = fileSystem.FileInfo.New(zipFilePath);
        using var targetZip = new ZipArchive(toFile.OpenWrite(), ZipArchiveMode.Create);
        foreach (var directory in directories) {
            ArgumentNullException.ThrowIfNull(directory.Parent);
            await CreateZipEntryFromPath(fileSystem, targetZip, directory.Parent, directory.FullName);
        }
        return toFile;
        
    }

    private static async Task CreateZipEntryFromPath(IFileSystem fileSystem, ZipArchive targetZip,
        IDirectoryInfo rootDirectory, string directoryPath) {
        foreach (var path in rootDirectory.GetFileSystemInfos()) {
            var relativeName = Path.GetRelativePath(directoryPath, path.FullName);
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

    /// <summary>
    /// Extracts the contents of a ZIP archive to a specified destination directory using an abstracted file system.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction to be used for creating directories and writing files.</param>
    /// <param name="zipFile">The ZIP archive to be extracted.</param>
    /// <param name="destinationDirectory">The path to the directory where the ZIP contents should be extracted.</param>
    /// <returns>A task that represents the asynchronous extraction operation.</returns>
    public static async Task ExtractZipFile(this IFileSystem fileSystem, ZipArchive zipFile, string destinationDirectory) {
        var outputDirectory = fileSystem.Directory.CreateDirectory(destinationDirectory);
        foreach (var entry in zipFile.Entries) {
            var fullPath = Path.Combine(outputDirectory.FullName, entry.FullName);
            if (entry.FullName.EndsWith('/')) {
                fileSystem.Directory.CreateDirectory(fullPath);
            } else {
                await using var fileStream = fileSystem.FileStream.New(fullPath, FileMode.Create);
                await entry.Open().CopyToAsync(fileStream);
            }
        }
        
    }
}