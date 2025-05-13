using System.IO.Abstractions;
using System.IO.Compression;
using System.Security.Cryptography;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides functionality for downloading, verifying, and extracting source files
/// from a remote location as well as applying patches to the downloaded sources.
/// </summary>
[AutoConstructor]
public partial class SourceDownloadService : ISourceDownloadService {

  private readonly HttpClient _httpClient;
  private readonly IFileSystem _fileSystem;
  private readonly IProcessRunner _processRunner;

  /// <inheritdoc />
  public async Task DownloadAndExtractSources(SourceLocation sourceLocation, IDirectoryInfo directory) {
    using var intermediate = _fileSystem.CreateDisposableDirectory(out var intermediateFolder);
    var zipFileName = Path.Join(intermediateFolder.FullName, "Source.zip");
    await using (var downloadStream = await _httpClient.GetStreamAsync(sourceLocation.Url)) {
      await using var fileStream = _fileSystem.FileStream.New(zipFileName, FileMode.Create);
      await downloadStream.CopyToAsync(fileStream);
    }

    await VerifySourceHash(_fileSystem.FileInfo.New(zipFileName), sourceLocation.Sha);

    using var zipArchive = new ZipArchive(_fileSystem.File.OpenRead(zipFileName), ZipArchiveMode.Read);
    await _fileSystem.ExtractZipFile(zipArchive, directory.FullName);
  }

  /// <inheritdoc />
  public async Task VerifySourceHash(IFileInfo file, string expectedHash) {
    string computedHashString;
    await using (var verifyStream = file.OpenRead()) {
      using var sha256 = SHA256.Create();
      var computedHash = await sha256.ComputeHashAsync(verifyStream);
      computedHashString = Convert.ToHexString(computedHash).ToLowerInvariant();
    }
    if (!string.Equals(computedHashString, expectedHash, StringComparison.OrdinalIgnoreCase)) {
      throw new BadSubmissionException(
          $"SHA256 hash mismatch for downloaded file. Expected: {expectedHash}, Got: {computedHashString}");
    }
  }

  /// <inheritdoc />
  public async Task PatchSources(IDirectoryInfo directory, IReadOnlyList<string> patches) {
    foreach (var patch in patches) {
      using var disposableFile = _fileSystem.CreateDisposableFile(out var tempFileInfo);
      await _fileSystem.File.WriteAllTextAsync(tempFileInfo.FullName, patch);

      var result = await _processRunner.RunProcess("git", ["apply", tempFileInfo.FullName], directory.FullName);
      if (result != 0) {
        throw new BadSubmissionException("Failed to apply patches.");
      }
    }

  }
}