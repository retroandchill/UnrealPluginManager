using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Local.Model.Cache;

namespace UnrealPluginManager.Local.Services;

public interface IBinaryCacheService {

  Task<PluginBuildInfo> CacheBuiltPlugin(PluginManifest manifest, IDirectoryInfo directory,
                                         IReadOnlyList<string> patches,
                                         string engineVersion, IReadOnlyCollection<string> platforms);

  Task<Option<PluginBuildInfo>> GetCachedPluginBuild(string pluginName, SemVersion pluginVersion, string engineVersion,
                                                     IReadOnlyCollection<string> targetPlatforms);

  Task<Option<IDirectoryInfo>> GetCachedBuildDirectory(string pluginName, SemVersion pluginVersion,
                                                       string engineVersion,
                                                       IReadOnlyCollection<string> targetPlatforms);

}