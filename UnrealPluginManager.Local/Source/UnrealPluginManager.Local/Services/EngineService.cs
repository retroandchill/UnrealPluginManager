using System.IO.Abstractions;
using LanguageExt;
using Retro.ReadOnlyParams.Annotations;
using Semver;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Model.Engine;
using UnrealPluginManager.Local.Model.Plugins;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides services related to managing Unreal Engine installations.
/// </summary>
public class EngineService(
    [ReadOnly] IFileSystem fileSystem,
    [ReadOnly] IProcessRunner processRunner,
    [ReadOnly] IJsonService jsonService,
    [ReadOnly] IEnginePlatformService enginePlatformService,
    [ReadOnly] IPluginStructureService pluginStructureService) : IEngineService {
  /// <inheritdoc />
  public InstalledEngine GetInstalledEngine(string? engineVersion) {
    var installedEngines = GetInstalledEngines();
    var installedEngine = engineVersion is not null
        ? installedEngines.First(x => x.Name == engineVersion)
        : installedEngines.Where(x => !x.CustomBuild)
            .OrderByDescending(x => x.Version)
            .First();
    return installedEngine;
  }

  /// <inheritdoc />
  public List<InstalledEngine> GetInstalledEngines() {
    return enginePlatformService.GetInstalledEngines();
  }

  /// <inheritdoc />
  public async Task<int> BuildPlugin(IFileInfo pluginFile,
                                     IDirectoryInfo destination,
                                     string? engineVersion,
                                     IReadOnlyCollection<string> platforms) {
    var installedEngine = GetInstalledEngine(engineVersion);

    var scriptPath = Path.Join(installedEngine.BatchFilesDirectory,
                               $"RunUAT.{enginePlatformService.ScriptFileExtension}");

    return await processRunner.RunProcess(scriptPath, [
        "BuildPlugin",
        $"-Plugin=\"{pluginFile.FullName}\"",
        $"-package=\"{destination.FullName}\""
    ]);
  }

  /// <inheritdoc />
  public async Task<Option<SemVersion>> GetInstalledPluginVersion(string pluginName, string? engineVersion) {
    var installedEngine = GetInstalledEngine(engineVersion);
    var installDirectory = Path.Join(installedEngine.PackageDirectory, pluginName);
    var upluginFile = Path.Join(installDirectory, $"{pluginName}.uplugin");
    if (!fileSystem.Directory.Exists(installDirectory) || !fileSystem.File.Exists(upluginFile)) {
      return Option<SemVersion>.None;
    }

    var upluginInfo = fileSystem.FileInfo.New(upluginFile);
    await using var reader = upluginInfo.OpenRead();
    var pluginDescriptor = await jsonService.DeserializeAsync<PluginDescriptor>(reader);
    ArgumentNullException.ThrowIfNull(pluginDescriptor);
    return pluginDescriptor.VersionName;
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<InstalledPlugin> GetInstalledPlugins(string? engineVersion) {
    var installedEngine = GetInstalledEngine(engineVersion);
    var packageDirectory = fileSystem.DirectoryInfo.New(installedEngine.PackageDirectory);
    foreach (var file in packageDirectory.EnumerateFiles("*.uplugin", SearchOption.AllDirectories)) {
      await using var reader = file.OpenRead();
      var pluginDescriptor = await jsonService.DeserializeAsync<PluginDescriptor>(reader);
      ArgumentNullException.ThrowIfNull(pluginDescriptor);
      ArgumentNullException.ThrowIfNull(file.Directory);
      yield return new InstalledPlugin(Path.GetFileNameWithoutExtension(file.Name), pluginDescriptor.VersionName,
                                       pluginStructureService.GetInstalledBinaries(file.Directory));
    }
  }

  /// <inheritdoc />
  public void InstallPlugin(string pluginName, IDirectoryInfo sourceDirectory, string? engineVersion) {
    var installedEngine = GetInstalledEngine(engineVersion);
    var installDirectory = Path.Join(installedEngine.PackageDirectory, pluginName);
    if (fileSystem.Directory.Exists(installDirectory)) {
      fileSystem.Directory.Delete(installDirectory, true);
    }

    var destinationDirectory = fileSystem.Directory.CreateDirectory(installDirectory);


    foreach (var file in sourceDirectory.GetFiles("*", SearchOption.AllDirectories)) {
      var relativePath = Path.GetRelativePath(sourceDirectory.FullName, file.FullName);
      var targetPath = Path.Join(destinationDirectory.FullName, relativePath);

      // Ensure target directory exists
      var targetDir = Path.GetDirectoryName(targetPath);
      if (!string.IsNullOrEmpty(targetDir)) {
        fileSystem.Directory.CreateDirectory(targetDir);
      }

      file.CopyTo(targetPath, true);
    }
  }
}