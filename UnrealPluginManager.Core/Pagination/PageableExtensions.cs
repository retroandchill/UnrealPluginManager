using LanguageExt;

namespace UnrealPluginManager.Core.Pagination;

/// <summary>
/// Provides extension methods for the <see cref="Pageable"/> struct to enhance its usability.
/// </summary>
/// <remarks>
/// The <see cref="PageableExtensions"/> class contains utility methods used to simplify or
/// extend the functionality of <see cref="Pageable"/>. These methods include transformations
/// and conversions that work specifically with the <see cref="Pageable"/> struct, such as
/// converting it to an <see cref="Option{T}"/>.
/// </remarks>
public static class PageableExtensions {

    /// <summary>
    /// Converts a <see cref="Pageable"/> instance to an <see cref="Option{T}"/> of type <see cref="PageRequest"/>.
    /// </summary>
    /// <param name="pageable">
    /// The <see cref="Pageable"/> instance to be converted.
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing a <see cref="PageRequest"/> if the conversion is valid;
    /// otherwise, an empty <see cref="Option{T}"/>.
    /// </returns>
    public static Option<PageRequest> ToOption(this Pageable pageable) {
        return pageable;
    }

    /// <summary>
    /// Iterates over a data source, performing paginated requests until reaching the end of the source.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items in the source collection.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the items contained in each page of the results.
    /// </typeparam>
    /// <param name="source">
    /// The initial data source that will be used to determine the starting point of the pagination.
    /// </param>
    /// <param name="func">
    /// A function that takes an item from the source and a <see cref="PageRequest"/>, and returns a <see cref="Page{TResult}"/>.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page. Defaults to 10 if not specified.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> containing all the results from the paginated requests.
    /// </returns>
    public static IEnumerable<TResult> PageToEnd<T, TResult>(this IEnumerable<T> source,
                                                             Func<T, PageRequest, Page<TResult>> func,
                                                             int pageSize = 10) {
        var pageable = new PageRequest(1, pageSize);
        var first = source.First();
        var currentPage = func(first, pageable);
        IEnumerable<TResult> result = currentPage;
        while (!currentPage.IsLastPage) {
            pageable = pageable.Next;
            currentPage = func(first, pageable);
            result = result.Concat(currentPage);
        }

        return result;
    }

}