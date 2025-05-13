using UnrealPluginManager.Core.Database.Entities;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database;

/// <summary>
/// Provides extension methods for configuring database models with specific property mappings and behavior.
/// </summary>
public static class DbModelExtensions {
  /// <summary>
  /// Orders the elements of a sequence of <see cref="IVersionedEntity"/> by their semantic version in ascending order.
  /// This method sorts the versions considering the major, minor, patch values, as well as the prerelease
  /// and metadata components of the semantic version.
  /// </summary>
  /// <typeparam name="TSource">
  /// The type of the elements in the sequence. Must implement <see cref="IVersionedEntity"/>.
  /// </typeparam>
  /// <param name="source">
  /// The sequence of <typeparamref name="TSource"/> objects to order.
  /// </param>
  /// <returns>
  /// Returns an <see cref="IOrderedQueryable{T}"/> where the elements are sorted
  /// by semantic version in ascending order.
  /// </returns>
  public static IOrderedQueryable<TSource> OrderByVersion<TSource>(this IQueryable<TSource> source)
      where TSource : IVersionedEntity {
    return source.OrderBy(x => x.Major)
        .ThenBy(x => x.Minor)
        .ThenBy(x => x.Patch)
        .ThenBy(x => x.PrereleaseNumber == null)
        .ThenBy(x => x.PrereleaseNumber);
  }

  /// <summary>
  /// Orders the elements of a sequence of <typeparamref name="TSource"/> objects by their version in descending order.
  /// This method sorts based on the major, minor, patch, prerelease, and metadata components of the version, following
  /// semantic versioning precedence rules.
  /// </summary>
  /// <typeparam name="TSource">
  /// The type of the elements in the source sequence. It must implement <see cref="IVersionedEntity"/>.
  /// </typeparam>
  /// <param name="source">
  /// The <see cref="IQueryable{T}"/> sequence of <typeparamref name="TSource"/> objects to be ordered by version.
  /// </param>
  /// <returns>
  /// Returns an <see cref="IOrderedQueryable{T}"/> where the elements are ordered by their version in descending order.
  /// </returns>
  public static IOrderedQueryable<TSource> OrderByVersionDescending<TSource>(this IQueryable<TSource> source)
      where TSource : IVersionedEntity {
    return source.OrderByDescending(x => x.Major)
        .ThenByDescending(x => x.Minor)
        .ThenByDescending(x => x.Patch)
        .ThenByDescending(x => x.PrereleaseNumber == null)
        .ThenByDescending(x => x.PrereleaseNumber);
  }

  /// <summary>
  /// Retrieves the latest plugin versions from a sequence of <see cref="PluginVersion"/> objects.
  /// The latest version is determined based on the semantic versioning rules, comparing major, minor, and patch versions,
  /// as well as prerelease components when applicable.
  /// </summary>
  /// <param name="versions">
  /// The sequence of <see cref="PluginVersion"/> objects to evaluate for the latest versions.
  /// </param>
  /// <returns>
  /// Returns an <see cref="IQueryable{T}"/> of <see cref="PluginVersion"/> objects, where each plugin has its latest version included.
  /// </returns>
  public static IQueryable<PluginVersion> GetLatestVersions(this IQueryable<PluginVersion> versions) {
    return versions.Where(pv => !versions
                              .Any(other =>
                                       other.ParentId == pv.ParentId && (
                                           other.Major > pv.Major ||
                                           (other.Major == pv.Major && other.Minor > pv.Minor) ||
                                           (other.Major == pv.Major && other.Minor == pv.Minor &&
                                            other.Patch > pv.Patch) ||
                                           (other.Major == pv.Major && other.Minor == pv.Minor &&
                                            other.Patch == pv.Patch &&
                                            other.PrereleaseNumber != null && (pv.PrereleaseNumber == null ||
                                                                               other.PrereleaseNumber >
                                                                               pv.PrereleaseNumber)))));
  }
}