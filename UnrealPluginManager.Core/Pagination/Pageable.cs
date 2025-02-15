using LanguageExt;

namespace UnrealPluginManager.Core.Pagination;

/// <summary>
/// Represents an unpaged indicator used to signify the absence of pagination.
/// </summary>
/// <remarks>
/// The <see cref="Unpaged"/> struct serves as a marker to denote that a collection
/// or dataset is to be treated without any pagination rules applied. It is utilized
/// internally to configure instances of pagination utilities, such as <see cref="PageRequest"/>,
/// allowing for more flexible and explicit control over paginated and unpaginated scenarios.
/// </remarks>
internal struct Unpaged;

/// <summary>
/// Represents a request for pagination with details about the page number and page size.
/// </summary>
/// <remarks>
/// The <see cref="PageRequest"/> struct is used to encapsulate the details of a pagination request,
/// including the desired page number and the number of items per page. It provides functionality
/// to navigate through pages and validate pagination parameters. Instances of <see cref="PageRequest"/>
/// support advanced pagination use cases, such as retrieving the first, next, or previous page, and
/// calculating the offset from the start of the dataset.
/// </remarks>
public struct PageRequest {

    /// <summary>
    /// Represents the current page number in a paginated request.
    /// </summary>
    /// <remarks>
    /// This property specifies the 1-based index of the page being requested in the context
    /// of pagination. It is a critical component of any paginated query, used alongside the
    /// page size to determine the subset of data to retrieve. A valid page number must be
    /// greater than zero.
    /// </remarks>
    public int PageNumber { get; }
    /// <summary>
    /// Represents the number of items to be retrieved per page in a paginated request.
    /// </summary>
    /// <remarks>
    /// This property defines the size of each page in the context of pagination, specifying
    /// the maximum number of items returned for a single page. A valid page size must be greater
    /// than zero. It works alongside the page number to determine the specific subset of data to retrieve.
    /// </remarks>
    public int PageSize { get; }

    /// <summary>
    /// Represents the offset of the first item in the current page within the entire dataset.
    /// </summary>
    /// <remarks>
    /// This property calculates the zero-based position of the first item on the current page
    /// relative to the start of the dataset. It is derived from the page number and page size,
    /// playing a critical role in database queries or collections where an explicit starting
    /// index is required for pagination.
    /// </remarks>
    public int Offset => PageNumber * PageSize;

    /// <summary>
    /// Indicates whether the current pagination request is valid.
    /// </summary>
    /// <remarks>
    /// This property evaluates the validity of the pagination request by ensuring that both the
    /// page number and page size are greater than zero. A valid pagination request must have
    /// positive values for these parameters. This property is used internally to determine if
    /// the pagination request can be processed correctly.
    /// </remarks>
    internal bool IsValid => PageNumber > 0 && PageSize > 0;

    /// <summary>
    /// Represents a request for pagination with details about the page number and page size.
    /// </summary>
    /// <remarks>
    /// The <see cref="PageRequest"/> struct is used to define and manage parameters necessary for paginated data access,
    /// such as the specific page number and items per page. It provides methods for navigating between pages,
    /// such as accessing the first, next, or previous page. Validation for pagination settings is also included
    /// to ensure consistency when handling data collections or query results.
    /// </remarks>
    public PageRequest(int pageNumber = 1, int pageSize = 10) {
        if (pageNumber < 1) {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
        }

        if (pageSize < 1) {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }
        
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Represents a request for paginated data, including the page number and the number of items per page.
    /// </summary>
    /// <remarks>
    /// The <see cref="PageRequest"/> struct is designed to manage the state and behavior of pagination, enabling precise control
    /// over navigation through a dataset. It provides properties for determining offsets, validating pagination inputs,
    /// and retrieving the first, next, and previous pages. This structure simplifies the handling of paginated queries
    /// in scenarios like UI pagination or database data loading.
    /// </remarks>
    internal PageRequest(Unpaged tag) {
        PageNumber = 0;
    }

    /// <summary>
    /// Retrieves the first page in the pagination sequence.
    /// </summary>
    /// <remarks>
    /// This property returns a <see cref="PageRequest"/> object representing the first page of a
    /// paginated dataset, with the current page size maintained. It is useful for resetting
    /// pagination to the beginning of the dataset while preserving the page size configuration.
    /// </remarks>
    public PageRequest First => new PageRequest(1, PageSize);

    /// <summary>
    /// Retrieves the next page request in the pagination sequence.
    /// </summary>
    /// <remarks>
    /// This property generates a new <see cref="PageRequest"/> instance representing
    /// the next page in the paginated dataset. It increments the current page number
    /// while maintaining the same page size, facilitating forward navigation through
    /// the paginated results. The new page request can be used to efficiently load
    /// the subsequent set of data items.
    /// </remarks>
    public PageRequest Next => new PageRequest(PageNumber + 1, PageSize);

    /// <summary>
    /// Determines whether there is a previous page available in the pagination sequence.
    /// </summary>
    /// <remarks>
    /// This property returns true if the current page number is greater than 1, indicating
    /// the existence of a preceding page. It is commonly used to enable navigation to
    /// earlier pages in a paginated data set or query result.
    /// </remarks>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Retrieves either the previous page request or the first page request, depending on availability.
    /// </summary>
    /// <remarks>
    /// This property determines which page to navigate to based on the current page number. If there is a
    /// valid previous page (i.e., the current page is greater than the first page), it returns a request
    /// pointing to the previous page. Otherwise, it returns a request pointing to the first page.
    /// This ensures safe navigation within paginated data, avoiding invalid page references.
    /// </remarks>
    public PageRequest PreviousOrFirst => HasPrevious ? new PageRequest(PageNumber - 1, PageSize) : First;

    /// <summary>
    /// Creates a new <see cref="PageRequest"/> with the specified page number while retaining the current page size.
    /// </summary>
    /// <param name="pageNumber">The page number to set for the new pagination request. It must be greater than 0.</param>
    /// <returns>A new <see cref="PageRequest"/> instance with the specified page number and the current page size.</returns>
    public PageRequest WithPageNumber(int pageNumber) => new PageRequest(pageNumber, PageSize);
}

/// <summary>
/// Represents a pageable utility for performing pagination on collections or datasets.
/// </summary>
/// <remarks>
/// This struct provides mechanisms to configure pagination settings, such as
/// page number and page size. It also enables determining whether the pagination is applied
/// or left unpaged. The <see cref="Pageable"/> struct can be utilized to convert collections
/// into paginated results or execute conditional logic based on the pagination configuration.
/// </remarks>
public struct Pageable {

    private PageRequest _request = new PageRequest(new Unpaged());

    /// <summary>
    /// Indicates whether the pagination configuration is currently active.
    /// </summary>
    /// <remarks>
    /// This property evaluates the internal page request to determine if
    /// specific pagination parameters, such as page number and page size,
    /// have been set and are valid. It returns true if a valid pagination
    /// configuration exists, meaning data retrieval can be paginated accordingly.
    /// If false, it signifies that no pagination is applied, and the request is unpaged.
    /// </remarks>
    public bool IsPaged => _request.IsValid;

    /// <summary>
    /// Indicates whether the current configuration is not utilizing pagination.
    /// </summary>
    /// <remarks>
    /// This property determines if the internal page request represents an unpaged state,
    /// which occurs when no valid page number or page size values are set. It returns true
    /// when pagination is not applied, signifying that the data retrieval or processing
    /// does not involve any partitioning by pages. If false, it implies that pagination
    /// settings are active and data is being handled in a paginated manner.
    /// </remarks>
    public bool IsUnpaged => !_request.IsValid;

    /// <summary>
    /// Represents a pageable abstraction for managing pagination settings and behavior.
    /// </summary>
    /// <remarks>
    /// This struct is used to define and validate page requests with a specific page number
    /// and size. It supports both paged and unpaged operations, enabling more flexible handling
    /// of data collections or results. The <see cref="Match"/> methods allow conditional branching
    /// based on whether pagination settings are valid or not, facilitating different scenarios
    /// where pagination may or may not be applied.
    /// </remarks>
    public Pageable(int pageNumber, int pageSize) {
        _request = new PageRequest(pageNumber, pageSize);
    }

    /// <summary>
    /// Executes conditional logic based on whether pagination is applied or not.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the provided delegates.</typeparam>
    /// <param name="ifPresent">A delegate to execute if pagination is configured.</param>
    /// <param name="ifEmpty">A delegate to execute if pagination is not configured (unpaged).</param>
    /// <returns>
    /// The result of the <paramref name="ifPresent"/> delegate if pagination is applied
    /// or the result of the <paramref name="ifEmpty"/> delegate if it is unpaged.
    /// </returns>
    public TResult Match<TResult>(Func<PageRequest, TResult> ifPresent, Func<TResult> ifEmpty) {
        return _request.IsValid ? ifPresent(_request) : ifEmpty();
    }

    /// <summary>
    /// Executes conditional logic based on whether the pagination settings are valid or not.
    /// </summary>
    /// <typeparam name="TResult">
    /// The return type of the functions to be executed depending on the pagination state.
    /// </typeparam>
    /// <param name="ifPresent">
    /// A function to execute when the pagination settings are valid. This function is provided
    /// with the page number and page size as its arguments.
    /// </param>
    /// <param name="ifEmpty">
    /// A function to execute when the pagination settings are invalid (unpaged).
    /// </param>
    /// <returns>
    /// The result of either the <paramref name="ifPresent"/> or <paramref name="ifEmpty"/> function,
    /// depending on whether the pagination settings are valid.
    /// </returns>
    public TResult Match<TResult>(Func<int, int, TResult> ifPresent, Func<TResult> ifEmpty) {
        return _request.IsValid ? ifPresent(_request.PageNumber, _request.PageSize) : ifEmpty();
    }

    /// <summary>
    /// Defines an implicit conversion operator for <see cref="Pageable"/> to <see cref="Option{PageRequest}"/>.
    /// </summary>
    /// <remarks>
    /// This operator facilitates the conversion of a <see cref="Pageable"/> instance to an
    /// <see cref="Option{PageRequest}"/> object, enabling the encapsulation of a valid <see cref="PageRequest"/>
    /// or the representation of an unpaged state as none. The operation checks if the underlying
    /// <see cref="PageRequest"/> is valid and converts accordingly.
    /// </remarks>
    /// <param name="pageable">The <see cref="Pageable"/> instance to be converted.</param>
    /// <returns>
    /// An <see cref="Option{PageRequest}"/> that contains a valid <see cref="PageRequest"/> if the
    /// <see cref="Pageable"/> is configured for pagination, or an empty option if unpaged.
    /// </returns>
    public static implicit operator Option<PageRequest>(Pageable pageable) {
        return pageable._request.IsValid ? pageable._request : Option<PageRequest>.None;
    }

}