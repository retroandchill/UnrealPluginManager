using System.IO.Abstractions;
using Semver;
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
            var sourceDirectories = pluginDirectory.EnumerateDirectories()
                    .Where(x => !x.Name.StartsWith("Binaries") || !x.Name.StartsWith("Intermediate"));
            var sourceFiles = pluginDirectory.EnumerateFiles();
            sourceZipInfo = await _fileSystem.CreateZipFile(sourceZipInfo.FullName, sourceDirectories, sourceFiles);
            pluginSource = await _storageService.StorePluginSource(pluginName, version, 
                                                                   new CopyFileSource(sourceZipInfo));
        }

        var binariesDirectory = pluginDirectory.SubDirectory("Binaries");
        var intermediateDirectory = pluginDirectory.SubDirectory("Intermediate");
        var binaryDirectories = binariesDirectory.EnumerateDirectories()
                .ToDictionary<IDirectoryInfo, string, List<IDirectoryInfo>>(binary => binary.Name, 
                                                                            binary => [binary]);

        foreach (var intermediate in intermediateDirectory.EnumerateDirectories()) {
            var existingDirectories = binaryDirectories.TryGetValue(intermediate.Name, out var directories)
                ? directories
                : [];
            binaryDirectories[intermediate.Name] = existingDirectories.Concat([ intermediate ]).ToList();
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
}