using System.IO.Abstractions;
using Retro.ReadOnlyParams.Annotations;

namespace UnrealPluginManager.Core.Files;

/// <summary>
/// The CopyFileSource class represents a source for copying files in a file system.
/// It implements the IFileSource interface to provide functionality for creating
/// copies of files at specified destination paths.
/// </summary>
public sealed class CopyFileSource([ReadOnly] IFileInfo info) : IFileSource {
  /// <inheritdoc />
  public Task<IFileInfo> CreateFile(string destinationPath) {
    return Task.FromResult(info.CopyTo(destinationPath, false));
  }

  /// <inheritdoc />
  public Task OverwriteFile(IFileInfo fileInfo) {
    return Task.FromResult(info.CopyTo(fileInfo.FullName, true));
  }
}