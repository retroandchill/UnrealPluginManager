using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database;

namespace UnrealPluginManager.Local.Tests.Utils;

public class TestLocalUnrealPluginManagerContext(IStorageService storageService)
    : LocalUnrealPluginManagerContext(storageService) {

  private SqliteConnection? _dbConnection;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    _dbConnection = new SqliteConnection("Filename=:memory:");
    _dbConnection.Open();
    optionsBuilder.UseSqlite(_dbConnection, b =>
            b.MinBatchSize(1)
                .MaxBatchSize(100))
        .UseSnakeCaseNamingConvention()
        .LogTo(Console.WriteLine);
  }

  public override void Dispose() {
    base.Dispose();
    _dbConnection?.Dispose();
    GC.SuppressFinalize(this);
  }

}