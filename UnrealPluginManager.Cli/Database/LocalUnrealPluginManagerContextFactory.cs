using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore.Design;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Database;

/// <summary>
/// Factory class for creating instances of <see cref="LocalUnrealPluginManagerContext"/> at design time.
/// </summary>
/// <remarks>
/// This factory implementation is used primarily by Entity Framework tools to configure the DbContext for
/// migrations or other design-time operations. It utilizes services such as file system abstractions and
/// storage to ensure proper setup of the SQLite database context.
/// </remarks>
public class LocalUnrealPluginManagerContextFactory : IDesignTimeDbContextFactory<LocalUnrealPluginManagerContext> {
    /// <inheritdoc />
    public LocalUnrealPluginManagerContext CreateDbContext(string[] args) {
        var filesystem = new FileSystem();
        var environment = new SystemEnvironment();
        var storageService = new LocalStorageService(filesystem, environment);
        return new LocalUnrealPluginManagerContext(filesystem, storageService);
    }
}