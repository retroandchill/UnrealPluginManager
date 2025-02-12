using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore.Design;
using Testably.Abstractions;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Database;

public class SqliteUnrealPluginManagerContextFactory : IDesignTimeDbContextFactory<SqliteUnrealPluginManagerContext> {
    public SqliteUnrealPluginManagerContext CreateDbContext(string[] args) {
        var filesystem = new RealFileSystem();
        var storageService = new LocalStorageService(filesystem);
        return new SqliteUnrealPluginManagerContext(filesystem, storageService);
    }
}