
using System.CodeDom.Compiler;
using System.IO.Compression;
using System.Text.Json;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Solver;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides operations for managing plugins within the Unreal Plugin Manager application.
/// </summary>
public class PluginService(UnrealPluginManagerContext dbContext, IStorageService storageService) : IPluginService {
    
    private static readonly JsonSerializerOptions JsonOptions = new() {
        AllowTrailingCommas = true
    };
    
    /// <inheritdoc/>
    public async Task<List<PluginSummary>> GetPluginSummaries() {
        return await dbContext.Plugins
            .GroupBy(x => x.Name)
            .Select(g => g.OrderByDescending(x => x.VersionString).FirstOrDefault())
            .AsAsyncEnumerable()
            .Where(p => p != null)
            .Select(p => new PluginSummary(p!.Name, p.Version, p.Description))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<PluginSummary>> GetDependencyList(string pluginName) {
        var plugin = await dbContext.Plugins
            .Include(p => p.Dependencies)
            .Where(p => p.Name == pluginName)
            .OrderByDescending(p => p.VersionString)
            .FirstAsync();

        var pluginData = new Dictionary<string, List<Plugin>> { { plugin.Name, [plugin] } };
        var unresolved = new System.Collections.Generic.HashSet<string>();

        unresolved.UnionWith(plugin.Dependencies
            .Where(x => x.Type == PluginType.Provided)
            .Select(pd => pd.PluginName));
        while (unresolved.Count > 0) {
            var currentlyExisting = unresolved.ToHashSet();
            var plugins = (await dbContext.Plugins
                .Include(p => p.Dependencies)
                .Where(p => unresolved.Contains(p.Name))
                .OrderByDescending(p => p.VersionString)
                .ToListAsync())
                .GroupBy(x => x.Name);

            foreach (var pluginList in plugins) {
                unresolved.Remove(pluginList.Key);
                var asList = pluginList
                    .OrderByDescending(p => p.Version, SemVersion.PrecedenceComparer)
                    .ToList();
                pluginData.Add(pluginList.Key, asList);

                unresolved.UnionWith(pluginList
                    .SelectMany(p => p.Dependencies)
                    .Where(x => x.Type == PluginType.Provided && !pluginData.ContainsKey(x.PluginName))
                    .Select(pd => pd.PluginName));
            }

            var intersection = currentlyExisting.Intersect(unresolved).ToList();
            if (intersection.Count > 0) {
                throw new DependencyResolutionException($"Unable to resolve plugin names:\n{string.Join("\n", intersection)}");
            }
        }

        var formula = ExpressionSolver.Convert(pluginName, plugin.Version, pluginData);
        var bindings = formula.Solve();
        if (bindings.IsNone) {
            return [];
        }
        
        return bindings.SelectMany(b => b)
            .Select(p => pluginData[p.Item1].First(d => d.Version == p.Item2))
            .Select(p => new PluginSummary(p.Name, p.Version, p.Description))
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<PluginSummary> AddPlugin(string pluginName, PluginDescriptor descriptor,
        EngineFileData? storedFile = null) {
        var plugin = MapBasePluginInfo(pluginName, descriptor, storedFile);
        dbContext.Plugins.Add(plugin);
        await dbContext.SaveChangesAsync();
        return new PluginSummary(plugin.Name, plugin.Version, plugin.Description);
    }

    private static Plugin MapBasePluginInfo(string pluginName, PluginDescriptor descriptor, EngineFileData? storedFile) {
        var plugin = new Plugin {
            Name = pluginName,
            FriendlyName = descriptor.FriendlyName,
            Version = descriptor.VersionName,
            Description = descriptor.Description,
            AuthorName = !string.IsNullOrWhiteSpace(descriptor.CreatedBy) ? descriptor.CreatedBy : null,
            AuthorWebsite = descriptor.CreatedByUrl,
            Dependencies = descriptor.Plugins
                .Select(x => new Dependency {
                    PluginName = x.Name,
                    Optional = x.Optional,
                    Type = x.PluginType,
                    PluginVersion = x.VersionMatcher
                })
                .ToList(),
            UploadedPlugins = (storedFile ?? Option<EngineFileData>.None)
                .Select(x => new PluginFileInfo {
                    EngineVersion = x.EngineVersion,
                    FilePath = x.FileInfo
                })
                .AsEnumerable()
                .ToList(),
        };

        return plugin;
    }
    
    public async Task<PluginSummary> SubmitPlugin(Stream fileData, Version engineVersion) {
        var fileInfo = await storageService.StorePlugin(fileData);
        await using var storedData = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var archive = new ZipArchive(storedData);
        
        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var baseName = Path.GetFileNameWithoutExtension(archiveEntry.FullName);
        await using var upluginFile = archiveEntry.Open();

        try {
            var descriptor = await JsonSerializer.DeserializeAsync<PluginDescriptor>(upluginFile, JsonOptions);
            if (descriptor is null) {
                throw new BadSubmissionException("Uplugin file was malformed");
            }
            
            return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, fileInfo));
        } catch (JsonException e) {
            throw new BadSubmissionException("Uplugin file was malformed", e);
        }
    }

    public async Task<Stream> GetPluginFileData(string pluginName, Version engineVersion) {
        var pluginInfo = await dbContext.UploadedPlugins
            .Include(x => x.Parent)
            .Where(p => p.Parent.Name == pluginName)
            .OrderByDescending(p => p.Parent.VersionString)
            .Where(x => x.EngineVersion == engineVersion)
            .FirstOrDefaultAsync();
        if (pluginInfo == null) {
            throw new PluginNotFoundException($"Plugin '{pluginName}' not found.");
        }

        return storageService.RetrievePlugin(pluginInfo.FilePath);
    }
}