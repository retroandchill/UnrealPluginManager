namespace UnrealPluginManager.Core.Utils;

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
    
}