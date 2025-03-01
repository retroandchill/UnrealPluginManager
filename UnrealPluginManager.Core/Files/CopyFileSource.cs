using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Files;

/// <summary>
/// The CopyFileSource class represents a source for copying files in a file system.
/// It implements the IFileSource interface to provide functionality for creating
/// copies of files at specified destination paths.
/// </summary>
[AutoConstructor]
public sealed partial class CopyFileSource : IFileSource {
    private readonly IFileInfo _fileInfo;

    /// <inheritdoc />
    public Task<IFileInfo> CreateFile(string destinationPath) {
        return Task.FromResult(_fileInfo.CopyTo(destinationPath));
    }
}