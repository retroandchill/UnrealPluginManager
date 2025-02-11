using System.CommandLine;
using UnrealPluginManager.Cli.Services;

namespace UnrealPluginManager.Cli.Commands;

public class VersionsCommand()
    : Command<VersionsCommandOptions, VersionsCommandOptionsHandler>("versions",
        "Lists all installed engine versions.");

public class VersionsCommandOptions : ICommandOptions;

public class VersionsCommandOptionsHandler(IConsole console, IEngineService engineService) : ICommandOptionsHandle<VersionsCommandOptions> {
    public Task<int> HandleAsync(VersionsCommandOptions options, CancellationToken cancellationToken) {
        var installedEngines = engineService.GetInstalledEngines();
        LanguageExt.Option<string> selected = Environment.GetEnvironmentVariable(EnvironmentVariables.PrimaryUnrealEngineVersion);
        var currentVersion = selected
            .Match(x => installedEngines.FindIndex(y => y.Name == x),
                () => installedEngines.Index()
                    .Where(y => !y.Item.CustomBuild)
                    .OrderByDescending(y => y.Item.Version)
                    .Select(y => y.Index)
                    .FirstOrDefault(-1));
        foreach (var version in installedEngines.Index()) {
            console.WriteLine($"- {version.Item.Name}{(version.Index == currentVersion ? " *" : "")}");
        }
        return Task.FromResult(0);
    }
}