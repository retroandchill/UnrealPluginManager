﻿namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// A utility class providing extension methods for collections.
/// </summary>
public static class CollectionUtils {

    /// <summary>
    /// Adds the elements of the specified collection to the current collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to which items will be added.</param>
    /// <param name="items">The collection of items to be added to the current collection.</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items) {
        foreach (var item in items) {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Retrieves the value associated with the specified key if it exists. If the key does not exist, generates a value using the specified factory function,
    /// adds it to the dictionary, and returns the generated value.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to perform the operation on.</param>
    /// <param name="key">The key whose associated value is to be retrieved or generated.</param>
    /// <param name="valueFactory">A factory function to generate a value if the key does not exist.</param>
    /// <returns>The value associated with the specified key, or the value generated by the factory function if the key does not exist.</returns>
    public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                       Func<TKey, TValue> valueFactory) {
        if (dictionary.TryGetValue(key, out var value)) {
            return value;
        }
        
        dictionary.Add(key, valueFactory(key));
        return dictionary[key];
    }

    /// <summary>
    /// Adds elements from the specified collection to the current collection
    /// without duplicating entries based on the selected key.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="TKey">The type of key used for identifying distinct elements.</typeparam>
    /// <param name="collection">The collection to which distinct items will be added.</param>
    /// <param name="items">The collection of items to be added.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    public static void AddDistinctRange<T, TKey>(this ICollection<T> collection, IEnumerable<T> items, 
                                                 Func<T, TKey> keySelector) {
        var explored = collection
                .Select(keySelector)
                .ToHashSet();
        foreach (var item in items) {
            var key = keySelector(item);
            if (explored.Contains(key)) {
                continue;
            }
            
            collection.Add(item);
            explored.Add(key);
        }
    }
    
}