using System.IO.Abstractions;
using UnrealPluginManager.Cli.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Services;

public class LocalStorageService(IFileSystem fileSystem, IEnvironment environment) : StorageServiceBase(fileSystem) {
    public sealed override string BaseDirectory =>
        environment.GetEnvironmentVariable(EnvironmentVariables.StorageDirectory) ??
        Path.Join(environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".unrealpluginmanager");
}