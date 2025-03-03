using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UnrealPluginManager.Core.Database;

namespace UnrealPluginManager.Core.Tests.Database;

public class TestUnrealPluginManagerContext(IFileSystem filesystem) : UnrealPluginManagerContext(filesystem) {
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString())
        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        .LogTo(Console.WriteLine);
  }
}