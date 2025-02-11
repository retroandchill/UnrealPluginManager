using System.IO.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Services;

public class LocalStorageService(IFileSystem fileSystem) : StorageServiceBase(fileSystem) {
    public sealed override string BaseDirectory =>
        Environment.GetEnvironmentVariable(EnvironmentVariables.StorageDirectory) ??
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".unrealpluginmanager");
}