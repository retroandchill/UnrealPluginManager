namespace UnrealPluginManager.Core.Utils;

public static class AsyncUtils {

  public static async Task<T> CatchAsync<T>(this Task<T> task, Func<Exception, Task<T>> func) {
    try {
      return await task;
    } catch (Exception e) {
      return await func(e);
    }
  }

}