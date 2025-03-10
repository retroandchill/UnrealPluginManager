namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Represents a dictionary that holds objects implementing <see cref="IAsyncDisposable"/>.
/// This dictionary ensures that all contained values are asynchronously disposed of when the dictionary itself is disposed asynchronously.
/// </summary>
/// <typeparam name="TKey">
/// The type of the keys in the dictionary. Must be non-nullable.
/// </typeparam>
/// <typeparam name="TValue">
/// The type of the values in the dictionary. Must implement <see cref="IAsyncDisposable"/>.
/// </typeparam>
/// <remarks>
/// <see cref="AsyncDisposableDictionary{TKey, TValue}"/> inherits from <see cref="Dictionary{TKey, TValue}"/>
/// but extends it with asynchronous disposal for its values.
/// </remarks>
public class AsyncDisposableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IAsyncDisposable 
    where TKey : notnull 
    where TValue : IAsyncDisposable {
  /// <inheritdoc />
  public async ValueTask DisposeAsync() {
    foreach (var value in Values) {
      await value.DisposeAsync();
    }
  }
}