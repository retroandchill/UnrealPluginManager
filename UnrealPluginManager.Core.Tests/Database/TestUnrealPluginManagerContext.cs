using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;

namespace UnrealPluginManager.Core.Tests.Database;

public class TestUnrealPluginManagerContext(IFileSystem filesystem) : UnrealPluginManagerContext(filesystem) {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .LogTo(Console.WriteLine);
    }
}