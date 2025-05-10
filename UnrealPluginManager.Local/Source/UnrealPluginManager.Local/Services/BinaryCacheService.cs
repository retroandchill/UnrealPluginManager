using System.IO.Abstractions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database;
using UnrealPluginManager.Local.Mappers;
using UnrealPluginManager.Local.Model.Cache;
using UnrealPluginManager.Server.Database.Building;

namespace UnrealPluginManager.Local.Services;

[AutoConstructor]
public partial class BinaryCacheService : IBinaryCacheService {

  private readonly LocalUnrealPluginManagerContext _dbContext;
  private readonly IFileSystem _fileSystem;
  private readonly IPluginService _pluginService;
  private readonly IEngineService _engineService;

  public async Task<PluginBuildInfo> CacheBuiltPlugin(PluginManifest manifest, IDirectoryInfo directory,
                                                      string engineVersion,
                                                      IReadOnlyCollection<string> platforms) {
    var iconFile = directory.File(Path.Join("Resources", "Icon128.png"));
    await using (var iconFileStream = iconFile.Exists ? iconFile.OpenRead() : null) {
      var readmeFile = directory.File("README.md");
      string? readmeContent = null;
      if (readmeFile.Exists) {
        using var readmeStream = readmeFile.OpenText();
        readmeContent = await readmeStream.ReadToEndAsync();
      }

      await _pluginService.SubmitPlugin(manifest, iconFileStream, readmeContent);
    }

    var pluginVersion = await _dbContext.PluginVersions
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
      var installedPluginVersion = await _engineService.GetInstalledPluginVersion(dependency.PluginName, engineVersion);
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
    _dbContext.CachedBuilds.Add(build);
    await _dbContext.SaveChangesAsync();

    return build.ToPluginBuildInfo();
  }

  public async Task<Option<PluginBuildInfo>> GetCachedPluginBuild(string pluginName, SemVersion pluginVersion,
                                                                  string engineVersion,
                                                                  IReadOnlyCollection<string> targetPlatforms) {
    return await _dbContext.CachedBuilds
        .Where(x => x.PluginVersion.Parent.Name == pluginName
                    && x.PluginVersion.VersionString == pluginVersion.ToString()
                    && x.EngineVersion == engineVersion
                    && targetPlatforms.All(y => x.Platforms.Any(z => z.Platform == y)))
        .OrderByDescending(x => x.BuiltOn)
        .ToPluginBuildInfoQuery()
        .FirstAsync();
  }

  public async Task<Option<IDirectoryInfo>> GetCachedBuildDirectory(string pluginName, SemVersion pluginVersion,
                                                                    string engineVersion,
                                                                    IReadOnlyCollection<string> targetPlatforms) {
    var cachedInstall = await GetCachedPluginBuild(pluginName, pluginVersion, engineVersion, targetPlatforms);
    return cachedInstall.Select(x => _fileSystem.DirectoryInfo.New(x.DirectoryName));
  }
}