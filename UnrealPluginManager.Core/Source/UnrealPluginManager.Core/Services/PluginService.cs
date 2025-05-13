using System.IO.Abstractions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Retro.SimplePage;
using Retro.SimplePage.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Storage;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Files;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Solver;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides operations for managing plugins within the Unreal Plugin Manager application.
/// </summary>
[AutoConstructor]
public partial class PluginService : IPluginService {
  private readonly UnrealPluginManagerContext _dbContext;
  private readonly IStorageService _storageService;
  private readonly IFileSystem _fileSystem;
  private readonly IPluginStructureService _pluginStructureService;

  /// <param name="matcher"></param>
  /// <param name="pageable"></param>
  /// <inheritdoc/>
  public async Task<Page<PluginOverview>> ListPlugins(string matcher = "", Pageable pageable = default) {
    return await _dbContext.Plugins
        .Where(x => x.Name.ToUpper().Contains(matcher.ToUpper()))
        .Include(x => x.Versions)
        .ThenInclude(x => x.Icon)
        .OrderByDescending(x => x.Name)
        .ToPluginOverviewQuery()
        .ToPageAsync(pageable);
  }

  /// <inheritdoc />
  public Task<Page<PluginVersionInfo>> ListLatestVersions(string pluginName, SemVersionRange versionRange,
                                                          Pageable pageable = default) {
    return _dbContext.PluginVersions
        .Include(x => x.Parent)
        .Include(x => x.Icon)
        .Include(x => x.Patches)
        .ThenInclude(x => x.FileResource)
        .Where(x => x.Parent.Name.ToUpper().Contains(pluginName.ToUpper()))
        .WhereVersionInRange(versionRange)
        .GroupBy(x => x.ParentId)
        .Select(x => x.OrderByDescending(y => y.Major)
                    .ThenByDescending(y => y.Minor)
                    .ThenByDescending(y => y.Patch)
                    .ThenByDescending(y => y.PrereleaseNumber == null)
                    .ThenByDescending(y => y.PrereleaseNumber)
                    .First())
        .ToPageAsync(pageable)
        .Map(x => x.Select(p => p.ToPluginVersionInfo()));
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(Guid pluginId, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDescending()
        .ToPluginVersionInfoQuery()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDescending()
        .ToPluginVersionInfoQuery()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersion version) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .Where(x => x.VersionString == version.ToString())
        .OrderByVersionDescending()
        .ToPluginVersionInfoQuery()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<DependencyManifest> GetPossibleVersions(List<PluginDependency> dependencies) {
    var manifest = new DependencyManifest();
    var unresolved = dependencies
        .Select(pd => pd.PluginName)
        .ToHashSet();

    while (unresolved.Count > 0) {
      var currentlyExisting = unresolved.ToHashSet();
      var plugins = await _dbContext.PluginVersions
          .Include(p => p.Parent)
          .Include(p => p.Dependencies)
          .Where(p => unresolved.Contains(p.Parent.Name))
          .OrderByVersionDescending()
          .ToPluginVersionInfoQuery()
          .ToListAsync()
          .Map(x => x.GroupBy(p => p.Name));

      foreach (var pluginList in plugins) {
        unresolved.Remove(pluginList.Key);
        var asList = pluginList
            .OrderByDescending(p => p.Version, SemVersion.PrecedenceComparer)
            .ToList();
        manifest.FoundDependencies.Add(pluginList.Key, asList);

        unresolved.UnionWith(pluginList
                                 .SelectMany(p => p.Dependencies)
                                 .Where(x => !manifest.FoundDependencies.ContainsKey(x.PluginName))
                                 .Select(pd => pd.PluginName));
      }

      var intersection = currentlyExisting.Intersect(unresolved).ToList();
      if (intersection.Count <= 0) {
        continue;
      }

      manifest.UnresolvedDependencies.AddRange(intersection);
      unresolved.RemoveWhere(x => intersection.Contains(x));
    }

    return manifest;
  }

  /// <inheritdoc/>
  public async Task<List<PluginSummary>> GetDependencyList(Guid pluginId, SemVersionRange? targetVersion = null) {
    var plugin = await _dbContext.PluginVersions
        .Where(p => p.ParentId == pluginId)
        .WhereVersionInRange(targetVersion ?? SemVersionRange.AllRelease)
        .OrderByVersionDescending()
        .ToPluginVersionInfoQuery()
        .FirstAsync();

    var dependencyList = await GetPossibleVersions(plugin.Dependencies);
    return GetDependencyList(plugin, dependencyList);
  }

  /// <inheritdoc />
  public List<PluginSummary> GetDependencyList(IDependencyChainNode root, DependencyManifest manifest) {
    if (manifest.UnresolvedDependencies.Count > 0) {
      throw new MissingDependenciesException(manifest.UnresolvedDependencies);
    }

    var formula = ExpressionSolver.Convert(root, manifest.FoundDependencies);
    return formula.Solve()
        .Right(List<PluginSummary> (r) => throw new DependencyConflictException(r))
        .Left(l => GetDependencyList(l, root, manifest));
  }

  private static List<PluginSummary> GetDependencyList(List<SelectedVersion> selectedVersions,
                                                       IDependencyChainNode root, DependencyManifest manifest) {
    IEnumerable<PluginVersionInfo> result;
    if (root is PluginVersionInfo plugin) {
      result = selectedVersions
          .Select(p => p.Name == root.Name
                      ? plugin
                      : manifest.FoundDependencies[p.Name].First(d => d.Version == p.Version));
    } else {
      result = selectedVersions
          .Where(p => p.Name != root.Name)
          .Select(p => manifest.FoundDependencies[p.Name].First(d => d.Version == p.Version));
    }

    return result.Select(p => p.ToPluginSummary())
        .ToList();
  }

  /// <inheritdoc />
  public async Task<string> GetPluginReadme(Guid pluginId, Guid versionId) {
    var readmeName = await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId)
        .Where(x => x.Id == versionId)
        .Select(x => x.Readme != null ? x.Readme.StoredFilename : null)
        .FirstOrDefaultAsync();
    if (readmeName is null) {
      throw new PluginNotFoundException($"Readme for plugin {pluginId} version {versionId} was not found!");
    }

    await using var readmeStream = _storageService.GetResourceStream(readmeName);
    using var reader = new StreamReader(readmeStream);
    return await reader.ReadToEndAsync();
  }

  /// <inheritdoc />
  public async Task<string> AddPluginReadme(Guid pluginId, Guid versionId, string readme) {
    var pluginVersion = await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId)
        .Where(x => x.Id == versionId)
        .Include(x => x.Parent)
        .Include(x => x.Readme)
        .FirstOrDefaultAsync();
    if (pluginVersion is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} version {versionId} was not found!");
    }

    if (pluginVersion.Readme is not null) {
      throw new BadSubmissionException($"Readme for plugin {pluginId} version {versionId} was already exists!");
    }

    await using var stream = readme.ToStream();
    var (storedFilename, _) = await _storageService.AddResource(new StreamFileSource(_fileSystem, stream));
    var resource = new FileResource {
        OriginalFilename = $"{pluginVersion.Parent.Name}_{pluginVersion.Version}.md",
        StoredFilename = storedFilename
    };
    _dbContext.FileResources.Add(resource);
    pluginVersion.ReadmeId = resource.Id;
    pluginVersion.Readme = resource;


    await _dbContext.SaveChangesAsync();
    return readme;
  }

  /// <inheritdoc />
  public async Task<string> UpdatePluginReadme(Guid pluginId, Guid versionId, string readme) {
    var pluginVersion = await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId)
        .Where(x => x.Id == versionId)
        .Include(x => x.Parent)
        .Include(x => x.Readme)
        .FirstOrDefaultAsync();
    if (pluginVersion is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} version {versionId} was not found!");
    }

    if (pluginVersion.Readme is null) {
      throw new BadSubmissionException($"Readme for plugin {pluginId} version {versionId} was already does notexists!");
    }

    await using var stream = readme.ToStream();
    await _storageService.UpdateResource(pluginVersion.Readme.StoredFilename,
                                         new StreamFileSource(_fileSystem, stream));
    return readme;
  }

  /// <inheritdoc />
  public async Task<PluginVersionInfo> SubmitPlugin(Stream archiveStream) {
    await using var result = await _pluginStructureService.ExtractPluginSubmission(archiveStream);
    return await SubmitPlugin(result.Manifest, result.Patches, result.IconStream, result.ReadmeText);
  }

  /// <inheritdoc />
  public async Task<PluginVersionInfo> SubmitPlugin(PluginManifest manifest,
                                                    IReadOnlyList<string> patches,
                                                    Stream? icon = null,
                                                    string? readme = null) {
    var mainPlugin = await _dbContext.Plugins
        .SingleOrDefaultAsync(x => x.Name == manifest.Name);
    if (mainPlugin is null) {
      mainPlugin = new Plugin {
          Name = manifest.Name
      };
      _dbContext.Plugins.Add(mainPlugin);
    }

    var version = manifest.ToPluginVersion();

    if (patches.Count != manifest.Patches.Count) {
      throw new BadSubmissionException("Number of patches does not match the number of patches in the manifest!");
    }

    for (var i = 0; i < manifest.Patches.Count; i++) {
      await using var patchStream = patches[i].ToStream();
      var fileSource = new StreamFileSource(_fileSystem, patchStream);
      var (storedName, _) = await _storageService.AddResource(fileSource);
      version.Patches.Add(new PluginSourcePatch {
          FileResource = new FileResource {
              OriginalFilename = manifest.Patches[i],
              StoredFilename = storedName
          },
          PatchNumber = (uint)i
      });
    }

    if (icon is not null) {
      var iconFile = await _storageService.AddResource(new StreamFileSource(_fileSystem, icon));
      version.Icon = new FileResource {
          OriginalFilename = $"{manifest.Name}_{manifest.Version}.png",
          StoredFilename = iconFile.ResourceName
      };
    }

    if (readme is not null) {
      await using var readmeStream = readme.ToStream();
      var readmeFile = await _storageService.AddResource(new StreamFileSource(_fileSystem, readmeStream));
      version.Readme = new FileResource {
          OriginalFilename = $"{manifest.Name}_{manifest.Version}.md",
          StoredFilename = readmeFile.ResourceName
      };
    }

    mainPlugin.Versions.Add(version);
    _dbContext.PluginVersions.Add(version);
    await _dbContext.SaveChangesAsync();

    return version.ToPluginVersionInfo();
  }

  /// <inheritdoc />
  public async Task<List<SourcePatchInfo>> GetSourcePatches(Guid pluginId, Guid versionId) {
    var pluginPatches = await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId && x.Id == versionId)
        .Select(x => new {
            PatchFiles = x.Patches
                .OrderBy(y => y.PatchNumber)
                .Select(y => new {
                    y.FileResource.StoredFilename,
                    y.FileResource.OriginalFilename
                })
                .ToList()
        })
        .SingleOrDefaultAsync();
    if (pluginPatches is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} version {versionId} was not found!");
    }

    return await pluginPatches.PatchFiles
        .ToAsyncEnumerable()
        .SelectAwait(async x => {
          await using var stream = _storageService.GetResourceStream(x.StoredFilename);
          using var reader = new StreamReader(stream);
          return new SourcePatchInfo(x.OriginalFilename, await reader.ReadToEndAsync());
        })
        .ToListAsync();
  }
}