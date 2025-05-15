using System.IO.Abstractions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Retro.ReadOnlyParams.Annotations;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database;
using UnrealPluginManager.Local.Database.Building;
using UnrealPluginManager.Local.Mappers;
using UnrealPluginManager.Local.Model.Cache;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides functionality for caching plugin builds in binary form.
/// </summary>
public class BinaryCacheService(
    [ReadOnly] LocalUnrealPluginManagerContext dbContext,
    [ReadOnly] IPluginService pluginService,
    [ReadOnly] IEngineService engineService) : IBinaryCacheService {
  /// <inheritdoc />
  public async Task<PluginBuildInfo> CacheBuiltPlugin(PluginManifest manifest, IDirectoryInfo directory,
                                                      IReadOnlyList<string> patches,
                                                      string engineVersion,
                                                      IReadOnlyCollection<string> platforms) {
    var versionInfo = await pluginService.GetPluginVersionInfo(manifest.Name, manifest.Version);
    if (versionInfo.IsNone) {
      var iconFile = directory.File(Path.Join("Resources", "Icon128.png"));
      await using (var iconFileStream = iconFile.Exists ? iconFile.OpenRead() : null) {
        var readmeFile = directory.File("README.md");
        string? readmeContent = null;
        if (readmeFile.Exists) {
          using var readmeStream = readmeFile.OpenText();
          readmeContent = await readmeStream.ReadToEndAsync();
        }

        await pluginService.SubmitPlugin(manifest, patches, iconFileStream, readmeContent);
      }
    }

    var pluginVersion = await dbContext.PluginVersions
        .Include(x => x.Parent)
        .Include(x => x.Dependencies)
        .SingleAsync(x => x.Parent.Name == manifest.Name && x.VersionString == manifest.Version.ToString());

    var build = new PluginBuild {
        PluginVersion = pluginVersion,
        PluginVersionId = pluginVersion.Id,
        EngineVersion = engineVersion,
        DirectoryName = directory.FullName,
        Platforms = platforms
            .Select(x => new PluginBuildPlatform {
                Platform = x
            })
            .ToList()
    };
    foreach (var dependency in pluginVersion.Dependencies) {
      var installedPluginVersion = await engineService.GetInstalledPluginVersion(dependency.PluginName, engineVersion);
      installedPluginVersion.Match(x => {
                                     build.BuiltWith.Add(new DependencyBuildVersion {
                                         Dependency = dependency,
                                         DependencyId = dependency.Id,
                                         Version = x
                                     });
                                   },
                                   () => throw new ContentNotFoundException(
                                       $"Missing a {dependency.PluginName} plugin in the engine's package directory."));
    }

    dbContext.CachedBuilds.Add(build);
    await dbContext.SaveChangesAsync();

    return build.ToPluginBuildInfo();
  }

  /// <inheritdoc />
  public async Task<Option<PluginBuildInfo>> GetCachedPluginBuild(string pluginName, SemVersion pluginVersion,
                                                                  string engineVersion,
                                                                  IReadOnlyCollection<string> targetPlatforms) {
    return await dbContext.CachedBuilds
        .Include(x => x.PluginVersion)
        .ThenInclude(x => x.Dependencies)
        .Include(x => x.PluginVersion)
        .ThenInclude(x => x.Parent)
        .Where(x => x.PluginVersion.Parent.Name == pluginName
                    && x.PluginVersion.VersionString == pluginVersion.ToString()
                    && x.EngineVersion == engineVersion
                    && targetPlatforms.All(y => x.Platforms.Any(z => z.Platform == y)))
        .AsAsyncEnumerable()
        .OrderByDescending(x => x.BuiltOn)
        .Select(x => x.ToPluginBuildInfo())
        .FirstAsync();
  }
}