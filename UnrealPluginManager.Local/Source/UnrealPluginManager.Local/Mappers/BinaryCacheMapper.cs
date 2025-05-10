using Riok.Mapperly.Abstractions;
using Semver;
using UnrealPluginManager.Local.Model.Cache;
using UnrealPluginManager.Server.Database.Building;

namespace UnrealPluginManager.Local.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class BinaryCacheMapper {

  [MapProperty(nameof(@PluginBuild.PluginVersion.Parent.Name), nameof(PluginBuildInfo.PluginName))]
  [MapProperty(nameof(PluginBuild.PluginVersion.Version), nameof(PluginBuildInfo.PluginVersion))]
  public static partial PluginBuildInfo ToPluginBuildInfo(this PluginBuild pluginBuild);

  private static List<string> GetPlatforms(this ICollection<PluginBuildPlatform> platforms) {
    return platforms.Select(pb => pb.Platform).ToList();
  }

  private static Dictionary<string, SemVersion> GetBuiltWith(this ICollection<DependencyBuildVersion> pluginBuild) {
    return pluginBuild.ToDictionary(pb => pb.Dependency.PluginName, pb => pb.Version);
  }

  public static partial IQueryable<PluginBuildInfo> ToPluginBuildInfoQuery(this IQueryable<PluginBuild> pluginBuilds);

}