using System.Text.RegularExpressions;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for working with patterns similar to globbing,
/// such as checking if strings match a specified pattern.
/// </summary>
public static class GlobUtils {

  /// <summary>
  /// Determines whether the specified string matches the given pattern using glob-like syntax.
  /// Supports '*' as a wildcard matching zero or more characters and '?' as a wildcard matching a single character.
  /// </summary>
  /// <param name="str">The string to test against the pattern.</param>
  /// <param name="pattern">The glob-like pattern to test the string against.</param>
  /// <returns>A boolean value indicating whether the string matches the pattern.</returns>
  public static bool Like(this string str, string pattern) {
    var regex = new Regex($"^{Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".")}$", 
                          RegexOptions.None, TimeSpan.FromMilliseconds(100));
    return regex.IsMatch(str);
  }

}