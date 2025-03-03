using System.IO.Abstractions;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Files;

/// Represents a file source that creates a zip file from a directory.
/// This class implements the IFileSource interface to provide functionality for creating a zip
/// archive from the contents of a directory. It uses a file system abstraction for seamless
/// file and directory operations.
[AutoConstructor]
public sealed partial class DirectoryZipFileSource : IFileSource {
  private readonly IDirectoryInfo _directoryInfo;

  /// <inheritdoc />
  public Task<IFileInfo> CreateFile(string destinationPath) {
    return _directoryInfo.FileSystem.CreateZipFile(destinationPath, _directoryInfo.FullName);
  }
}