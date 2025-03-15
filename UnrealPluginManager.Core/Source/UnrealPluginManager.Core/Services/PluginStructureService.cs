using System.IO.Abstractions;
using System.IO.Compression;
using Semver;
using UnrealPluginManager.Core.Files;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Utils;
using CopyFileSource = UnrealPluginManager.Core.Files.CopyFileSource;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and processing the structure of Unreal Engine plugin directories.
/// </summary>
[AutoConstructor]
public partial class PluginStructureService : IPluginStructureService {
  private const string Binaries = "Binaries";
  private const string Intermediate = "Intermediate";
  private const string IntermediateBuild = "Intermediate/Build";

  private readonly IFileSystem _fileSystem;
  private readonly IStorageService _storageService;

  /// <inheritdoc />
  public async Task<PartitionedPlugin> PartitionPlugin(string pluginName, SemVersion version, string engineVersion,
                                                       IDirectoryInfo pluginDirectory) {
    var icon = pluginDirectory.File(Path.Join("Resources", "Icon128.png"));

    ResourceHandle? pluginIcon = icon.Exists ? await _storageService.AddResource(new CopyFileSource(icon)) : null;

    var readme = pluginDirectory.File("README.md");
    ResourceHandle? pluginReadme =
        readme.Exists ? await _storageService.AddResource(new CopyFileSource(readme)) : null;


    ResourceHandle pluginSource;
    using (_fileSystem.CreateDisposableFile(out var sourceZipInfo)) {
      var sourceDirectories = pluginDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
          .Where(x => !x.Name.StartsWith(Binaries) && !x.Name.StartsWith(Intermediate));
      var sourceFiles = pluginDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
      sourceZipInfo = await _fileSystem.CreateZipFile(sourceZipInfo.FullName, sourceDirectories, sourceFiles);
      pluginSource = await _storageService.AddResource(new CopyFileSource(sourceZipInfo));
    }

    var binariesDirectory = pluginDirectory.SubDirectory(Binaries);
    var intermediateDirectory = pluginDirectory.SubDirectory(Intermediate, "Build");
    var binaryDirectories = binariesDirectory.EnumerateDirectories()
        .ToDictionary<IDirectoryInfo, string, List<ZipSubDirectory>>(binary => binary.Name,
            binary => [new ZipSubDirectory(Binaries, binary)]);

    foreach (var intermediate in intermediateDirectory.EnumerateDirectories()) {
      var existingDirectories = binaryDirectories.TryGetValue(intermediate.Name, out var directories)
          ? directories
          : [];
      binaryDirectories[intermediate.Name] = existingDirectories
          .Concat([new ZipSubDirectory(Path.Join(Intermediate, "Build"), intermediate)]).ToList();
    }

    var pluginBinaries = new Dictionary<PluginBinaryType, ResourceHandle>();
    foreach (var (platform, directories) in binaryDirectories) {
      using var disposableFile = _fileSystem.CreateDisposableFile(out var binaryZipInfo);
      binaryZipInfo = await _fileSystem.CreateZipFile(binaryZipInfo.FullName, directories, []);
      var key = new PluginBinaryType(engineVersion, platform);
      pluginBinaries[key] = await _storageService.AddResource(new CopyFileSource(binaryZipInfo));
    }

    return new PartitionedPlugin(pluginSource, pluginIcon, pluginReadme, pluginBinaries);
  }

  /// <inheritdoc />
  public async Task<PartitionedPlugin> PartitionPlugin(string pluginName, SemVersion version, string engineVersion,
                                                       ZipArchive zipArchive) {
    var iconEntry = zipArchive.GetEntry(Path.Join("Resources", "Icon128.png"));
    ResourceHandle? pluginIcon = null;
    if (iconEntry is not null) {
      await using var iconStream = iconEntry.Open();
      pluginIcon = await _storageService.AddResource(new StreamFileSource(_fileSystem, iconStream));
    }

    var readmeEntry = zipArchive.GetEntry(Path.Join("README.md"));
    ResourceHandle? pluginReadme = null;
    if (readmeEntry is not null) {
      await using var readmeStream = readmeEntry.Open();
      pluginReadme = await _storageService.AddResource(new StreamFileSource(_fileSystem, readmeStream));
    }

    ResourceHandle pluginSource;
    using (_fileSystem.CreateDisposableFile(out var sourceZipInfo)) {
      var sourceEntries = zipArchive.Entries
          .Where(x => !x.FullName.StartsWith(Binaries) && !x.FullName.StartsWith(Intermediate));
      sourceZipInfo = await _fileSystem.CopyEntries(sourceEntries, sourceZipInfo.FullName);
      pluginSource = await _storageService.AddResource(new CopyFileSource(sourceZipInfo));
    }

    var binaryEntries = zipArchive.Entries
        .Where(x => x.FullName.StartsWith(Binaries) || x.FullName.StartsWith(IntermediateBuild))
        .Where(x => x.FullName != "Binaries/" && x.FullName != $"{IntermediateBuild}/")
        .GroupBy(x => x.FullName.StartsWith(Binaries) ? x.FullName.Split('/')[1] : x.FullName.Split('/')[2])
        .ToDictionary(x => x.Key, x => x.ToList());

    var pluginBinaries = new Dictionary<PluginBinaryType, ResourceHandle>();
    foreach (var (platform, entries) in binaryEntries) {
      using var disposableFile = _fileSystem.CreateDisposableFile(out var binaryZipInfo);
      binaryZipInfo = await _fileSystem.CopyEntries(entries, binaryZipInfo.FullName);
      var key = new PluginBinaryType(engineVersion, platform);
      pluginBinaries[key] = await _storageService.AddResource(new CopyFileSource(binaryZipInfo));
    }

    return new PartitionedPlugin(pluginSource, pluginIcon, pluginReadme, pluginBinaries);
  }

  /// <inheritdoc />
  public List<string> GetInstalledBinaries(IDirectoryInfo pluginDirectory) {
    return pluginDirectory.EnumerateDirectories(Binaries, SearchOption.TopDirectoryOnly)
        .Concat(pluginDirectory.EnumerateDirectories(IntermediateBuild, SearchOption.TopDirectoryOnly))
        .Select(x => x.Name)
        .Distinct()
        .ToList();
  }
}