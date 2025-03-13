using Riok.Mapperly.Abstractions;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Utils;

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
  /// Converts an <see cref="IQueryable{Plugin}"/> to an <see cref="IQueryable{PluginOverview}"/> representation.
  /// </summary>
  /// <param name="plugins">The source <see cref="IQueryable{Plugin}"/> collection to be converted.</param>
  /// <returns>An <see cref="IQueryable{PluginOverview}"/> collection representing the provided <see cref="IQueryable{Plugin}"/>.</returns>
  public static partial IQueryable<PluginOverview> ToPluginOverview(this IQueryable<Plugin> plugins);

  /// <summary>
  /// Converts a <see cref="Plugin"/> instance to a <see cref="PluginDetails"/> representation.
  /// </summary>
  /// <param name="plugin">The <see cref="Plugin"/> instance to be converted.</param>
  /// <returns>A <see cref="PluginDetails"/> object representing the provided <see cref="Plugin"/>.</returns>
  public static partial PluginDetails ToPluginDetails(this Plugin plugin);

  /// <summary>
  /// Converts a <see cref="Plugin"/> entity and its associated <see cref="PluginVersion"/> to a <see cref="PluginDetails"/> representation.
  /// </summary>
  /// <param name="plugin">The <see cref="Plugin"/> entity to be converted.</param>
  /// <param name="versions">The associated <see cref="PluginVersion"/> data to be included in the conversion.</param>
  /// <returns>A <see cref="PluginDetails"/> instance representing the provided <see cref="Plugin"/> entity along with its version details.</returns>
  public static partial PluginDetails ToPluginDetails(this Plugin plugin, PluginVersion versions);

  /// <summary>
  /// Maps a <see cref="PluginVersion"/> to a list of <see cref="VersionDetails"/> instances.
  /// </summary>
  /// <param name="version">The <see cref="PluginVersion"/> instance containing version information.</param>
  /// <returns>A <see cref="List{T}"/> of <see cref="VersionDetails"/> objects based on the provided <see cref="PluginVersion"/>.</returns>
  public static List<VersionDetails> MapVersionDetails(PluginVersion version) {
    return version.ToEnumerable().Select(ToVersionDetails).ToList();
  }

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
  [MapProperty(nameof(PluginVersion.Parent.FriendlyName), nameof(PluginVersionInfo.FriendlyName))]
  [MapProperty(nameof(PluginVersion.Id), nameof(PluginVersionInfo.VersionId))]
  [MapperIgnoreTarget(nameof(PluginVersionInfo.Installed))]
  [MapperIgnoreTarget(nameof(PluginVersionInfo.RemoteIndex))]
  public static partial PluginVersionInfo ToPluginVersionInfo(this PluginVersion version);

  /// <summary>
  /// Converts an <see cref="IQueryable{PluginVersion}"/> to an <see cref="IQueryable{PluginVersionInfo}"/> representation.
  /// </summary>
  /// <param name="versions">The source <see cref="IQueryable{PluginVersion}"/> collection to be converted.</param>
  /// <returns>An <see cref="IQueryable{PluginVersionInfo}"/> collection representing the provided <see cref="IQueryable{PluginVersion}"/>.</returns>
  public static partial IQueryable<PluginVersionInfo> ToPluginVersionInfo(this IQueryable<PluginVersion> versions);
  
  [MapProperty(nameof(PluginVersion.ParentId), nameof(PluginVersionDetails.PluginId))]
  [MapProperty(nameof(PluginVersion.Parent.Name), nameof(PluginVersionDetails.Name))]
  [MapProperty(nameof(PluginVersion.Parent.FriendlyName), nameof(PluginVersionDetails.FriendlyName))]
  [MapProperty(nameof(PluginVersion.Id), nameof(PluginVersionDetails.VersionId))]
  [MapProperty(nameof(PluginVersion.Parent.Description), nameof(PluginVersionDetails.Description))]
  [MapperIgnoreTarget(nameof(PluginVersionDetails.Installed))]
  [MapperIgnoreTarget(nameof(PluginVersionDetails.RemoteIndex))]
  public static partial PluginVersionDetails ToPluginVersionDetails(this PluginVersion version);
  
  public static partial IQueryable<PluginVersionDetails> ToPluginVersionDetails(this IQueryable<PluginVersion> versions);

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
  /// Maps a <see cref="PluginDescriptor"/> object to a <see cref="Plugin"/> object using the specified plugin name and optional icon.
  /// </summary>
  /// <param name="descriptor">The <see cref="PluginDescriptor"/> object containing source data.</param>
  /// <param name="name">The name of the plugin to assign in the created <see cref="Plugin"/> instance.</param>
  /// <returns>A new <see cref="Plugin"/> object mapped from the provided <see cref="PluginDescriptor"/>, name, and optional icon.</returns>
  [MapperIgnoreTarget(nameof(Plugin.Id))]
  [MapperIgnoreTarget(nameof(Plugin.Versions))]
  [MapProperty(nameof(PluginDescriptor.CreatedBy), nameof(Plugin.AuthorName))]
  [MapProperty(nameof(PluginDescriptor.CreatedByUrl), nameof(Plugin.AuthorWebsite))]
  public static partial Plugin ToPlugin(this PluginDescriptor descriptor, string name);

  /// <summary>
  /// Maps a <see cref="PluginDescriptor"/> object to a <see cref="PluginVersion"/> object.
  /// </summary>
  /// <param name="descriptor">The <see cref="PluginDescriptor"/> object containing source data to be mapped.</param>
  /// <returns>A new <see cref="PluginVersion"/> object populated with data from the provided <see cref="PluginDescriptor"/> object.</returns>
  [MapperIgnoreTarget(nameof(PluginVersion.Id))]
  [MapperIgnoreTarget(nameof(PluginVersion.Parent))]
  [MapperIgnoreTarget(nameof(PluginVersion.ParentId))]
  [MapperIgnoreTarget(nameof(PluginVersion.Binaries))]
  [MapProperty(nameof(PluginDescriptor.VersionName), nameof(PluginVersion.Version))]
  [MapProperty(nameof(PluginDescriptor.Plugins), nameof(PluginVersion.Dependencies))]
  public static partial PluginVersion ToPluginVersion(this PluginDescriptor descriptor);

  /// <summary>
  /// Maps a <see cref="PluginReferenceDescriptor"/> object to a <see cref="Dependency"/> object with the specified properties transformed accordingly.
  /// </summary>
  /// <param name="descriptor">The <see cref="PluginReferenceDescriptor"/> object containing source data for mapping.</param>
  /// <returns>A new <see cref="Dependency"/> object mapped from the provided <see cref="PluginReferenceDescriptor"/> instance.</returns>
  [MapperIgnoreTarget(nameof(Dependency.Id))]
  [MapperIgnoreTarget(nameof(Dependency.Parent))]
  [MapperIgnoreTarget(nameof(Dependency.ParentId))]
  [MapProperty(nameof(PluginReferenceDescriptor.Name), nameof(Dependency.PluginName))]
  [MapProperty(nameof(PluginReferenceDescriptor.PluginType), nameof(Dependency.Type))]
  [MapProperty(nameof(PluginReferenceDescriptor.VersionMatcher), nameof(Dependency.PluginVersion))]
  public static partial Dependency ToDependency(this PluginReferenceDescriptor descriptor);

  /// <summary>
  /// Maps a <see cref="Dependency"/> entity to a <see cref="PluginDependency"/> representation.
  /// </summary>
  /// <param name="dependency">The source <see cref="Dependency"/> entity to be mapped.</param>
  /// <returns>A <see cref="PluginDependency"/> instance representing the provided <see cref="Dependency"/> entity.</returns>
  public static partial PluginDependency ToPluginDependency(this Dependency dependency);
  
  public static partial PluginDependency ToPluginDependency(this DependencyOverview dependency);

  /// <summary>
  /// Maps a <see cref="PluginReferenceDescriptor"/> to a <see cref="PluginDependency"/> representation.
  /// </summary>
  /// <param name="descriptor">The source <see cref="PluginReferenceDescriptor"/> to be mapped.</param>
  /// <returns>A <see cref="PluginDependency"/> instance representing the provided <see cref="PluginReferenceDescriptor"/>.</returns>
  [MapProperty(nameof(PluginReferenceDescriptor.Name), nameof(Dependency.PluginName))]
  [MapProperty(nameof(PluginReferenceDescriptor.PluginType), nameof(Dependency.Type))]
  [MapProperty(nameof(PluginReferenceDescriptor.VersionMatcher), nameof(Dependency.PluginVersion))]
  public static partial PluginDependency ToPluginDependency(this PluginReferenceDescriptor descriptor);

  /// <summary>
  /// Maps a <see cref="PluginBinaryType"/> to an <see cref="UploadedBinaries"/> representation.
  /// </summary>
  /// <param name="descriptor">The <see cref="PluginBinaryType"/> instance to be converted.</param>
  /// <returns>An <see cref="UploadedBinaries"/> object created based on the provided <see cref="PluginBinaryType"/>.</returns>
  [MapperIgnoreTarget(nameof(UploadedBinaries.Id))]
  [MapperIgnoreTarget(nameof(UploadedBinaries.ParentId))]
  [MapperIgnoreTarget(nameof(UploadedBinaries.Parent))]
  public static partial UploadedBinaries ToUploadedBinaries(this PluginBinaryType descriptor);

  /// <summary>
  /// Converts an <see cref="IDependencyHolder"/> instance to a <see cref="DependencyChainRoot"/> representation.
  /// </summary>
  /// <param name="descriptor">The source <see cref="IDependencyHolder"/> containing plugin dependency information.</param>
  /// <returns>A <see cref="DependencyChainRoot"/> instance representing the dependency chain of the provided <see cref="IDependencyHolder"/>.</returns>
  [MapProperty(nameof(IDependencyHolder.Plugins), nameof(DependencyChainRoot.Dependencies))]
  public static partial DependencyChainRoot ToDependencyChainRoot(this IDependencyHolder descriptor);

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