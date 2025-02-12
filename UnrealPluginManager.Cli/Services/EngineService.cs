using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Services;

public class EngineService(IPluginService pluginService, IEnginePlatformService enginePlatformService) : IEngineService {
    
    public List<InstalledEngine> GetInstalledEngines() {
        return enginePlatformService.GetInstalledEngines();
    }

    public async Task<int> BuildPlugin(FileInfo pluginFile, string? engineVersion) {
        var installedEngines = GetInstalledEngines();
        var installedEngine = engineVersion is not null
            ? installedEngines.Find(x => x.Key == engineVersion)
            : installedEngines.Where(x => !x.CustomBuild)
                .OrderByDescending(x => x.Version)
                .First();
        var scriptPath = Path.Join(GetBatchFilesDirectory(installedEngine!),
            $"RunUAT.{enginePlatformService.ScriptFileExtension}");
        using var collection = new TempFileCollection();

        var process = new Process();
        process.StartInfo.FileName = scriptPath;
        process.StartInfo.Arguments =
            $"BuildPlugin -Plugin=\"{pluginFile.FullName}\" -package=\"{collection.BasePath}\"";

        process.Start();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0) {
            return process.ExitCode;
        }

        var upluginInfo = new FileInfo(pluginFile.FullName);
        JsonNode pluginDescriptor;
        await using (var reader = upluginInfo.OpenRead()) {
            pluginDescriptor = (await JsonNode.ParseAsync(reader))!;
        }
        pluginDescriptor["bInstalled"] = true;
        await using (var jsonWriter = new Utf8JsonWriter(upluginInfo.OpenWrite())) {
            pluginDescriptor.WriteTo(jsonWriter);
        }
        upluginInfo.CopyTo(Path.Join(collection.BasePath, upluginInfo.Name), true);

        var zipFile = Path.Join(collection.BasePath, $"{pluginFile.Name}.zip");
        ZipFile.CreateFromDirectory(collection.BasePath, zipFile);
        
        await using var fileStream = new FileStream(zipFile, FileMode.Open);
        await pluginService.SubmitPlugin(fileStream, installedEngine!.Version);
        return 0;
    }

    private string GetBatchFilesDirectory(InstalledEngine installedEngine) {
        return Path.Join(installedEngine.Directory.FullName, "Engine", "Build", "BatchFiles");
    }
}