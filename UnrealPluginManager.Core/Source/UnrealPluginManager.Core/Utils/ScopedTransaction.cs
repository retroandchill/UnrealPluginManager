using Microsoft.EntityFrameworkCore.Storage;

namespace UnrealPluginManager.Core.Utils;

[AutoConstructor]
public sealed partial class ScopedTransaction : IAsyncDisposable {
  private readonly IDbContextTransaction _transaction;
  private bool _complete = false;

  public void Complete() {
    _complete = true;
  }

  public async ValueTask DisposeAsync() {
    if (_complete) {
      await _transaction.CommitAsync();
    } else {
      await _transaction.RollbackAsync();
    }

    await _transaction.DisposeAsync();
  }
}