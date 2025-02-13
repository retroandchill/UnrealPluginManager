using System.CommandLine;
using Semver;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Commands;

public class InstallCommand : Command<InstallCommandOptions, InstallCommandHandler> {
    public InstallCommand() : base("install", "Install the specified plugin into the engine") {
        AddArgument(new Argument<string>("input", "The name of plugin to install"));
        AddOption(new Option<SemVersionRange>(["--version", "-v"], "The version of the plugin to install")  {
            IsRequired = false,
        });
        AddOption(new Option<string>(["--engine-version", "-e"], "The version of the engine to build the plugin for") {
            IsRequired = false,
        });
    }
}

public class InstallCommandOptions : ICommandOptions {
    public string Input { get; set; }
    
    public SemVersionRange Version { get; set; } = SemVersionRange.AllRelease;
    
    public string? EngineVersion { get; set; }
}

public class InstallCommandHandler(IEngineService engineService) : ICommandOptionsHandle<InstallCommandOptions> {
    public Task<int> HandleAsync(InstallCommandOptions options, CancellationToken cancellationToken) {
        return engineService.InstallPlugin(options.Input, options.Version, options.EngineVersion);
    }
}