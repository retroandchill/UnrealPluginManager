using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Files;

/// <summary>
/// A file source implementation that creates a file based on a provided stream.
/// </summary>
/// <remarks>
/// This class facilitates creating a file in the file system by utilizing an internal stream,
/// allowing the user to specify the destination path for the created file.
/// </remarks>
[AutoConstructor]
public sealed partial class StreamFileSource : IFileSource {
  private readonly IFileSystem _fileSystem;
  private readonly Stream _stream;

  /// <inheritdoc />
  public async Task<IFileInfo> CreateFile(string destinationPath) {
    await using var fileStream = _fileSystem.FileStream.New(destinationPath, FileMode.Create);
    if (_stream.CanSeek) {
      _stream.Seek(0, SeekOrigin.Begin);
    }
    await _stream.CopyToAsync(fileStream);
    return _fileSystem.FileInfo.New(destinationPath);
  }

  /// <inheritdoc />
  public async Task OverwriteFile(IFileInfo fileInfo) {
    await using var fileStream = fileInfo.Open(FileMode.Truncate, FileAccess.Write);
    if (_stream.CanSeek) {
      _stream.Seek(0, SeekOrigin.Begin);
    }
    await _stream.CopyToAsync(fileStream);
  }

}