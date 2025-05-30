﻿using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Local.Model.Engine;
using UnrealPluginManager.Local.Model.Plugins;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides functionalities related to engine management within the Unreal Plugin Manager CLI.
/// </summary>
public interface IEngineService {

  /// <summary>
  /// Retrieves the installed Unreal Engine based on the specified version.
  /// </summary>
  /// <param name="engineVersion">
  /// The version of the Unreal Engine to retrieve. If null, retrieves the default or latest installed version.
  /// </param>
  /// <returns>
  /// An <see cref="InstalledEngine"/> object that represents the installed Unreal Engine corresponding to the specified version.
  /// </returns>
  InstalledEngine GetInstalledEngine(string? engineVersion);

  /// <summary>
  /// Retrieves a list of installed Unreal Engine versions available on the system.
  /// </summary>
  /// <returns>
  /// A list of <see cref="InstalledEngine"/> objects representing the installed Unreal Engine versions.
  /// </returns>
  List<InstalledEngine> GetInstalledEngines();

  /// <summary>
  /// Builds a specified Unreal Engine plugin using the provided engine version.
  /// </summary>
  /// <param name="pluginFile">
  ///   The <see cref="IFileInfo"/> representing the path of the plugin file to be built.
  /// </param>
  /// <param name="destination"></param>
  /// <param name="engineVersion">
  ///   The version of the Unreal Engine to use for building the plugin. If null, the default engine version is used.
  /// </param>
  /// <param name="platforms"></param>
  /// <returns>
  /// A task representing the asynchronous operation with an integer exit code indicating the result of the build process.
  /// </returns>
  public Task<int> BuildPlugin(IFileInfo pluginFile,
                               IDirectoryInfo destination,
                               string? engineVersion,
                               IReadOnlyCollection<string> platforms);

  /// <summary>
  /// Retrieves the installed version of a specified plugin for a given Unreal Engine version, if available.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin for which the version is being retrieved.
  /// </param>
  /// <param name="engineVersion">
  /// The version of the Unreal Engine to check for the installed plugin. This parameter can be null.
  /// </param>
  /// <returns>
  /// An <see cref="Option{T}"/> containing the installed <see cref="SemVersion"/> of the plugin if found; otherwise, an empty option.
  /// </returns>
  public Task<Option<SemVersion>> GetInstalledPluginVersion(string pluginName, string? engineVersion);

  /// <summary>
  /// Retrieves a list of installed plugins for a specified Unreal Engine version.
  /// </summary>
  /// <param name="engineVersion">
  /// The version of the Unreal Engine for which to retrieve the installed plugins.
  /// If null, the default or latest engine version will be used.
  /// </param>
  /// <returns>
  /// An asynchronous enumerable of <see cref="InstalledPlugin"/> objects representing
  /// the installed plugins for the specified Unreal Engine version.
  /// </returns>
  public IAsyncEnumerable<InstalledPlugin> GetInstalledPlugins(string? engineVersion);


  /// <summary>
  /// Installs a plugin into the specified Unreal Engine version from the provided source directory.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin to install.
  /// </param>
  /// <param name="sourceDirectory">
  /// The source directory containing the plugin files.
  /// </param>
  /// <param name="engineVersion">
  /// The version of the Unreal Engine where the plugin will be installed. If null, the plugin is installed into the default or latest installed engine version.
  /// </param>
  public void InstallPlugin(string pluginName, IDirectoryInfo sourceDirectory, string? engineVersion);
}