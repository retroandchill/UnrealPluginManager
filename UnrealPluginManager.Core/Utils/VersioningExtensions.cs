using Semver;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides extension methods for versioning functionalities, particularly related to
/// handling and extracting information from semantic versioning components.
/// </summary>
public static class VersioningExtensions {

    /// <summary>
    /// Retrieves the numeric value of the second prerelease identifier in a semantic version.
    /// The method validates that exactly two prerelease identifiers are present and ensures
    /// the second identifier contains a numeric value.
    /// </summary>
    /// <param name="identifiers">
    /// A read-only list of prerelease identifiers extracted from a semantic version.
    /// </param>
    /// <returns>
    /// The numeric value of the second prerelease identifier as an integer.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the number of prerelease identifiers is not equal to two.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the second prerelease identifier does not contain a numeric value.
    /// </exception>
    public static int GetPrereleaseNumber(this IReadOnlyList<PrereleaseIdentifier> identifiers) {
        ArgumentOutOfRangeException.ThrowIfNotEqual(identifiers.Count, 2);
        var numericValue = identifiers[1].NumericValue;
        return numericValue is not null ? (int) numericValue : throw new ArgumentException("Invalid release candidate number.");
    }
    
}