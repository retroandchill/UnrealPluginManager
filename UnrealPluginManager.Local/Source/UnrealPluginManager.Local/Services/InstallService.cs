using System.IO.Abstractions;
using LanguageExt;
using Retro.ReadOnlyParams.Annotations;
using Semver;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Model.Installation;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides services for installing plugins and their dependencies in Unreal Engine projects.
/// The <see cref="InstallService"/> class implements the <see cref="IInstallService"/> interface to
/// execute plugin installation operations and resolve dependencies.
/// </summary>
public class InstallService(
    [ReadOnly] IFileSystem fileSystem,
    [ReadOnly] IEngineService engineService,
    [ReadOnly] IPluginManagementService pluginManagementService,
    [ReadOnly] IJsonService jsonService) : IInstallService {
  /// <inheritdoc />
  public async Task<List<VersionChange>> InstallPlugin(string pluginName, SemVersionRange pluginVersion,
                                                       string? engineVersion,
                                                       IReadOnlyCollection<string> platforms) {
    var existingVersion = await engineService.GetInstalledPluginVersion(pluginName, engineVersion)
        .Map(x => x.Where(pluginVersion.Contains));
    return await existingVersion
        .Select(_ => new List<VersionChange>())
        .OrElseGetAsync(() => TryInstall(pluginName, pluginVersion, engineVersion, platforms));
  }

  /// <inheritdoc />
  public async Task<List<VersionChange>> InstallRequirements(string descriptorFile, string? engineVersion,
                                                             IReadOnlyCollection<string> platforms) {
    IDependencyHolder? descriptor;
    await using (var stream = fileSystem.File.OpenRead(descriptorFile)) {
      descriptor = descriptorFile.EndsWith(".uplugin")
          ? await jsonService.DeserializeAsync<PluginDescriptor>(stream)
          : await jsonService.DeserializeAsync<ProjectDescriptor>(stream);
      ArgumentNullException.ThrowIfNull(descriptor);
    }

    var chainRoot = descriptor.ToDependencyChainRoot();
    var dependencyTree = await pluginManagementService.GetPluginsToInstall(chainRoot, engineVersion);
    return await InstallToEngine(dependencyTree, engineVersion, platforms);
  }

  /// <inheritdoc />
  public async Task<List<VersionChange>> InstallRequirements(PluginManifest manifest, string? engineVersion,
                                                             IReadOnlyCollection<string> platforms) {
    var chainRoot = manifest.ToDependencyChainRoot();
    var dependencyTree = await pluginManagementService.GetPluginsToInstall(chainRoot, engineVersion);
    return await InstallToEngine(dependencyTree, engineVersion, platforms);
  }

  private async Task<List<VersionChange>> TryInstall(string name, SemVersionRange version, string? engineVersion,
                                                     IReadOnlyCollection<string> platforms) {
    var targetPlugin = await pluginManagementService.FindTargetPlugin(name, version, engineVersion);
    var dependencyTree = await pluginManagementService.GetPluginsToInstall(targetPlugin, engineVersion);
    return await InstallToEngine(dependencyTree, engineVersion, platforms);
  }

  private async Task<List<VersionChange>> InstallToEngine(List<PluginSummary> resolvedDependencies,
                                                          string? engineVersion,
                                                          IReadOnlyCollection<string> platforms) {
    var currentlyInstalled = await engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => (x.Version, x.Platforms));

    var installChanges = new List<VersionChange>();
    foreach (var dep in resolvedDependencies) {
      if (currentlyInstalled.TryGetValue(dep.Name, out var current) && current.Version == dep.Version
                                                                    && platforms.All(x =>
                                                                        current.Platforms.Contains(x))) {
        continue;
      }

      var currentVersion = engineService.GetInstalledEngine(engineVersion);
      ArgumentNullException.ThrowIfNull(currentVersion);
      var cachedPlugin =
          await pluginManagementService.FindLocalPlugin(dep.Name, dep.Version, currentVersion.Name, platforms);
      var plugin = await cachedPlugin.OrElseGetAsync(async () =>
                                                         await pluginManagementService.DownloadPlugin(
                                                             dep.Name, dep.Version, dep.RemoteIndex,
                                                             currentVersion.Name, platforms.ToList()));

      engineService.InstallPlugin(plugin.PluginName, fileSystem.DirectoryInfo.New(plugin.DirectoryName),
                                  engineVersion);
      installChanges.Add(new VersionChange(dep.Name, current.Version, dep.Version));
    }

    return installChanges;
  }
}