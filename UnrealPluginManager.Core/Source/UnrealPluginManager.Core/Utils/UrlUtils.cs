namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for handling and manipulating URLs.
/// </summary>
public static class UrlUtils {

  /// <summary>
  /// Converts the given <see cref="Uri"/> to a string representation without a trailing slash.
  /// </summary>
  /// <param name="uri">
  /// The <see cref="Uri"/> to be converted. This parameter must not be null.
  /// </param>
  /// <returns>
  /// A string representation of the <paramref name="uri"/> without a trailing slash.
  /// </returns>
  public static string ToStringWithoutTrailingSlash(this Uri uri) {
    return uri.ToString().TrimEnd('/');
  }
  
}