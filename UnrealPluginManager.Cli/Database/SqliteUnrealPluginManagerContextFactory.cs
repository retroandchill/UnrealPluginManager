using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore.Design;
using UnrealPluginManager.Cli.Abstractions;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Database;

public class SqliteUnrealPluginManagerContextFactory : IDesignTimeDbContextFactory<SqliteUnrealPluginManagerContext> {
    public SqliteUnrealPluginManagerContext CreateDbContext(string[] args) {
        var filesystem = new FileSystem();
        var environment = new SystemEnvironment();
        var storageService = new LocalStorageService(filesystem, environment);
        return new SqliteUnrealPluginManagerContext(filesystem, storageService);
    }
}