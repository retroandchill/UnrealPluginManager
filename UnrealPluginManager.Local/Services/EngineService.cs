using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using Semver;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Model.Engine;

namespace UnrealPluginManager.Local.Services;

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
        
        await _pluginService.SubmitPlugin(intermediateFolder, installedEngine.Version.ToString());
        return 0;
    }

    /// <inheritdoc />
    public async Task<int> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion,
                                         IReadOnlyCollection<string> targetPlatforms) {
        var installedEngine = GetInstalledEngine(engineVersion);
        var installDirectory = Path.Join(installedEngine.PackageDirectory, pluginName);
        if (_fileSystem.Directory.Exists(installDirectory)) {
            _fileSystem.Directory.Delete(installDirectory, true);
        }
        var destinationDirectory = _fileSystem.Directory.CreateDirectory(installDirectory);

        await foreach (var zipFile in _pluginService.GetAllPluginData(pluginName, pluginVersion, installedEngine.Name,
                                                                      targetPlatforms)) {
            await using var fileStream = zipFile.OpenRead();
            using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            await _fileSystem.ExtractZipFile(zipArchive, destinationDirectory.FullName);
        }
        
        return 0;
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