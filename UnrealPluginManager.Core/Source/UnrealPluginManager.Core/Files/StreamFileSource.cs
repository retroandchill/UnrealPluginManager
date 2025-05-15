using System.IO.Abstractions;
using Retro.ReadOnlyParams.Annotations;

namespace UnrealPluginManager.Core.Files;

/// <summary>
/// A file source implementation that creates a file based on a provided stream.
/// </summary>
/// <remarks>
/// This class facilitates creating a file in the file system by utilizing an internal stream,
/// allowing the user to specify the destination path for the created file.
/// </remarks>
public sealed class StreamFileSource([ReadOnly] IFileSystem fileSystem, [ReadOnly] Stream stream) : IFileSource {
  /// <inheritdoc />
  public async Task<IFileInfo> CreateFile(string destinationPath) {
    await using var fileStream = fileSystem.FileStream.New(destinationPath, FileMode.Create);
    if (stream.CanSeek) {
      stream.Seek(0, SeekOrigin.Begin);
    }

    await stream.CopyToAsync(fileStream);
    return fileSystem.FileInfo.New(destinationPath);
  }

  /// <inheritdoc />
  public async Task OverwriteFile(IFileInfo fileInfo) {
    await using var fileStream = fileInfo.Open(FileMode.Truncate, FileAccess.Write);
    if (stream.CanSeek) {
      stream.Seek(0, SeekOrigin.Begin);
    }

    await stream.CopyToAsync(fileStream);
  }
}