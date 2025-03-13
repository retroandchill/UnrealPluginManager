using System.IO.Abstractions;
using System.Text.Json;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
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
[AutoConstructor]
public partial class InstallService : IInstallService {
  private readonly IFileSystem _fileSystem;
  private readonly IEngineService _engineService;
  private readonly IPluginManagementService _pluginManagementService;
  private readonly IJsonService _jsonService;

  /// <inheritdoc />
  public async Task<List<VersionChange>> InstallPlugin(string pluginName, SemVersionRange pluginVersion,
                                                       string? engineVersion,
                                                       IReadOnlyCollection<string> platforms) {
    var existingVersion = await _engineService.GetInstalledPluginVersion(pluginName, engineVersion)
        .Map(x => x.Where(pluginVersion.Contains));
    return await existingVersion
        .Select(_ => new List<VersionChange>())
        .OrElseGetAsync(() => TryInstall(pluginName, pluginVersion, engineVersion, platforms));
  }

  /// <inheritdoc />
  public async Task<List<VersionChange>> InstallRequirements(string descriptorFile, string? engineVersion,
                                                             IReadOnlyCollection<string> platforms) {
    IDependencyHolder? descriptor;
    await using (var stream = _fileSystem.File.OpenRead(descriptorFile)) {
      descriptor = descriptorFile.EndsWith(".uplugin") ? await _jsonService.DeserializeAsync<PluginDescriptor>(stream) 
          : await _jsonService.DeserializeAsync<ProjectDescriptor>(stream);
      ArgumentNullException.ThrowIfNull(descriptor);
    }

    var chainRoot = descriptor.ToDependencyChainRoot();
    var dependencyTree = await _pluginManagementService.GetPluginsToInstall(chainRoot, engineVersion);
    return await InstallToEngine(dependencyTree, engineVersion, platforms);
  }

  private async Task<List<VersionChange>> TryInstall(string name, SemVersionRange version, string? engineVersion,
                                                     IReadOnlyCollection<string> platforms) {
    var targetPlugin = await _pluginManagementService.FindTargetPlugin(name, version, engineVersion);
    var dependencyTree = await _pluginManagementService.GetPluginsToInstall(targetPlugin, engineVersion);
    return await InstallToEngine(dependencyTree, engineVersion, platforms);
  }
  
  private async Task<List<VersionChange>> InstallToEngine(List<PluginSummary> resolvedDependencies, string? engineVersion,
                                                          IReadOnlyCollection<string> platforms) {
    var currentlyInstalled = await _engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => (x.Version, x.Platforms));

    var installChanges = new List<VersionChange>();
    foreach (var dep in resolvedDependencies) {
      if (currentlyInstalled.TryGetValue(dep.Name, out var current) && current.Version == dep.Version 
                                                                    && platforms.All(x => current.Platforms.Contains(x))) {
        continue;
      }
      
      var currentVersion = _engineService.GetInstalledEngine(engineVersion);
      ArgumentNullException.ThrowIfNull(currentVersion);
      var cachedPlugin = await _pluginManagementService.FindLocalPlugin(dep.Name, dep.Version);
      if (cachedPlugin.IsNone) {
        var foundPlugin = await _pluginManagementService.DownloadPlugin(dep.Name, dep.Version, dep.RemoteIndex,
            currentVersion.Name, platforms.ToList());

        var missingPlatforms = foundPlugin.Binaries
            .Where(x => x.EngineVersion == currentVersion.Name)
            .Select(x => x.Platform)
            .Except(platforms)
            .ToList();
        if (missingPlatforms.Count > 0) {
          // TODO: Build for missing platforms
          throw new PlatformNotSupportedException(
              $"Unable to install plugin {dep.Name} with version {dep.Version} on platforms {string.Join(", ", missingPlatforms)}.");
        }
      }

      await _engineService.InstallPlugin(dep.Name, dep.Version, engineVersion, platforms);
      installChanges.Add(new VersionChange(dep.Name, current.Version, dep.Version));
    }

    return installChanges;
  }
}