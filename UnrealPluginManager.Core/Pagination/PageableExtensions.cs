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
    
}