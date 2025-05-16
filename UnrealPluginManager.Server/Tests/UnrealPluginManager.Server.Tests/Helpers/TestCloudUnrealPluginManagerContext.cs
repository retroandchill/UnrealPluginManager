using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Server.Database;

namespace UnrealPluginManager.Server.Tests.Helpers;

public sealed class TestCloudUnrealPluginManagerContext() : CloudUnrealPluginManagerContext(CreateMockedConfig()) {

  private SqliteConnection? _dbConnection;

  private DeferredDelete? _deferredDelete;

  public sealed class DeferredDelete([ReadOnly] TestCloudUnrealPluginManagerContext owner)
      : IDisposable, IAsyncDisposable {

    public void Dispose() {
      owner.Dispose();
    }

    public async ValueTask DisposeAsync() {
      await owner.DisposeAsync();
    }
  }

  public DeferredDelete DefferDeletion() {
    if (_deferredDelete is not null) {
      return _deferredDelete;
    }

    _deferredDelete = new DeferredDelete(this);
    return _deferredDelete;
  }

  private static IConfiguration CreateMockedConfig() {
    var config = new Mock<IConfiguration>();
    var mockSection = new Mock<IConfigurationSection>();
    config.Setup(x => x.GetSection("Postgresql")).Returns(mockSection.Object);
    mockSection.Setup(x => x.Value)
        .Returns((string?) null);

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
    if (_deferredDelete is not null) {
      return;
    }

    base.Dispose();
    _dbConnection?.Dispose();
  }

  public override async ValueTask DisposeAsync() {
    if (_deferredDelete is not null) {
      return;
    }

    await base.DisposeAsync();
    if (_dbConnection is null) {
      return;
    }

    await _dbConnection.DisposeAsync();
  }

}