using System.IO.Abstractions;
using System.IO.Compression;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Represents a subdirectory to be included in a ZIP archive, along with its corresponding path prefix within the archive.
/// </summary>
public record struct ZipSubDirectory(string Prefix, IDirectoryInfo Directory);

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

  /// <summary>
  /// Creates a ZIP archive from the specified collection of directories and files.
  /// </summary>
  /// <param name="fileSystem">The file system abstraction used for file and directory operations.</param>
  /// <param name="zipFilePath">The full path where the ZIP file will be created.</param>
  /// <param name="directories">A collection of directories to include in the ZIP archive.</param>
  /// <param name="files">A collection of individual files to include in the ZIP archive.</param>
  /// <returns>A task representing the asynchronous operation, which on completion provides an IFileInfo object representing the created ZIP file.</returns>
  public static Task<IFileInfo> CreateZipFile(this IFileSystem fileSystem, string zipFilePath,
                                              IEnumerable<IDirectoryInfo> directories, IEnumerable<IFileInfo> files) {
    return CreateZipFile(fileSystem, zipFilePath,
                         directories.Select(d => new ZipSubDirectory("", d)), files);
  }

  /// <summary>
  /// Creates a ZIP archive from the specified directories and files, using the provided file system abstraction.
  /// </summary>
  /// <param name="fileSystem">The file system abstraction to use for file and directory operations.</param>
  /// <param name="zipFilePath">The full path where the ZIP file will be created.</param>
  /// <param name="directories">A collection of directories, with optional prefixes, to include in the ZIP archive.</param>
  /// <param name="files">A collection of individual files to include in the ZIP archive.</param>
  /// <returns>A task that represents the asynchronous operation, returning the created ZIP file as an <see cref="IFileInfo"/>.</returns>
  public static async Task<IFileInfo> CreateZipFile(this IFileSystem fileSystem, string zipFilePath,
                                                    IEnumerable<ZipSubDirectory> directories,
                                                    IEnumerable<IFileInfo> files) {
    var toFile = fileSystem.FileInfo.New(zipFilePath);
    using var targetZip = new ZipArchive(toFile.OpenWrite(), ZipArchiveMode.Create);
    foreach (var (prefix, directory) in directories) {
      if (!string.IsNullOrEmpty(prefix)) {
        targetZip.CreateZipEntryDirectoryHierarchy(prefix);
      }

      ArgumentNullException.ThrowIfNull(directory.Parent);
      await CreateZipEntryFromPath(fileSystem, targetZip, directory, directory.Parent.FullName, prefix);
    }

    foreach (var file in files) {
      var entry = targetZip.CreateEntry(file.Name);
      await using var entryStream = entry.Open();
      await using var sourceFile = fileSystem.FileStream.New(file.FullName, FileMode.Open);
      await sourceFile.CopyToAsync(entryStream);
    }

    return toFile;
  }

  private static void CreateZipEntryDirectoryHierarchy(this ZipArchive targetZip, string prefix) {
    var splitPath = prefix.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
    var currentPath = "";
    foreach (var path in splitPath) {
      currentPath = Path.Join(currentPath, path);
      targetZip.CreateEntry($"{currentPath}/");
    }
  }

  private static async Task CreateZipEntryFromPath(IFileSystem fileSystem, ZipArchive targetZip,
                                                   IDirectoryInfo rootDirectory, string directoryPath,
                                                   string prefix = "") {
    foreach (var path in rootDirectory.GetFileSystemInfos()) {
      var relativeName = Path.Join(prefix, Path.GetRelativePath(directoryPath, path.FullName));
      if (path is IDirectoryInfo directory) {
        targetZip.CreateEntry($"{relativeName}/");
        await CreateZipEntryFromPath(fileSystem, targetZip, directory, directoryPath, prefix);
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
  public static async Task
      ExtractZipFile(this IFileSystem fileSystem, ZipArchive zipFile, string destinationDirectory) {
    var outputDirectory = fileSystem.Directory.CreateDirectory(destinationDirectory);
    foreach (var entry in zipFile.Entries) {
      var fullPath = Path.Combine(outputDirectory.FullName, entry.FullName);
      if (entry.FullName.EndsWith('/')) {
        fileSystem.Directory.CreateDirectory(fullPath);
      } else {
        var directory = fileSystem.Directory.GetParent(fullPath);
        if (directory is not null) {
          fileSystem.Directory.CreateDirectory(directory.FullName);
        }

        await using var fileStream = fileSystem.FileStream.New(fullPath, FileMode.Create);
        await entry.Open().CopyToAsync(fileStream);
      }
    }
  }

  /// <summary>
  /// Merges the entries from one ZIP archive into another.
  /// </summary>
  /// <param name="target">The target ZIP archive where entries will be merged.</param>
  /// <param name="source">The source ZIP archive whose entries will be added to the target archive.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  public static Task Merge(this ZipArchive target, ZipArchive source) {
    return target.CopyEntries(source.Entries);
  }

  /// <summary>
  /// Copies the given collection of ZIP archive entries into a new ZIP file at the specified destination path.
  /// </summary>
  /// <param name="fileSystem">The file system abstraction to use for file and directory operations.</param>
  /// <param name="entries">The collection of ZIP archive entries to copy to the new ZIP file.</param>
  /// <param name="destinationPath">The file path where the new ZIP file should be created.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a reference to the created ZIP file.</returns>
  public static async Task<IFileInfo> CopyEntries(this IFileSystem fileSystem, IEnumerable<ZipArchiveEntry> entries,
                                                  string destinationPath) {
    var toFile = fileSystem.FileInfo.New(destinationPath);
    using var targetZip = new ZipArchive(toFile.OpenWrite(), ZipArchiveMode.Create);
    await targetZip.CopyEntries(entries);
    return toFile;
  }

  private static async Task CopyEntries(this ZipArchive targetZip, IEnumerable<ZipArchiveEntry> entries) {
    var entriesAdded = new HashSet<string>();
    foreach (var entry in entries) {
      var splitPath = entry.FullName.TrimEnd('/').Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
      var currentPath = "";
      foreach (var (index, path) in splitPath.Index()) {
        if (index == splitPath.Length - 1) {
          break;
        }

        currentPath = Path.Join(currentPath, path);
        var dirName = $"{currentPath}/";
        if (entriesAdded.Contains(dirName)) {
          continue;
        }

        targetZip.CreateEntry(dirName);
        entriesAdded.Add(dirName);
      }

      var createdEntry = targetZip.CreateEntry(entry.FullName);
      if (entry.FullName.EndsWith('/')) {
        entriesAdded.Add(entry.FullName);
        continue;
      }

      await using var entryStream = createdEntry.Open();
      await using var sourceStream = entry.Open();
      await sourceStream.CopyToAsync(entryStream);
    }
  }
}