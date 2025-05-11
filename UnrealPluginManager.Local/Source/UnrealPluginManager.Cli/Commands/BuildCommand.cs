using System.CommandLine;
using System.IO.Abstractions;
using JetBrains.Annotations;
using UnrealPluginManager.Cli.Helpers;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Cli.Commands;

/// <summary>
/// Represents a command-line interface (CLI) command for building a specified Unreal Engine plugin.
/// </summary>
/// <remarks>
/// The <see cref="BuildCommand"/> class is utilized to build a specified Unreal Engine plugin
/// from a given source directory, optionally specifying the target engine version.
/// </remarks>
public class BuildCommand : Command<BuildCommandOptions, BuildCommandOptionsHandler> {
  /// <summary>
  /// Represents the "build" command in the command-line interface (CLI) of the Unreal Plugin Manager.
  /// </summary>
  /// <remarks>
  /// This command is intended to build a specified Unreal Engine plugin. The user must provide
  /// the source directory of the plugin to be built as a required argument. An optional
  /// engine version can also be specified using the "--version" option for targeting a specific
  /// Unreal Engine version.
  /// </remarks>
  /// <example>
  /// When invoked, the "build" command processes the input directory and optionally uses
  /// the provided engine version to build the plugin using the Unreal Engine tools.
  /// </example>
  public BuildCommand() : base("build", "build the specified plugin") {
    AddArgument(new Argument<string>("input", "The source directory for the plugin"));
    AddOption(new Option<string>("--version", "The version of the engine to build the plugin for") {
        IsRequired = false
    });
  }
}

/// <summary>
/// Represents the command-line options for the build command used to compile Unreal Engine plugins.
/// </summary>
/// <remarks>
/// The <see cref="BuildCommandOptions"/> class provides the configuration options and parameters
/// needed to execute the build process for an Unreal Engine plugin. These options include the source
/// plugin directory and optionally a target engine version.
/// </remarks>
[UsedImplicitly]
public class BuildCommandOptions : ICommandOptions {
  /// <summary>
  /// Gets or sets the path to the input plugin directory for the build process.
  /// </summary>
  /// <remarks>
  /// This property specifies the directory containing the Unreal Engine plugin source files
  /// that need to be compiled as part of the build command.
  /// </remarks>
  [UsedImplicitly]
  public required string Input { get; set; }

  /// <summary>
  /// Gets or sets the target Unreal Engine version for the build process.
  /// </summary>
  /// <remarks>
  /// This property specifies the version of the Unreal Engine to use when compiling the plugin.
  /// If no version is specified, the build process may use a default or globally configured version.
  /// </remarks>
  [UsedImplicitly]
  public string? Version { get; set; }
}

/// <summary>
/// Handles the execution logic for the build command options used in compiling Unreal Engine plugins.
/// </summary>
/// <remarks>
/// The <see cref="BuildCommandOptionsHandler"/> class processes the command-line arguments
/// provided to the <see cref="BuildCommand"/> through <see cref="BuildCommandOptions"/>. It utilizes
/// the specified engine service to perform the build operation based on the given input and optional
/// target engine version.
/// </remarks>
[AutoConstructor]
[UsedImplicitly]
public partial class BuildCommandOptionsHandler : ICommandOptionsHandler<BuildCommandOptions> {
  private readonly IConsole _console;
  private readonly IFileSystem _fileSystem;
  private readonly IPluginManagementService _pluginManagementService;
  private readonly IInstallService _installService;
  private readonly IJsonService _jsonService;

  /// <inheritdoc />
  public async Task<int> HandleAsync(BuildCommandOptions options, CancellationToken cancellationToken) {
    var manifestFile = _fileSystem.FileInfo.New(options.Input);
    await using var manifestStream = manifestFile.OpenRead();
    var manifest = await _jsonService.DeserializeAsync<PluginManifest>(manifestStream);

    var installResult = await _installService.InstallRequirements(manifest, options.Version,
        ["Win64"]);
    _console.WriteVersionChanges(installResult);

    var patchesFolder = manifestFile.Directory?
        .GetDirectories("patches", SearchOption.TopDirectoryOnly)
        .FirstOrDefault();

    var patchFiles = patchesFolder?.EnumerateFiles("*.patch", SearchOption.AllDirectories)
        .ToDictionary(x => x.Name) ?? [];

    var patchContents = await manifest.Patches
        .ToAsyncEnumerable()
        .SelectAwait(async x => {
          await using var patchStream = patchFiles[x].OpenRead();
          using var patchReader = new StreamReader(patchStream);
          return await patchReader.ReadToEndAsync(cancellationToken);
        })
        .ToListAsync(cancellationToken);


    await _pluginManagementService.BuildFromManifest(manifest, patchContents, options.Version, ["Win64"]);
    return 0;
  }
}