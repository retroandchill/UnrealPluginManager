using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UnrealPluginManager.Server.Config;
using UnrealPluginManager.Server.Database;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestCloudUnrealPluginManagerContext() : CloudUnrealPluginManagerContext(CreateMockedConfig()) {

  private SqliteConnection? _dbConnection;

  private static IConfiguration CreateMockedConfig() {
    var config = new Mock<IConfiguration>();
    var mockSection = new Mock<IConfigurationSection>();
    config.Setup(x => x.GetSection("Postgresql")).Returns(mockSection.Object);
    mockSection.Setup(x => x.Get<PostgresConfig>())
        .Returns((PostgresConfig?) null);

    return config.Object;
  }

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