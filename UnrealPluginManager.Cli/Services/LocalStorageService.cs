using System.IO.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Services;

public class LocalStorageService(IFileSystem fileSystem) : StorageServiceBase(fileSystem) {
    protected sealed override string PluginDirectory =>
        Environment.GetEnvironmentVariable(EnvironmentVariables.StorageDirectory) ??
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".upm");
}