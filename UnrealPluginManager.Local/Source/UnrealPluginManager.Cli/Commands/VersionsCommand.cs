using System.CommandLine;
using System.CommandLine.IO;
using JetBrains.Annotations;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Cli.Commands;

/// <summary>
/// Represents a command that lists all installed versions of the Unreal Engine.
/// </summary>
/// <remarks>
/// The VersionsCommand is designed to provide an overview of installed Unreal Engine versions within the tool.
/// It utilizes the CLI framework to enable users to retrieve this information directly from the command line.
/// The command is integrated with specific options and a handler to execute the required logic for listing the engine versions.
/// </remarks>
public class VersionsCommand()
    : Command<VersionsCommandOptions, VersionsCommandOptionsHandler>("versions",
                                                                     "Lists all installed engine versions.");

/// <summary>
/// Represents the options required for the VersionsCommand in the CLI interface.
/// </summary>
/// <remarks>
/// This class holds the configuration and input parameters that can be passed to the VersionsCommand.
/// It enables the command to process user-provided data or defaults when listing all installed versions of the Unreal Engine.
/// VersionsCommandOptions acts as a data contract for the command, ensuring proper parameter handling during execution.
/// </remarks>
[UsedImplicitly]
public class VersionsCommandOptions : ICommandOptions;

/// <summary>
/// Handles execution logic for the "versions" command, which lists all installed engine versions.
/// </summary>
/// <remarks>
/// The VersionsCommandOptionsHandler is responsible for processing the options received for the "versions" command
/// and executing the associated functionality to display installed Unreal Engine versions.
/// This includes determining the current selected engine version and marking it in the output.
/// The class interacts with the IEngineService to retrieve engine version details and utilizes
/// the console for displaying the results.
/// </remarks>
[UsedImplicitly]
public class VersionsCommandOptionsHandler(
    [ReadOnly] IConsole console,
    [ReadOnly] IEnvironment environment,
    [ReadOnly] IEngineService engineService) : ICommandOptionsHandler<VersionsCommandOptions> {
  /// <inheritdoc />
  public Task<int> HandleAsync(VersionsCommandOptions options, CancellationToken cancellationToken) {
    var installedEngines = engineService.GetInstalledEngines();
    var currentVersion = environment.GetEnvironmentVariable(EnvironmentVariables.PrimaryUnrealEngineVersion)
        .Match(x => installedEngines.FindIndex(y => y.Name == x),
               () => installedEngines.Index()
                   .Where(y => !y.Item.CustomBuild)
                   .OrderByDescending(y => y.Item.Version)
                   .Select(y => y.Index)
                   .FirstOrDefault(-1));
    foreach (var version in installedEngines.Index()) {
      console.Out.WriteLine($"- {version.Item.DisplayName}{(version.Index == currentVersion ? " *" : "")}");
    }

    return Task.FromResult(0);
  }
}