namespace UnrealPluginManager.Core.Utils;

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
    /// Converts an enumerable collection of key-value pairs into an OrderedDictionary with the same key-value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of the values in the collection.</typeparam>
    /// <param name="source">The source collection of key-value pairs.</param>
    /// <returns>An OrderedDictionary containing the key-value pairs from the source collection.</returns>
    public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<T, TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, T>> source, Func<T, TValue> valueSelector) where TKey : notnull {
        return new OrderedDictionary<TKey, TValue>(source.Select(x => new KeyValuePair<TKey, TValue>(x.Key, valueSelector(x.Value))));
    }

}