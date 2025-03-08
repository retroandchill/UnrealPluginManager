namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides a set of static methods for extended LINQ functionality, allowing for more flexible
/// and enriched querying and transformation capabilities on collections and sequences.
/// </summary>
public static class LinqExtensions {
  
  /// <summary>
  /// Projects each element of a sequence into a new form by using a provided transform function,
  /// ignoring elements where the transform function results in exceptions.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
  /// <typeparam name="TResult">The type of the value returned by the transform function.</typeparam>
  /// <param name="source">The source sequence to project.</param>
  /// <param name="selector">A transform function to apply to each element in the source sequence.</param>
  /// <returns>An enumerable collection of the transformed elements that did not throw exceptions when processed by the selector function.</returns>
  public static IEnumerable<TResult> SelectValid<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector) {
    foreach (var item in source) {
      TResult result;
      try {
        result = selector(item);
      } catch (Exception) {
        continue;
      }

      yield return result;
    }
  }

  /// <summary>
  /// Projects each element of a sequence into a new form using a provided transform function
  /// that takes both the element and its index, ignoring elements where the transform function results in exceptions.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
  /// <typeparam name="TResult">The type of the value returned by the transform function.</typeparam>
  /// <param name="source">The source sequence to project.</param>
  /// <param name="selector">A transform function to apply to each element in the source sequence, which takes the element and its index as parameters.</param>
  /// <returns>An enumerable collection of the transformed elements that did not throw exceptions when processed by the selector function.</returns>
  public static IEnumerable<TResult> SelectValid<T, TResult>(this IEnumerable<T> source,
                                                             Func<T, int, TResult> selector) {
    int i = 0;
    foreach (var item in source) {
      TResult result;
      try {
        result = selector(item, i);
        i++;
      } catch (Exception) {
        i++;
        continue;
      }

      yield return result;
    }
  }

  /// <summary>
  /// Converts a sequence of key-value pairs into an ordered dictionary.
  /// </summary>
  /// <typeparam name="TKey">The type of keys in the source sequence.</typeparam>
  /// <typeparam name="TValue">The type of values in the source sequence.</typeparam>
  /// <param name="source">The source sequence of key-value pairs.</param>
  /// <returns>An ordered dictionary containing the elements of the source sequence.</returns>
  public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, TValue>> source) where TKey : notnull {
    return new OrderedDictionary<TKey, TValue>(source);
  }

  /// <summary>
  /// Converts a sequence of elements into an <see cref="OrderedDictionary{TKey, TValue}"/>
  /// using specified functions to select the key and value from each element.
  /// </summary>
  /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
  /// <typeparam name="TKey">The type of the keys in the resulting dictionary.</typeparam>
  /// <typeparam name="TValue">The type of the values in the resulting dictionary.</typeparam>
  /// <param name="source">The source sequence of elements to convert.</param>
  /// <param name="keySelector">A function to extract a key from each element.</param>
  /// <param name="valueSelector">A function to extract a value from each element.</param>
  /// <returns>An <see cref="OrderedDictionary{TKey, TValue}"/> representing the transformed sequence.</returns>
  public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<T, TKey, TValue>(this IEnumerable<T> source,
    Func<T, TKey> keySelector, Func<T, TValue> valueSelector) where TKey : notnull {
    var result = new OrderedDictionary<TKey, TValue>();
    foreach (var item in source) {
      result.Add(keySelector(item), valueSelector(item));
    }

    return result;
  }

  /// <summary>
  /// Converts an enumerable collection of key-value pairs into an OrderedDictionary using a specified value selector function.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the source collection.</typeparam>
  /// <typeparam name="TValue">The type of the values in the resulting OrderedDictionary.</typeparam>
  /// <typeparam name="T">The type of the values in the source collection.</typeparam>
  /// <param name="source">The source collection of key-value pairs.</param>
  /// <param name="valueSelector">A function to extract or transform values for the resulting OrderedDictionary.</param>
  /// <returns>An OrderedDictionary containing the transformed key-value pairs from the source collection.</returns>
  public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<T, TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, T>> source, Func<T, TValue> valueSelector) where TKey : notnull {
    return new OrderedDictionary<TKey, TValue>(source.Select(x =>
                                                                 new KeyValuePair<TKey, TValue>(
                                                                     x.Key, valueSelector(x.Value))));
  }

  /// <summary>
  /// Converts an asynchronous sequence of key-value pairs into an OrderedDictionary.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the source sequence.</typeparam>
  /// <typeparam name="TValue">The type of the values in the source sequence.</typeparam>
  /// <param name="source">An asynchronous enumerable sequence of key-value pairs to convert.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains an OrderedDictionary with the keys and values from the source sequence.</returns>
  public static async Task<OrderedDictionary<TKey, TValue>> ToOrderedDictionaryAsync<TKey, TValue>(
      this IAsyncEnumerable<KeyValuePair<TKey, TValue>> source) where TKey : notnull {
    var result = new OrderedDictionary<TKey, TValue>();
    await foreach (var elem in source) {
      result.Add(elem.Key, elem.Value);
    }

    return result;
  }

  /// <summary>
  /// Converts an OrderedDictionary with asynchronous value conversion logic into an OrderedDictionary asynchronously.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the OrderedDictionary.</typeparam>
  /// <typeparam name="T">The type of the values in the source OrderedDictionary.</typeparam>
  /// <typeparam name="TValue">The type of the converted values in the resulting OrderedDictionary.</typeparam>
  /// <param name="source">The source OrderedDictionary to be converted asynchronously.</param>
  /// <param name="valueSelector">A function to asynchronously project each value in the source OrderedDictionary into a new form.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the new OrderedDictionary with the transformed values.</returns>
  public static async Task<OrderedDictionary<TKey, TValue>> ToOrderedDictionaryAsync<T, TKey, TValue>(
      this OrderedDictionary<TKey, T> source, Func<T, Task<TValue>> valueSelector) where TKey : notnull {
    return await source.ToAsyncEnumerable()
        .SelectAwait(async x => new KeyValuePair<TKey, TValue>(x.Key, await valueSelector(x.Value)))
        .ToOrderedDictionaryAsync();
  }

}