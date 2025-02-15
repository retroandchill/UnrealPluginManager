using System.Collections;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Pagination;

/// <summary>
/// Represents a paginated collection of items of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the items in the paginated collection.</typeparam>
[JsonConverter(typeof(PageJsonConverterFactory))]
public class Page<T> : IReadOnlyList<T> {
    
    private readonly IList<T> _items;

    /// <summary>
    /// Represents a paginated collection of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the paginated collection.</typeparam>
    public Page(IEnumerable<T> items, int pageNumber, int pageSize) {
        _items = items as IList<T> ?? items.ToList();
        PageNumber = pageNumber;
        TotalPages = pageSize;
    }

    /// <summary>
    /// Represents a paginated collection of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the paginated collection.</typeparam>
    public Page(IEnumerable<T> items, int count, int pageNumber, int pageSize) 
        : this(items, pageNumber, (int) Math.Ceiling(count / (double) pageSize)) {
    }

    /// <summary>
    /// Gets the current page number in the paginated collection.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the total number of pages in the paginated collection.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Indicates whether the current page is the first page in the paginated collection.
    /// </summary>
    public bool IsFirstPage => PageNumber == 1;

    /// <summary>
    /// Indicates whether the current page is the last page in the paginated collection.
    /// </summary>
    public bool IsLastPage => PageNumber == TotalPages;
    
    /// <inheritdoc/>
    public int Count => _items.Count;
    
    /// <inheritdoc/>
    public T this[int index] => _items[index];

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    /// <summary>
    /// Transforms the items in the current paginated collection by applying a selector function and creates a new paginated collection of the transformed items.
    /// </summary>
    /// <typeparam name="TResult">The type of the items in the resulting paginated collection.</typeparam>
    /// <param name="selector">A function to transform each item in the current collection.</param>
    /// <returns>A new <see cref="Page{TResult}"/> instance containing the transformed items.</returns>
    public Page<TResult> Select<TResult>(Func<T, TResult> selector) {
        return new Page<TResult>(_items.Select(selector), PageNumber, TotalPages);
    }

    /// <summary>
    /// Transforms the items in the current paginated collection by applying a selector function and creates a new paginated collection of the transformed items.
    /// </summary>
    /// <typeparam name="TResult">The type of the items in the resulting paginated collection.</typeparam>
    /// <param name="selector">A function to transform each item in the current collection, providing the index of the item as an additional argument.</param>
    /// <returns>A new <see cref="Page{TResult}"/> instance containing the transformed items.</returns>
    public Page<TResult> Select<TResult>(Func<T, int, IEnumerable<TResult>> selector) {
        return new Page<TResult>(_items.SelectMany(selector), PageNumber, TotalPages);
    }
}