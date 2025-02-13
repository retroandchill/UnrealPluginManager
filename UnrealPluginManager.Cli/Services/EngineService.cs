﻿using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using Semver;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Cli.Services;

public class EngineService(IFileSystem fileSystem, IPluginService pluginService, IEnginePlatformService enginePlatformService) : IEngineService {
    
    public List<InstalledEngine> GetInstalledEngines() {
        return enginePlatformService.GetInstalledEngines();
    }

    public async Task<int> BuildPlugin(FileInfo pluginFile, string? engineVersion) {
        var installedEngine = GetInstalledEngine(engineVersion);
        var scriptPath = Path.Join(installedEngine.BatchFilesDirectory,
            $"RunUAT.{enginePlatformService.ScriptFileExtension}");
        using var intermediate = fileSystem.CreateDisposableDirectory(out var intermediateFolder);

        
        var process = new Process();
        process.StartInfo.FileName = scriptPath;
        process.StartInfo.Arguments =
            $"BuildPlugin -Plugin=\"{pluginFile.FullName}\" -package=\"{intermediateFolder.FullName}\"";

        process.Start();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0) {
            return process.ExitCode;
        }

        var upluginInfo = fileSystem.FileInfo.New(pluginFile.FullName);
        JsonNode pluginDescriptor;
        await using (var reader = upluginInfo.OpenRead()) {
            pluginDescriptor = (await JsonNode.ParseAsync(reader))!;
        }
        pluginDescriptor["bInstalled"] = true;
        
        var destPath = Path.Join(intermediateFolder.FullName, upluginInfo.Name);
        await using (var destination = fileSystem.File.Open(destPath, FileMode.OpenOrCreate)) {
            destination.SetLength(0);
            await using var jsonWriter = new Utf8JsonWriter(destination, new JsonWriterOptions { Indented = true });
            pluginDescriptor.WriteTo(jsonWriter);
        }
        
        using var dest = fileSystem.CreateDisposableDirectory(out var destFolder);
        var zipFile = Path.Join(destFolder.FullName, $"{Path.GetFileNameWithoutExtension(pluginFile.Name)}.zip");
        await fileSystem.CreateZipFile(zipFile, intermediateFolder.FullName); 
        
        await using var fileStream = fileSystem.FileStream.New(zipFile, FileMode.Open);
        await pluginService.SubmitPlugin(fileStream, installedEngine!.Version);
        return 0;
    }

    public async Task<int> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion) {
        var installedEngine = GetInstalledEngine(engineVersion);
        var installDirectory = Path.Join(installedEngine.PackageDirectory, pluginName);
        if (fileSystem.Directory.Exists(installDirectory)) {
            fileSystem.Directory.Delete(installDirectory, true);
        }
        var destinationDirectory = fileSystem.Directory.CreateDirectory(installDirectory);

        await using var result = await pluginService.GetPluginFileData(pluginName, pluginVersion, installedEngine.Version);
        using var zipArchive = new ZipArchive(result, ZipArchiveMode.Read);
        await fileSystem.ExtractZipFile(zipArchive, destinationDirectory.FullName);
        
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