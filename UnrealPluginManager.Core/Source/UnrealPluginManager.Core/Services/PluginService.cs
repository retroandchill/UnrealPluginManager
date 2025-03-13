using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Files;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Pagination;
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
  private readonly IJsonService _jsonService;

  /// <param name="matcher"></param>
  /// <param name="pageable"></param>
  /// <inheritdoc/>
  public Task<Page<PluginOverview>> ListPlugins(string matcher = "*", Pageable pageable = default) {
    return _dbContext.Plugins
        .Where(x => EF.Functions.Like(x.Name, matcher.Replace("*", "%")))
        .OrderByDescending(x => x.Name)
        .ToPluginOverview()
        .ToPageAsync(pageable);
  }

  /// <inheritdoc />
  public Task<Page<PluginVersionInfo>> ListLatestVersions(string pluginName, SemVersionRange versionRange,
                                                          Pageable pageable = default) {
    return _dbContext.PluginVersions
        .Include(x => x.Parent)
        .Where(x => EF.Functions.Like(x.Parent.Name, pluginName.Replace("*", "%")))
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
  public Task<Option<PluginVersionDetails>> GetPluginVersionDetails(string pluginName, SemVersion version) {
    return _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .Where(x => x.VersionString == version.ToString())
        .Include(x => x.Parent)
        .Include(x => x.Binaries)
        .Include(x => x.Dependencies)
        .ToPluginVersionDetails()
        .FirstOrDefaultAsync()
        .Map(x => x.ToOption());
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(Guid pluginId, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersion version) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .Where(x => x.VersionString == version.ToString())
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<DependencyManifest> GetPossibleVersions(List<PluginDependency> dependencies) {
    var manifest = new DependencyManifest();
    var unresolved = dependencies
        .Where(x => x.Type == PluginType.Provided)
        .Select(pd => pd.PluginName)
        .ToHashSet();

    while (unresolved.Count > 0) {
      var currentlyExisting = unresolved.ToHashSet();
      var plugins = (await _dbContext.PluginVersions
              .Include(p => p.Parent)
              .Include(p => p.Dependencies)
              .Where(p => unresolved.Contains(p.Parent.Name))
              .OrderByVersionDecending()
              .ToPluginVersionInfo()
              .ToListAsync())
          .GroupBy(x => x.Name);

      foreach (var pluginList in plugins) {
        unresolved.Remove(pluginList.Key);
        var asList = pluginList
            .OrderByDescending(p => p.Version, SemVersion.PrecedenceComparer)
            .ToList();
        manifest.FoundDependencies.Add(pluginList.Key, asList);

        unresolved.UnionWith(pluginList
            .SelectMany(p => p.Dependencies)
            .Where(x => x.Type == PluginType.Provided &&
                        !manifest.FoundDependencies.ContainsKey(x.PluginName))
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
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
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

  /// <inheritdoc/>
  public Task<PluginVersionDetails> AddPlugin(string pluginName, PluginDescriptor descriptor,
                                              EngineFileData? storedFile = null) {
    return AddPlugin(pluginName, descriptor, storedFile.ToOption()
        .Select(x => x.FileInfo.Binaries
            .Select(y => new PluginBinaryType(x.EngineVersion, y.Key)))
        .SelectMany(x => x));
  }

  private async Task<PluginVersionDetails> AddPlugin(string pluginName, PluginDescriptor descriptor,
                                                     IEnumerable<PluginBinaryType> binaries) {
    await using var transaction = await _dbContext.Database.BeginTransactionAsync();
    var plugin = await _dbContext.Plugins
        .Where(x => x.Name == pluginName)
        .FirstOrDefaultAsync();
    if (plugin is null) {
      plugin = descriptor.ToPlugin(pluginName);
      _dbContext.Plugins.Add(plugin);
      await _dbContext.SaveChangesAsync();
    }

    var pluginVersion = descriptor.ToPluginVersion();
    pluginVersion.ParentId = plugin.Id;
    pluginVersion.Binaries.AddRange(binaries.Select(x => x.ToUploadedBinaries()));
    _dbContext.PluginVersions.Add(pluginVersion);
    await _dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
    return pluginVersion.ToPluginVersionDetails();
  }

  /// <inheritdoc/>
  public async Task<PluginVersionDetails> SubmitPlugin(Stream fileData, string engineVersion) {
    var (archive, baseName, descriptor) = await ValidateAndExtractPluginDescriptor(fileData);

    var partitionedPlugin = await _pluginStructureService.PartitionPlugin(baseName, descriptor.VersionName,
        engineVersion, archive);
    return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, partitionedPlugin));
  }

  /// <inheritdoc />
  public async Task<PluginVersionDetails> SubmitPlugin(Stream submission) {
    using var archive = new ZipArchive(submission);
    var sourceArchive = archive.GetEntry("Source.zip");
    if (sourceArchive is null) {
      throw new BadSubmissionException("Source.zip was not found");
    }

    string pluginName;
    PluginDescriptor descriptor;
    await using (var sourceStream = sourceArchive.Open()) {
      var (_, baseName, desc) = await ValidateAndExtractPluginDescriptor(sourceStream);
      pluginName = baseName;
      descriptor = desc;
    }

    await using (var sourceStream = sourceArchive.Open()) {
      await _storageService.StorePluginSource(pluginName, descriptor.VersionName,
          new StreamFileSource(_fileSystem, sourceStream));
    }

    var icon = archive.GetEntry("Icon.png");
    if (icon is not null) {
      await using var iconStream = icon.Open();
      await _storageService.StorePluginIcon(pluginName, new StreamFileSource(_fileSystem, iconStream));
    }

    await using var binaries = archive.Entries
        .Select(x => (Parts: PathSeparatorRegex().Split(x.FullName).Where(y => !string.IsNullOrWhiteSpace(y)).ToArray(),
            Entry: x))
        .Where(x => x.Parts.Length == 3)
        .Where(x => x.Parts[0] == "Binaries")
        .Where(x => x.Parts[2].EndsWith(".zip"))
        .GroupBy(x => x.Parts[1])
        .ToAsyncDisposableDictionary(x => x.Key,
            x => x.ToAsyncDisposableDictionary(y => y.Parts[2].Replace(".zip", ""), y => y.Entry.Open()));

    var binariesTasks = binaries.Select(x => x.Value
            .Select(y => _storageService.StorePluginBinaries(pluginName, descriptor.VersionName, x.Key, y.Key,
                new StreamFileSource(_fileSystem, y.Value))))
        .SelectMany(x => x)
        .ToList();
    await SafeTasks.WhenAll(binariesTasks);

    return await AddPlugin(pluginName, descriptor, binaries
        .Select(x => x.Value
            .Select(y => new PluginBinaryType(x.Key, y.Key)))
        .SelectMany(x => x));
  }

  private async Task<(ZipArchive archive, string baseName, PluginDescriptor descriptor)>
      ValidateAndExtractPluginDescriptor(Stream source) {
    var archive = new ZipArchive(source);
    var pluginDescriptorFile = archive.Entries
        .FirstOrDefault(x => x.Name.EndsWith(".uplugin"));
    if (pluginDescriptorFile is null) {
      throw new BadSubmissionException("Uplugin file was not found");
    }

    var baseName = Path.GetFileNameWithoutExtension(pluginDescriptorFile.FullName);

    PluginDescriptor descriptor;
    try {
      await using var upluginFile = pluginDescriptorFile.Open();
      descriptor = await _jsonService.DeserializeAsync<PluginDescriptor>(upluginFile);
    } catch (JsonException e) {
      throw new BadSubmissionException("Uplugin file was malformed", e);
    }

    var exists = await _dbContext.PluginVersions.AnyAsync(x => x.Parent.Name == baseName
                                                               && x.VersionString == descriptor.VersionName.ToString());
    if (exists) {
      throw new BadSubmissionException($"Plugin {baseName} version {descriptor.VersionName} already exists");
    }

    return (archive, baseName, descriptor);
  }

  /// <inheritdoc />
  public async Task<PluginVersionDetails> SubmitPlugin(IDirectoryInfo pluginDirectory, string engineVersion) {
    var pluginDescriptorFile = pluginDirectory.GetFiles("*.uplugin", SearchOption.TopDirectoryOnly)
        .FirstOrDefault();
    if (pluginDescriptorFile is null) {
      throw new BadSubmissionException("Uplugin file was not found");
    }

    var baseName = Path.GetFileNameWithoutExtension(pluginDescriptorFile.FullName);

    PluginDescriptor descriptor;
    try {
      await using var upluginFile = pluginDescriptorFile.OpenRead();
      descriptor = await _jsonService.DeserializeAsync<PluginDescriptor>(upluginFile);
    } catch (JsonException e) {
      throw new BadSubmissionException("Uplugin file was malformed", e);
    }

    var exists = await _dbContext.PluginVersions.AnyAsync(x => x.Parent.Name == baseName
                                                               && x.VersionString == descriptor.VersionName.ToString());
    if (exists) {
      throw new BadSubmissionException($"Plugin {baseName} version {descriptor.VersionName} already exists");
    }

    var partitionedPlugin = await _pluginStructureService.PartitionPlugin(baseName, descriptor.VersionName,
        engineVersion, pluginDirectory);
    return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, partitionedPlugin));
  }

  /// <inheritdoc />
  public async Task<IFileInfo> GetPluginSource(Guid pluginId, Guid versionId) {
    var plugin = await _dbContext.PluginVersions
        .Include(p => p.Parent)
        .Where(p => p.ParentId == pluginId)
        .Where(p => p.Id == versionId)
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }

    return GetPluginSource(plugin.Parent.Name, plugin.Version);
  }

  private IFileInfo GetPluginSource(string pluginName, SemVersion version) {
    return _storageService.RetrievePluginSource(pluginName, version)
        .OrElseThrow(() => new PluginNotFoundException($"Source for {pluginName} was not found!"));
  }

  /// <inheritdoc />
  public async Task<IFileInfo> GetPluginBinaries(Guid pluginId, Guid versionId, string engineVersion,
                                                 string platform) {
    var plugin = await _dbContext.PluginBinaries
        .Include(b => b.Parent)
        .ThenInclude(v => v.Parent)
        .Where(b => b.Parent.ParentId == pluginId)
        .Where(b => b.ParentId == versionId)
        .Where(b => b.EngineVersion == engineVersion)
        .Where(b => b.Platform == platform)
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Binaries for plugin {pluginId} was not found!");
    }

    return GetPluginBinaries(plugin.Parent.Parent.Name, plugin.Parent.Version, engineVersion, platform);
  }

  private IFileInfo GetPluginBinaries(string pluginName, SemVersion version, string engineVersion, string platform) {
    return _storageService.RetrievePluginBinaries(pluginName, version, engineVersion, platform)
        .OrElseThrow(() => new PluginNotFoundException($"Binaries for {pluginName} was not found!"));
  }

  private IEnumerable<(string, IFileInfo)> GetPluginBinaries(string pluginName, SemVersion version,
                                                             string engineVersion,
                                                             IReadOnlyCollection<string> targetPlatforms) {
    return targetPlatforms.Select(platform => (platform, _storageService
        .RetrievePluginBinaries(pluginName, version, engineVersion, platform)
        .OrElseThrow(() => new PluginNotFoundException($"Binaries for {platform} was not found!"))));
  }

  private async Task<Stream> CreateStructuredFileData(IEnumerable<PluginFileInfo> pluginFiles) {
    var fileStream = _fileSystem.FileStream.New(Path.Join(Path.GetTempPath(), Path.GetRandomFileName()),
        FileMode.Create, FileAccess.ReadWrite,
        FileShare.Read, 4096, FileOptions.DeleteOnClose);

    try {
      using var destinationZip = new ZipArchive(fileStream, ZipArchiveMode.Create, true);

      var createdEngineVersions = new System.Collections.Generic.HashSet<string>();
      destinationZip.CreateEntry("Binaries/");
      foreach (var data in pluginFiles) {
        await data.Match(async source => {
              var sourceEntry = destinationZip.CreateEntry("Source.zip");
              await using var sourceStream = source.File.OpenRead();
              await using var destStream = sourceEntry.Open();
              await sourceStream.CopyToAsync(destStream);
            },
            async icon => {
              var iconEntry = destinationZip.CreateEntry("Icon.png");
              await using var iconStream = icon.File.OpenRead();
              await using var destStream = iconEntry.Open();
              await iconStream.CopyToAsync(destStream);
            },
            async binaries => {
              if (!createdEngineVersions.Contains(binaries.EngineVersion)) {
                destinationZip.CreateEntry(Path.Join("Binaries", $"{binaries.EngineVersion}/"));
                createdEngineVersions.Add(binaries.EngineVersion);
              }

              var binaryEntry =
                  destinationZip.CreateEntry(Path.Join("Binaries", $"{binaries.EngineVersion}",
                      $"{binaries.Platform}.zip"));
              await using var binaryStream = binaries.File.OpenRead();
              await using var destStream = binaryEntry.Open();
              await binaryStream.CopyToAsync(destStream);
            });
      }
    } catch {
      // If we encounter an exception we need to close the file stream so that the temp file is properly deleted
      fileStream.Close();
      throw;
    }

    fileStream.Seek(0, SeekOrigin.Begin);
    return fileStream;
  }

  private async Task<Stream> CreateCombinedFileData(IEnumerable<PluginFileInfo> pluginFiles, string engineVersion) {
    var fileStream = _fileSystem.FileStream.New(Path.Join(Path.GetTempPath(), Path.GetRandomFileName()),
        FileMode.Create, FileAccess.ReadWrite,
        FileShare.Read, 4096, FileOptions.DeleteOnClose);

    try {
      using var destinationZip = new ZipArchive(fileStream, ZipArchiveMode.Create, true);
      foreach (var archive in pluginFiles) {
        if (archive is PluginFileInfo.Binaries binaries && binaries.EngineVersion != engineVersion) {
          continue;
        }

        await using var data = archive.File.OpenRead();
        using var zipArchive = new ZipArchive(data);
        await destinationZip.Merge(zipArchive);
      }
    } catch (Exception) {
      // If we encounter an exception we need to close the file stream so that the temp file is properly deleted
      fileStream.Close();
      throw;
    }

    fileStream.Seek(0, SeekOrigin.Begin);
    return fileStream;
  }

  /// <inheritdoc />
  public async Task<PluginDownload> GetPluginFileData(Guid pluginId, Guid versionId) {
    var plugin = await _dbContext.PluginVersions
        .Include(x => x.Parent)
        .Where(x => x.Id == versionId)
        .Where(x => x.ParentId == pluginId)
        .Include(x => x.Binaries)
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }
    
    var pluginName = plugin.Parent.Name;
    var version = plugin.Version;
    var platforms = plugin.Binaries.Select(x => (x.EngineVersion, x.Platform));
    var pluginData = GetPluginFileInfoUnchecked(pluginName, version, platforms);
    var fileStream = await CreateStructuredFileData(pluginData);
    return new PluginDownload(pluginName, fileStream);
  }

  /// <inheritdoc />
  public async Task<PluginDownload> GetPluginFileData(Guid pluginId, SemVersionRange targetVersion,
                                                      string engineVersion,
                                                      IReadOnlyCollection<string> targetPlatforms,
                                                      bool separated = false) {
    var plugin = await _dbContext.PluginVersions
        .Include(x => x.Parent)
        .WhereVersionInRange(targetVersion)
        .Where(x => x.ParentId == pluginId)
        .Include(x => x.Binaries
            .Where(y => y.EngineVersion == engineVersion)
            .Where(y => targetPlatforms.Contains(y.Platform)))
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }

    var missing = plugin.Binaries.Select(x => x.Platform)
        .Except(targetPlatforms)
        .ToList();
    if (missing.Count > 0) {
      throw new PluginNotFoundException($"Missing binaries for {string.Join(", ", missing)}");
    }
    
    var pluginName = plugin.Parent.Name;
    var version = plugin.Version;
    var pluginData = GetPluginFileInfoUnchecked(pluginName, version, engineVersion, targetPlatforms);
    var fileStream = separated ? await CreateStructuredFileData(pluginData) : await CreateCombinedFileData(pluginData, engineVersion);
    return new PluginDownload(pluginName, fileStream);
  }

  /// <inheritdoc/>
  public async Task<PluginDownload> GetPluginFileData(Guid pluginId, Guid versionId, string engineVersion,
                                                      IReadOnlyCollection<string> targetPlatforms,
                                                      bool separated = false) {
    var plugin = await _dbContext.PluginVersions
        .Include(x => x.Parent)
        .Where(x => x.Id == versionId)
        .Where(x => x.ParentId == pluginId)
        .Include(x => x.Binaries
            .Where(y => y.EngineVersion == engineVersion)
            .Where(y => targetPlatforms.Contains(y.Platform)))
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }

    var missing = plugin.Binaries.Select(x => x.Platform)
        .Except(targetPlatforms)
        .ToList();
    if (missing.Count > 0) {
      throw new PluginNotFoundException($"Missing binaries for {string.Join(", ", missing)}");
    }
    
    var pluginName = plugin.Parent.Name;
    var version = plugin.Version;
    var pluginData = GetPluginFileInfoUnchecked(pluginName, version, engineVersion, targetPlatforms);
    var fileStream = separated ? await CreateStructuredFileData(pluginData) : await CreateCombinedFileData(pluginData, engineVersion);
    return new PluginDownload(pluginName, fileStream);
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<PluginFileInfo> GetAllPluginData(string pluginName, SemVersion pluginVersion,
                                                                 string engineVersion,
                                                                 IReadOnlyCollection<string> targetPlatforms) {
    var plugin = await _dbContext.PluginVersions
        .Include(x => x.Parent)
        .Where(x => x.VersionString == pluginVersion.ToString())
        .Where(x => x.Parent.Name == pluginName)
        .Include(x => x.Binaries
            .Where(y => y.EngineVersion == engineVersion)
            .Where(y => targetPlatforms.Contains(y.Platform)))
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginName} was not found!");
    }

    var missing = plugin.Binaries.Select(x => x.Platform)
        .Except(targetPlatforms)
        .ToList();
    if (missing.Count > 0) {
      throw new PluginNotFoundException($"Missing binaries for {string.Join(", ", missing)}");
    }

    foreach (var data in GetPluginFileInfoUnchecked(pluginName, pluginVersion, engineVersion, targetPlatforms)) {
      yield return data;
    }
  }
  
  private IEnumerable<PluginFileInfo> GetPluginFileInfoUnchecked(string pluginName, SemVersion pluginVersion,
                                                                 IEnumerable<(string, string)> binaries) {
    yield return new PluginFileInfo.Source(
        _storageService.RetrievePluginSource(pluginName, pluginVersion).OrElseThrow());
    var icon = _storageService.RetrievePluginSource(pluginName, pluginVersion).ValueUnsafe();
    if (icon is not null) {
      yield return new PluginFileInfo.Icon(icon);
    }

    foreach (var (engineVersion, platform) in binaries) {
      var archive = _storageService.RetrievePluginBinaries(pluginName, pluginVersion, engineVersion, platform)
          .OrElseThrow();
      yield return new PluginFileInfo.Binaries(engineVersion, platform, archive);
    }
  }

  private IEnumerable<PluginFileInfo> GetPluginFileInfoUnchecked(string pluginName, SemVersion pluginVersion,
                                                                 string engineVersion,
                                                                 IReadOnlyCollection<string> targetPlatforms) {
    return GetPluginFileInfoUnchecked(pluginName, pluginVersion,
        targetPlatforms.Select(platform => (engineVersion, platform)));
  }

  [GeneratedRegex(@"[\\/]")]
  private static partial Regex PathSeparatorRegex();
}