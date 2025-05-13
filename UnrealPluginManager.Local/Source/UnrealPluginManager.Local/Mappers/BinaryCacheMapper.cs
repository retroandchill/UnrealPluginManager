using Riok.Mapperly.Abstractions;
using Semver;
using UnrealPluginManager.Local.Database.Building;
using UnrealPluginManager.Local.Model.Cache;

namespace UnrealPluginManager.Local.Mappers;

/// <summary>
/// Provides mapping functionality to convert between PluginBuild and PluginBuildInfo objects.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class BinaryCacheMapper {
  /// Converts a PluginBuild instance to a PluginBuildInfo instance by mapping relevant properties.
  /// <param name="pluginBuild">
  /// The PluginBuild object containing the plugin build information to be converted.
  /// </param>
  /// <returns>
  /// A new PluginBuildInfo object with the mapped properties from the provided PluginBuild instance.
  /// </returns>
  [MapProperty(nameof(@PluginBuild.PluginVersion.Parent.Name), nameof(PluginBuildInfo.PluginName))]
  [MapProperty(nameof(PluginBuild.PluginVersion.Version), nameof(PluginBuildInfo.PluginVersion))]
  public static partial PluginBuildInfo ToPluginBuildInfo(this PluginBuild pluginBuild);

  private static List<string> GetPlatforms(this ICollection<PluginBuildPlatform> platforms) {
    return platforms.Select(pb => pb.Platform).ToList();
  }

  private static Dictionary<string, SemVersion> GetBuiltWith(this ICollection<DependencyBuildVersion> pluginBuild) {
    return pluginBuild.ToDictionary(pb => pb.Dependency.PluginName, pb => pb.Version);
  }
}