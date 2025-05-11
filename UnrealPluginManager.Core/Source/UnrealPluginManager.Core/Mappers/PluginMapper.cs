using Riok.Mapperly.Abstractions;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Core.Mappers;

/// <summary>
/// A static partial class responsible for mapping between
/// data transfer objects (DTOs) and database entities related to plugins.
/// Provides mappings to transform PluginDescriptor and PluginReferenceDescriptor
/// objects into corresponding Plugin, PluginVersion, and Dependency entities.
/// Utilizes Riok.Mapperly library for advanced mapping configurations.
/// </summary>
[Mapper(UseDeepCloning = true, RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class PluginMapper {

  /// <summary>
  /// Maps a <see cref="PluginVersion"/> entity to its <see cref="PluginSummary"/> representation.
  /// </summary>
  /// <param name="version">The <see cref="PluginVersion"/> instance to be mapped to a <see cref="PluginSummary"/>.</param>
  /// <returns>A <see cref="PluginSummary"/> representing the provided <see cref="PluginVersion"/>.</returns>
  public static partial PluginSummary ToPluginSummary(this PluginVersionInfo version);

  /// <summary>
  /// Retrieves the semantic version of a plugin from the provided <see cref="PluginVersion"/> instance.
  /// </summary>
  /// <param name="plugin">The <see cref="PluginVersion"/> instance containing the version information.</param>
  /// <returns>A <see cref="SemVersion"/> representing the plugin's version.</returns>
  public static SemVersion GetPluginVersion(this PluginVersion plugin) => plugin.Version;

  /// <summary>
  /// Converts a <see cref="PluginVersion"/> instance to a <see cref="PluginVersionInfo"/> representation.
  /// </summary>
  /// <param name="version">The source <see cref="PluginVersion"/> to be converted.</param>
  /// <returns>A <see cref="PluginVersionInfo"/> representing the provided <see cref="PluginVersion"/>.</returns>
  [MapProperty(nameof(PluginVersion.ParentId), nameof(PluginVersionInfo.PluginId))]
  [MapProperty(nameof(PluginVersion.Parent.Name), nameof(PluginVersionInfo.Name))]
  [MapProperty(nameof(PluginVersion.Id), nameof(PluginVersionInfo.VersionId))]
  [MapperIgnoreTarget(nameof(PluginVersionInfo.Installed))]
  [MapperIgnoreTarget(nameof(PluginVersionInfo.RemoteIndex))]
  public static partial PluginVersionInfo ToPluginVersionInfo(this PluginVersion version);

  /// <summary>
  /// Converts a <see cref="PluginManifest"/> instance to a <see cref="PluginVersion"/> representation.
  /// </summary>
  /// <param name="manifest">The <see cref="PluginManifest"/> instance to be converted.</param>
  /// <returns>A <see cref="PluginVersion"/> object representing the provided <see cref="PluginManifest"/>.</returns>
  public static partial PluginVersion ToPluginVersion(this PluginManifest manifest);

  public static partial PluginManifest ToPluginManifest(this PluginVersionInfo version);

  /// <summary>
  /// Converts a <see cref="PluginDependencyManifest"/> instance to a <see cref="Dependency"/> representation.
  /// </summary>
  /// <param name="manifest">The <see cref="PluginDependencyManifest"/> instance to be converted.</param>
  /// <returns>A <see cref="Dependency"/> object representing the provided <see cref="PluginDependencyManifest"/>.</returns>
  [MapProperty(nameof(PluginDependencyManifest.Name), nameof(Dependency.PluginName))]
  [MapProperty(nameof(PluginDependencyManifest.Version), nameof(Dependency.PluginVersion))]
  [MapperIgnoreTarget(nameof(Dependency.Id))]
  [MapperIgnoreTarget(nameof(Dependency.Parent))]
  [MapperIgnoreTarget(nameof(Dependency.ParentId))]
  public static partial Dependency ToDependency(this PluginDependencyManifest manifest);

  [MapProperty(nameof(PluginDependency.PluginName), nameof(PluginDependencyManifest.Name))]
  [MapProperty(nameof(PluginDependency.PluginVersion), nameof(PluginDependencyManifest.Version))]
  public static partial PluginDependencyManifest ToPluginDependencyManifest(this PluginDependency dependency);

  /// <summary>
  /// Converts a given <see cref="PluginVersion"/> to a <see cref="VersionOverview"/> representation.
  /// </summary>
  /// <param name="version">The source <see cref="PluginVersion"/> instance to be converted.</param>
  /// <returns>A <see cref="VersionOverview"/> object representing the provided <see cref="PluginVersion"/>.</returns>
  public static partial VersionOverview ToVersionOverview(this PluginVersion version);

  /// <summary>
  /// Converts a <see cref="PluginVersion"/> to a <see cref="VersionOverview"/> representation.
  /// </summary>
  /// <param name="versions">The source <see cref="PluginVersion"/> instance to be converted.</param>
  /// <returns>A <see cref="VersionOverview"/> object representing the provided <see cref="PluginVersion"/>.</returns>
  public static List<VersionOverview> ToVersionOverview(this ICollection<PluginVersion> versions) {
    return versions.OrderByDescending(x => x.Major)
        .ThenByDescending(x => x.Minor)
        .ThenByDescending(x => x.Patch)
        .ThenByDescending(x => x.PrereleaseNumber == null)
        .ThenByDescending(x => x.PrereleaseNumber)
        .Select(x => x.ToVersionOverview())
        .ToList();
  }

  /// <summary>
  /// Converts a <see cref="PluginVersion"/> instance into its corresponding <see cref="VersionDetails"/> representation.
  /// </summary>
  /// <param name="version">The <see cref="PluginVersion"/> instance to be converted.</param>
  /// <returns>A <see cref="VersionDetails"/> instance representing the provided <see cref="PluginVersion"/>.</returns>
  public static partial VersionDetails ToVersionDetails(this PluginVersion version);

  /// <summary>
  /// Maps a <see cref="Dependency"/> entity to a <see cref="PluginDependency"/> representation.
  /// </summary>
  /// <param name="dependency">The source <see cref="Dependency"/> entity to be mapped.</param>
  /// <returns>A <see cref="PluginDependency"/> instance representing the provided <see cref="Dependency"/> entity.</returns>
  public static partial PluginDependency ToPluginDependency(this Dependency dependency);

  /// <summary>
  /// Converts an <see cref="IDependencyHolder"/> instance to a <see cref="DependencyChainRoot"/> representation.
  /// </summary>
  /// <param name="descriptor">The source <see cref="IDependencyHolder"/> containing plugin dependency information.</param>
  /// <returns>A <see cref="DependencyChainRoot"/> instance representing the dependency chain of the provided <see cref="IDependencyHolder"/>.</returns>
  [MapProperty(nameof(IDependencyHolder.Plugins), nameof(DependencyChainRoot.Dependencies))]
  public static partial DependencyChainRoot ToDependencyChainRoot(this IDependencyHolder descriptor);

  public static partial DependencyChainRoot ToDependencyChainRoot(this PluginManifest manifest);

  [MapProperty(nameof(PluginReferenceDescriptor.Name), nameof(PluginDependency.PluginName))]
  [MapProperty(nameof(PluginReferenceDescriptor.VersionMatcher), nameof(PluginDependency.PluginVersion))]
  public static partial PluginDependency ToPluginDependency(this PluginReferenceDescriptor manifest);

  [MapProperty(nameof(PluginDependencyManifest.Name), nameof(PluginDependency.PluginName))]
  [MapProperty(nameof(PluginDependencyManifest.Version), nameof(PluginDependency.PluginVersion))]
  public static partial PluginDependency ToPluginDependency(this PluginDependencyManifest manifest);

  /// <summary>
  /// Converts an <see cref="IQueryable{Plugin}"/> to an <see cref="IQueryable{PluginOverview}"/> representation.
  /// </summary>
  /// <param name="plugins">The source <see cref="IQueryable{Plugin}"/> collection to be converted.</param>
  /// <returns>An <see cref="IQueryable{PluginOverview}"/> collection representing the provided <see cref="IQueryable{Plugin}"/>.</returns>
  public static partial IQueryable<PluginOverview> ToPluginOverviewQuery(this IQueryable<Plugin> plugins);

  /// <summary>
  /// Converts an <see cref="IQueryable{PluginVersion}"/> to an <see cref="IQueryable{PluginVersionInfo}"/> representation.
  /// </summary>
  /// <param name="versions">The source <see cref="IQueryable{PluginVersion}"/> collection to be converted.</param>
  /// <returns>An <see cref="IQueryable{PluginVersionInfo}"/> collection representing the provided <see cref="IQueryable{PluginVersion}"/>.</returns>
  public static partial IQueryable<PluginVersionInfo> ToPluginVersionInfoQuery(this IQueryable<PluginVersion> versions);

  /// <summary>
  /// Returns a <see cref="SemVersion"/> object without any modifications.
  /// </summary>
  /// <param name="version">The <see cref="SemVersion"/> object to be returned.</param>
  /// <returns>The same <see cref="SemVersion"/> object provided as input.</returns>
  private static SemVersion PassSemVersion(SemVersion version) => version;

  /// <summary>
  /// Passes through the provided <see cref="SemVersionRange"/> object without modification.
  /// </summary>
  /// <param name="version">The <see cref="SemVersionRange"/> object to be passed through.</param>
  /// <returns>The same <see cref="SemVersionRange"/> object that was provided as input.</returns>
  private static SemVersionRange PassSemVersionRange(SemVersionRange version) => version;
}