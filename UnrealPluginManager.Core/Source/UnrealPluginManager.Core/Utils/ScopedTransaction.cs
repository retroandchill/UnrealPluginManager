using Microsoft.EntityFrameworkCore.Storage;
using Retro.ReadOnlyParams.Annotations;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// A scoped object for handling a transaction where if not marked complete will automatically roll back.
/// </summary>
/// <param name="transaction">The encompassing transaction</param>
public sealed class ScopedTransaction([ReadOnly] IDbContextTransaction transaction) : IAsyncDisposable {
  private bool _complete;

  /// <summary>
  /// Mark the transaction as complete, leading to a commit when the scope ends.
  /// </summary>
  public void Complete() {
    _complete = true;
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync() {
    if (_complete) {
      await transaction.CommitAsync();
    } else {
      await transaction.RollbackAsync();
    }

    await transaction.DisposeAsync();
  }
}