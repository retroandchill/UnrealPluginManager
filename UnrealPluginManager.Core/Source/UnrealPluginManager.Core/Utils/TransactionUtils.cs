using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.Utils;

public static class TransactionUtils {
  public static async Task<ScopedTransaction> StartTransaction(this DbContext dbContext) {
    return new ScopedTransaction(await dbContext.Database.BeginTransactionAsync());
  }
}