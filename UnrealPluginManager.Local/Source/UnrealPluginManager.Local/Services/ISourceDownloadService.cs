using System.IO.Abstractions;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides methods for downloading, verifying, and patching source code files.
/// </summary>
public interface ISourceDownloadService {

  /// <summary>
  /// Downloads source code from the specified location, extracts it, and places it into the target directory.
  /// </summary>
  /// <param name="sourceLocation">The location of the source code to be downloaded, including its URI and hash.</param>
  /// <param name="directory">The directory where the source code will be extracted.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task DownloadAndExtractSources(SourceLocation sourceLocation, IDirectoryInfo directory);

  /// <summary>
  /// Verifies that the SHA256 hash of the specified file matches the expected hash.
  /// </summary>
  /// <param name="file">The file whose hash is to be verified.</param>
  /// <param name="expectedHash">The expected SHA256 hash value of the file.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  /// <exception cref="BadSubmissionException">Thrown when the computed hash of the file does not match the expected hash.</exception>
  Task VerifySourceHash(IFileInfo file, string expectedHash);

  /// <summary>
  /// Applies patches to the source code located in the specified directory.
  /// </summary>
  /// <param name="directory">The directory containing the source code to be patched.</param>
  /// <param name="patches">A list of patch file paths to be applied to the source code.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task PatchSources(IDirectoryInfo directory, IReadOnlyList<string> patches);

}