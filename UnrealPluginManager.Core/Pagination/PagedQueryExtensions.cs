using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.Pagination;

/// <summary>
/// Provides extension methods for paginating queryable collections.
/// </summary>
public static class PagedQueryExtensions {

    /// <summary>
    /// Converts a queryable collection into a paginated page of items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="query">The queryable collection to paginate.</param>
    /// <param name="pageNumber">The number of the page to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A <see cref="Page{T}"/> containing the items for the specified page and metadata about pagination.</returns>
    public static Page<T> ToPage<T>(this IQueryable<T> query, int pageNumber, int pageSize) {
        var count = query.Count();
        if (count <= 0) {
            return new Page<T>([], count, pageNumber, pageSize);
        }
        
        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return new Page<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Asynchronously converts a queryable collection into a paginated page of items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    /// <param name="query">The queryable collection to paginate.</param>
    /// <param name="pageNumber">The number of the page to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Page{T}"/> containing the items for the specified page and metadata about the pagination.</returns>
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default) {
        var count = await query.CountAsync(cancellationToken);
        if (count <= 0) {
            return new Page<T>([], count, pageNumber, pageSize);
        }
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new Page<T>(items, count, pageNumber, pageSize);

    }
    
}