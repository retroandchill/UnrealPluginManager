using System.CommandLine;
using UnrealPluginManager.Cli.Services;

namespace UnrealPluginManager.Cli.Commands;

public class VersionsCommand()
    : Command<VersionsCommandOptions, VersionsCommandOptionsHandler>("versions",
        "Lists all installed engine versions.");

public class VersionsCommandOptions : ICommandOptions;

public class VersionsCommandOptionsHandler(IConsole console, IEngineService engineService) : ICommandOptionsHandle<VersionsCommandOptions> {
    public Task<int> HandleAsync(VersionsCommandOptions options, CancellationToken cancellationToken) {
        foreach (var version in engineService.GetInstalledEngines()) {
            console.WriteLine(version);
        }
        return Task.FromResult(0);
    }
}