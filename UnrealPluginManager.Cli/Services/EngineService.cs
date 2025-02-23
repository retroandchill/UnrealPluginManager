using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using Semver;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Provides services related to managing Unreal Engine installations.
/// </summary>
[AutoConstructor]
public partial class EngineService : IEngineService {
    private readonly IFileSystem _fileSystem;
    private readonly IPluginService _pluginService;
    private readonly IEnginePlatformService _enginePlatformService;
    private readonly IProcessRunner _processRunner;

    /// <inheritdoc />
    public async Task<IDependencyHolder> ReadSubmittedPluginFile(string filename) {
        await using var fileStream = _fileSystem.File.OpenRead(filename);
        const StringComparison ignore = StringComparison.InvariantCultureIgnoreCase;
        if (filename.EndsWith(".uplugin", ignore)) {
            return JsonSerializer.Deserialize<PluginDescriptor>(fileStream)!;
        }

        if (filename.EndsWith(".uproject", ignore)) {
            return JsonSerializer.Deserialize<ProjectDescriptor>(fileStream)!;
        }
        
        throw new ArgumentException("Invalid file type.");
    }

    /// <inheritdoc />
    public List<InstalledEngine> GetInstalledEngines() {
        return _enginePlatformService.GetInstalledEngines();
    }

    /// <inheritdoc />
    public async Task<int> BuildPlugin(IFileInfo pluginFile, string? engineVersion) {
        var installedEngine = GetInstalledEngine(engineVersion);
        var scriptPath = Path.Join(installedEngine.BatchFilesDirectory,
            $"RunUAT.{_enginePlatformService.ScriptFileExtension}");
        using var intermediate = _fileSystem.CreateDisposableDirectory(out var intermediateFolder);

        var exitCode = await _processRunner.RunProcess(scriptPath, [
            "BuildPlugin",
            $"-Plugin=\"{pluginFile.FullName}\"",
            $"-package=\"{intermediateFolder.FullName}\""
        ]);
        if (exitCode != 0) {
            return exitCode;
        }

        var upluginInfo = _fileSystem.FileInfo.New(pluginFile.FullName);
        JsonNode pluginDescriptor;
        await using (var reader = upluginInfo.OpenRead()) {
            pluginDescriptor = (await JsonNode.ParseAsync(reader))!;
        }
        pluginDescriptor["bInstalled"] = true;
        
        var destPath = Path.Join(intermediateFolder.FullName, upluginInfo.Name);
        await using (var destination = _fileSystem.File.Open(destPath, FileMode.OpenOrCreate)) {
            destination.SetLength(0);
            await using var jsonWriter = new Utf8JsonWriter(destination, new JsonWriterOptions { Indented = true });
            pluginDescriptor.WriteTo(jsonWriter);
        }
        
        using var dest = _fileSystem.CreateDisposableDirectory(out var destFolder);
        var zipFile = Path.Join(destFolder.FullName, $"{Path.GetFileNameWithoutExtension(pluginFile.Name)}.zip");
        await _fileSystem.CreateZipFile(zipFile, intermediateFolder.FullName); 
        
        await using var fileStream = _fileSystem.FileStream.New(zipFile, FileMode.Open);
        await _pluginService.SubmitPlugin(fileStream, installedEngine.Version);
        return 0;
    }

    /// <inheritdoc />
    public async Task<int> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion) {
        var installedEngine = GetInstalledEngine(engineVersion);
        var installDirectory = Path.Join(installedEngine.PackageDirectory, pluginName);
        if (_fileSystem.Directory.Exists(installDirectory)) {
            _fileSystem.Directory.Delete(installDirectory, true);
        }
        var destinationDirectory = _fileSystem.Directory.CreateDirectory(installDirectory);

        await using var result = await _pluginService.GetPluginFileData(pluginName, pluginVersion, installedEngine.Name);
        using var zipArchive = new ZipArchive(result, ZipArchiveMode.Read);
        await _fileSystem.ExtractZipFile(zipArchive, destinationDirectory.FullName);
        
        return 0;
    }

    /// <inheritdoc />
    public async Task<List<PluginVersionInfo>> GetInstalledPluginVersions(IEnumerable<string> pluginNames,
        string? engineVersion) {
        var installedEngine = GetInstalledEngine(engineVersion);
        var installedPlugins = await _fileSystem.DirectoryInfo.New(installedEngine.PackageDirectory)
            .GetFiles("*.uplugin", SearchOption.AllDirectories)
            .ToAsyncEnumerable()
            .SelectAwait(async x => {
                await using var fileStream = x.OpenRead();
                return (Name: Path.GetFileNameWithoutExtension(x.Name),
                    Descriptor: (await JsonSerializer.DeserializeAsync<PluginDescriptor>(fileStream))!);
            })
            .Select(x => new PluginVersionRequest(x.Name, x.Descriptor.VersionName))
            .Where(x => pluginNames.Contains(x.Name))
            .ToListAsync();
        
        return (await _pluginService.RequestPluginInfos(installedPlugins))
            .Select(x => {
                x.IsInstalled = true;
                return x;
            })
            .ToList();
    }

    private InstalledEngine GetInstalledEngine(string? engineVersion) {
        var installedEngines = GetInstalledEngines();
        var installedEngine = engineVersion is not null
            ? installedEngines.Find(x => x.Name == engineVersion)
            : installedEngines.Where(x => !x.CustomBuild)
                .OrderByDescending(x => x.Version)
                .First();
        return installedEngine!;
    }
}