using System.IO.Abstractions;
using System.Text.Json;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Local.Model.Installation;
using static UnrealPluginManager.Core.Model.Resolution.ResolutionResult;
using InstallResult = UnrealPluginManager.Local.Model.Installation.InstallResult;

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

  /// <inheritdoc />
  public async Task<InstallResult> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion,
                                                 IReadOnlyCollection<string> platforms) {
    var existingVersion = await _engineService.GetInstalledPluginVersion(pluginName, engineVersion)
        .Map(x => x.Where(pluginVersion.Contains));
    return await existingVersion
        .Match(_ => Task.FromResult(InstallResult.NoChanges),
               () => TryInstall(pluginName, pluginVersion, engineVersion, platforms));
  }

  /// <inheritdoc />
  public async Task<InstallResult> InstallRequirements(string descriptorFile, string? engineVersion, IReadOnlyCollection<string> platforms) {
    IDependencyHolder? descriptor;
    await using (var stream = _fileSystem.File.OpenRead(descriptorFile)) {
      descriptor = descriptorFile.EndsWith(".uplugin") ? await JsonSerializer.DeserializeAsync<PluginDescriptor>(stream) 
          : await JsonSerializer.DeserializeAsync<ProjectDescriptor>(stream);
      ArgumentNullException.ThrowIfNull(descriptor);
    }

    var chainRoot = descriptor.ToDependencyChainRoot();
    var dependencyTree = await _pluginManagementService.GetPluginsToInstall(chainRoot, engineVersion);
    return await dependencyTree.Match(x => InstallToEngine(x, engineVersion, platforms),
                                      x => Task.FromResult<InstallResult>(x.Conflicts));
  }

  private async Task<InstallResult> TryInstall(string name, SemVersionRange version, string? engineVersion,
                                               IReadOnlyCollection<string> platforms) {
    var targetPlugin = await _pluginManagementService.FindTargetPlugin(name, version, engineVersion);
    var dependencyTree = await _pluginManagementService.GetPluginsToInstall(targetPlugin, engineVersion);
    return await dependencyTree.Match(x => InstallToEngine(x, engineVersion, platforms),
                                      x => Task.FromResult<InstallResult>(x.Conflicts));
  }
  
  private async Task<InstallResult> InstallToEngine(ResolvedDependencies resolvedDependencies, string? engineVersion,
                                                    IReadOnlyCollection<string> platforms) {
    var currentlyInstalled = await _engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => (x.Version, x.Platforms));

    var installChanges = new List<VersionChange>();
    foreach (var dep in resolvedDependencies.SelectedPlugins) {
      if (currentlyInstalled.TryGetValue(dep.Name, out var current) && current.Version == dep.Version 
                                                                    && platforms.All(x => current.Platforms.Contains(x))) {
        continue;
      }

      await _engineService.InstallPlugin(dep.Name, dep.Version, engineVersion, platforms);
      installChanges.Add(new VersionChange(dep.Name, current.Version, dep.Version));
    }

    return installChanges;
  }
}