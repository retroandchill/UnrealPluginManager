namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for handling tasks safely, particularly for scenarios where exception handling is required for individual tasks.
/// </summary>
public static class SafeTasks {

  /// <summary>
  /// Executes a collection of tasks and returns a list of the provided tasks, ensuring that exceptions in individual tasks are safely ignored.
  /// This method is primarily designed for scenarios where all tasks must run to completion,
  /// regardless of exceptions in any of them, while still returning the original tasks for further inspection or handling.
  /// </summary>
  /// <typeparam name="T">The type of the result produced by the tasks.</typeparam>
  /// <param name="tasks">A collection of tasks to be executed.</param>
  /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation, containing a list of the original tasks.</returns>
  public static ValueTask<List<Task<T>>> WhenAll<T>(params IEnumerable<Task<T>> tasks) {
    return tasks.ToAsyncEnumerable()
        .SelectAwait(async task => {
          try {
            await task;
          } catch (Exception) {
            // Ignore exceptions.
          }
          return task;
        })
        .ToListAsync();
  }

  /// <summary>
  /// Executes a collection of tasks sequentially and yields each task upon completion,
  /// ensuring that exceptions in individual tasks are ignored. This method is useful for scenarios
  /// where each task should execute to completion while safely handling any potential exceptions.
  /// </summary>
  /// <typeparam name="T">The type of the result produced by the tasks.</typeparam>
  /// <param name="tasks">A collection of tasks to be executed sequentially.</param>
  /// <returns>An asynchronous enumerable of the original tasks, providing each task as it completes.</returns>
  public static async IAsyncEnumerable<Task<T>> WhenEach<T>(params IEnumerable<Task<T>> tasks) {
    foreach (var task in tasks) {
      try {
        await task;
      } catch (Exception) {
        // Ignore exceptions.
      }
      yield return task;
    }
  }

}