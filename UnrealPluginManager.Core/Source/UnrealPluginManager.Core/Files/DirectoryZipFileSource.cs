using System.IO.Abstractions;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Files;

/// Represents a file source that creates a zip file from a directory.
/// This class implements the IFileSource interface to provide functionality for creating a zip
/// archive from the contents of a directory. It uses a file system abstraction for seamless
/// file and directory operations.
public sealed class DirectoryZipFileSource([ReadOnly] IDirectoryInfo directoryInfo) : IFileSource {
  /// <inheritdoc />
  public Task<IFileInfo> CreateFile(string destinationPath) {
    return directoryInfo.FileSystem.CreateZipFile(destinationPath, directoryInfo.FullName);
  }

  /// <inheritdoc />
  public Task OverwriteFile(IFileInfo fileInfo) {
    return directoryInfo.FileSystem.CreateZipFile(fileInfo.FullName, directoryInfo.FullName);
  }
}