using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using System.Text.Json;
using JetBrains.Annotations;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Local.Services;
using static UnrealPluginManager.Core.Model.Resolution.ResolutionResult;

namespace UnrealPluginManager.Cli.Commands;

/// <summary>
/// Represents the command for installing a plugin into the Unreal Engine.
/// </summary>
/// <remarks>
/// The <see cref="InstallCommand"/> class is a CLI command that allows the user
/// to install a specified plugin into an Unreal Engine instance. It supports
/// specifying the plugin name, the desired version, and the engine version
/// for compatibility during installation.
/// </remarks>
public class InstallCommand : Command<InstallCommandOptions, InstallCommandHandler> {
  /// <summary>
  /// Represents the command used for installing software plugins into a specified Unreal Engine instance.
  /// </summary>
  /// <remarks>
  /// The <see cref="InstallCommand"/> class is part of a CLI tool built for managing Unreal Engine plugins.
  /// This command utilizes arguments and options to determine which plugin to install, its version,
  /// and the target Unreal Engine version. The installation process relies on dependency injection for
  /// access to underlying services.
  /// </remarks>
  /// <example>
  /// This command is added to the CLI root command and provides functionality for plugin installation.
  /// Users can invoke this command with the appropriate arguments and options, such as plugin name,
  /// version, and Unreal Engine version.
  /// </example>
  public InstallCommand() : base("install", "Install the specified plugin into the engine") {
    var inputArg = new Argument<string>("input", "The name of plugin to install");
    AddArgument(inputArg);
    var versionOption = new System.CommandLine.Option<SemVersionRange>(aliases: ["--version", "-v"],
                                                                       description:
                                                                       "The version of the plugin to install",
                                                                       parseArgument: r =>
                                                                           r.Tokens.Count == 1
                                                                               ? SemVersionRange
                                                                                   .Parse(r.Tokens[0]
                                                                                         .Value)
                                                                               : SemVersionRange
                                                                                   .AllRelease) {
        IsRequired = false,
    };
    AddOption(versionOption);
    AddOption(new System.CommandLine.Option<string>(["--engine-version", "-e"],
                                                    "The version of the engine to build the plugin for") {
        IsRequired = false,
    });

    AddValidator(result => {
      var resultArg = result.GetValueForArgument(inputArg);
      const StringComparison ignore = StringComparison.InvariantCultureIgnoreCase;
      if ((resultArg.EndsWith(".uplugin", ignore) || resultArg.EndsWith(".uproject")) &&
          result.GetValueForOption(versionOption) is not null) {
        result.ErrorMessage = "You cannot specify a version when installing from a project or plugin file.";
      }
    });
  }
}

/// <summary>
/// Represents the options required for the "install" command in the CLI tool.
/// </summary>
/// <remarks>
/// The <see cref="InstallCommandOptions"/> class defines the parameters needed
/// to execute the installation of a plugin. These parameters include the plugin name,
/// the version range of the plugin, and the target Unreal Engine version. These options
/// are used to customize and validate the installation process.
/// </remarks>
[UsedImplicitly]
public class InstallCommandOptions : ICommandOptions {
  /// <summary>
  /// Gets or sets the input value representing the name of the plugin to be installed.
  /// </summary>
  /// <remarks>
  /// This property specifies the mandatory identifier for the plugin that the user
  /// wishes to install using the command-line interface. It is a required parameter
  /// utilized during the plugin installation process.
  /// </remarks>
  [UsedImplicitly]
  public required string Input { get; set; }

  /// <summary>
  /// Gets or sets the version range of the plugin to be installed.
  /// </summary>
  /// <remarks>
  /// This property specifies the acceptable version range of the plugin that the user
  /// intends to install. It supports specifying exact versions, ranges, or constraints,
  /// allowing flexibility in determining compatible versions during the installation process.
  /// </remarks>
  [UsedImplicitly]
  public SemVersionRange Version { get; set; } = SemVersionRange.AllRelease;

  /// <summary>
  /// Gets or sets the target Unreal Engine version for the installation process.
  /// </summary>
  /// <remarks>
  /// This property specifies the version of Unreal Engine that the plugin should be installed for.
  /// It allows users to target a specific engine version when executing the installation command.
  /// If not specified, it may default to a suitable version as determined by the system or the installer.
  /// </remarks>
  [UsedImplicitly]
  public string? EngineVersion { get; set; }
}

/// <summary>
/// Handles the logic for executing the install command in the CLI.
/// </summary>
/// <remarks>
/// The <see cref="InstallCommandHandler"/> is responsible for coordinating the installation
/// of a specified plugin by utilizing the <see cref="IEngineService"/> to handle the
/// installation process. It takes user-defined options such as the plugin name, version range,
/// and engine version, and ensures the appropriate installation steps are performed.
/// </remarks>
[AutoConstructor]
[UsedImplicitly]
public partial class InstallCommandHandler : ICommandOptionsHandler<InstallCommandOptions> {
  private readonly IConsole _console;
  private readonly IFileSystem _fileSystem;
  private readonly IEngineService _engineService;
  private readonly IPluginManagementService _pluginManagementService;

  /// <inheritdoc />
  public Task<int> HandleAsync(InstallCommandOptions options, CancellationToken cancellationToken) {
    if (options.Input.EndsWith(".uplugin", StringComparison.InvariantCultureIgnoreCase) || options.Input.EndsWith(".uproject", StringComparison.InvariantCultureIgnoreCase)) {
      return InstallRequirements(options.Input, options.EngineVersion);
    }

    return InstallPlugin(options.Input, options.Version, options.EngineVersion);
  }

  private async Task<int> InstallRequirements(string pluginFile, string? engineVersion) {
    IDependencyHolder? descriptor;
    await using (var stream = _fileSystem.File.OpenRead(pluginFile)) {
      descriptor = pluginFile.EndsWith(".uplugin") ? await JsonSerializer.DeserializeAsync<PluginDescriptor>(stream) 
          : await JsonSerializer.DeserializeAsync<ProjectDescriptor>(stream);
      ArgumentNullException.ThrowIfNull(descriptor);
    }

    var chainRoot = descriptor.ToDependencyChainRoot();
    var dependencyTree = await _pluginManagementService.GetPluginsToInstall(chainRoot, engineVersion);
    return await dependencyTree.Match(x => InstallToEngine(x, engineVersion),
                                      x => ReportConflicts("requirements", x));
  }

  private async Task<int> InstallPlugin(string name, SemVersionRange version, string? engineVersion) {
    var existingVersion = await _engineService.GetInstalledPluginVersion(name, engineVersion)
        .Map(x => x.Where(version.Contains));
    return await existingVersion
        .Match(v => {
                 _console.Out.WriteLine($"Installed version {v} already satisfies the version requirement.");
                 return Task.FromResult(0);
               },
               () => TryInstall(name, version, engineVersion));
  }
  

  private async Task<int> TryInstall(string name, SemVersionRange version, string? engineVersion) {
    var targetPlugin = await _pluginManagementService.FindTargetPlugin(name, version, engineVersion);
    var dependencyTree = await _pluginManagementService.GetPluginsToInstall(targetPlugin, engineVersion);
    return await dependencyTree.Match(x => InstallToEngine(x, engineVersion),
        x => ReportConflicts(name, x));
  }

  private async Task<int> InstallToEngine(ResolvedDependencies resolvedDependencies, string? engineVersion) {
    var currentlyInstalled = await _engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => x.Version);

    foreach (var dep in resolvedDependencies.SelectedPlugins) {
      if (!currentlyInstalled.TryGetValue(dep.Name, out var currentVersion) || currentVersion != dep.Version) {
        await _engineService.InstallPlugin(dep.Name, dep.Version, engineVersion, ["Win64"]);
      }
    }

    return 0;
  }

  private Task<int> ReportConflicts(string pluginName, ConflictDetected conflicts) {
    _console.Out.WriteLine($"Unable to install {pluginName} due conflicts. The following conflicts were detected:");
    foreach (var conflict in conflicts.Conflicts) {
      _console.Out.WriteLine($"\n{conflict.PluginName} required by:");
      foreach (var requiredBy in conflict.Versions) {
        _console.Out.WriteLine($"    {requiredBy.RequiredBy} => {requiredBy.RequiredVersion}");
      }
    }

    return Task.FromResult(1);
  }
}