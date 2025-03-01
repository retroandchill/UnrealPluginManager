using System.IO.Abstractions;
using System.IO.Compression;
using Semver;
using UnrealPluginManager.Core.Files;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Utils;
using CopyFileSource = UnrealPluginManager.Core.Files.CopyFileSource;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and processing the structure of Unreal Engine plugin directories.
/// </summary>
[AutoConstructor]
public partial class PluginStructureService : IPluginStructureService {
    private readonly IFileSystem _fileSystem;
    private readonly IStorageService _storageService;

    /// <inheritdoc />
    public async Task<PartitionedPlugin> PartitionPlugin(string pluginName, SemVersion version, string engineVersion,
                                                         IDirectoryInfo pluginDirectory) {
        var icon = pluginDirectory.File(Path.Join("Resources", "Icon128.png"));
        
        var pluginIcon = icon is not null ? await _storageService.StorePluginIcon(pluginName, new CopyFileSource(icon)) : null;
        

        IFileInfo pluginSource;
        using (_fileSystem.CreateDisposableFile(out var sourceZipInfo)) {
            var sourceDirectories = pluginDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                    .Where(x => !x.Name.StartsWith("Binaries") && !x.Name.StartsWith("Intermediate"));
            var sourceFiles = pluginDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
            sourceZipInfo = await _fileSystem.CreateZipFile(sourceZipInfo.FullName, sourceDirectories, sourceFiles);
            pluginSource = await _storageService.StorePluginSource(pluginName, version, 
                                                                   new CopyFileSource(sourceZipInfo));
        }

        var binariesDirectory = pluginDirectory.SubDirectory("Binaries");
        var intermediateDirectory = pluginDirectory.SubDirectory("Intermediate", "Build");
        var binaryDirectories = binariesDirectory.EnumerateDirectories()
                .ToDictionary<IDirectoryInfo, string, List<ZipSubDirectory>>(binary => binary.Name, 
                                                                            binary => [new ZipSubDirectory("Binaries", binary)]);

        foreach (var intermediate in intermediateDirectory.EnumerateDirectories()) {
            var existingDirectories = binaryDirectories.TryGetValue(intermediate.Name, out var directories)
                ? directories
                : [];
            binaryDirectories[intermediate.Name] = existingDirectories
                    .Concat([ new ZipSubDirectory(Path.Join("Intermediate", "Build"), intermediate) ]).ToList();
        }

        var pluginBinaries = new Dictionary<string, IFileInfo>();
        foreach (var (platform, directories) in binaryDirectories) {
            using var disposableFile = _fileSystem.CreateDisposableFile(out var binaryZipInfo);
            binaryZipInfo = await _fileSystem.CreateZipFile(binaryZipInfo.FullName, directories, []);
            pluginBinaries[platform] = await _storageService.StorePluginBinaries(pluginName, version, engineVersion, 
                                                                     platform, new CopyFileSource(binaryZipInfo));
        }

        return new PartitionedPlugin(pluginSource, pluginIcon, pluginBinaries);
    }

    /// <inheritdoc />
    public async Task<PartitionedPlugin> PartitionPlugin(string pluginName, SemVersion version, string engineVersion,
                                                         ZipArchive zipArchive) {
        var iconEntry = zipArchive.GetEntry(Path.Join("Resources", "Icon128.png"));
        IFileInfo? pluginIcon = null;
        if (iconEntry is not null) {
            await using var iconStream = iconEntry.Open();
            pluginIcon = await _storageService.StorePluginIcon(pluginName, new StreamFileSource(_fileSystem, iconStream));
        }
        
        IFileInfo pluginSource;
        using (_fileSystem.CreateDisposableFile(out var sourceZipInfo)) {
            var sourceEntries = zipArchive.Entries
                    .Where(x => !x.FullName.StartsWith("Binaries") && !x.FullName.StartsWith("Intermediate"));
            sourceZipInfo = await _fileSystem.CopyEntries(sourceEntries, sourceZipInfo.FullName);
            pluginSource = await _storageService.StorePluginSource(pluginName, version, 
                                                                   new CopyFileSource(sourceZipInfo));
        }

        const string intermediateBuild = "Intermediate/Build";
        var binaryEntries = zipArchive.Entries
                .Where(x => x.FullName.StartsWith("Binaries") || x.FullName.StartsWith(intermediateBuild))
                .Where(x => x.FullName != "Binaries/" && x.FullName != $"{intermediateBuild}/")
                .GroupBy(x => x.FullName.StartsWith("Binaries") ? x.FullName.Split('/')[1] : x.FullName.Split('/')[2])
                .ToDictionary(x => x.Key, x => x.ToList());

        var pluginBinaries = new Dictionary<string, IFileInfo>();
        foreach (var (platform, entries) in binaryEntries) {
            using var disposableFile = _fileSystem.CreateDisposableFile(out var binaryZipInfo);
            binaryZipInfo = await _fileSystem.CopyEntries(entries, binaryZipInfo.FullName);
            pluginBinaries[platform] = await _storageService.StorePluginBinaries(pluginName, version, engineVersion, 
                platform, new CopyFileSource(binaryZipInfo));
        }
        
        return new PartitionedPlugin(pluginSource, pluginIcon, pluginBinaries);
    }
}