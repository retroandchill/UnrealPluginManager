using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Files;

/// <summary>
/// Defines the interface for a file source that can create files at a specified destination.
/// </summary>
public interface IFileSource {
  /// <summary>
  /// Creates a file at the specified destination path.
  /// </summary>
  /// <param name="destinationPath">The path where the file will be created.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a reference to the created
  /// file as an <see cref="IFileInfo"/>.</returns>
  Task<IFileInfo> CreateFile(string destinationPath);

  /// <summary>
  /// Overwrites the contents of the specified file using the provided stream.
  /// </summary>
  /// <param name="fileInfo">The file to be overwritten.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task OverwriteFile(IFileInfo fileInfo);
}