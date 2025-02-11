using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore.Design;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Database;

public class SqliteUnrealPluginManagerContextFactory : IDesignTimeDbContextFactory<SqliteUnrealPluginManagerContext> {
    public SqliteUnrealPluginManagerContext CreateDbContext(string[] args) {
        var filesystem = new FileSystem();
        var storageService = new LocalStorageService(filesystem);
        return new SqliteUnrealPluginManagerContext(filesystem, storageService);
    }
}