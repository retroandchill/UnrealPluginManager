using System.CodeDom.Compiler;
using System.CommandLine;
using UnrealPluginManager.Cli.Services;

namespace UnrealPluginManager.Cli.Commands;

public class BuildCommand : Command<BuildCommandOptions, BuildCommandOptionsHandler> {
    public BuildCommand() : base("build", "build the specified plugin") {
        AddArgument(new Argument<string>("input", "The source directory for the plugin"));
        AddOption(new Option<string>("--version", "The version of the engine to build the plugin for") {
            IsRequired = false
        });
    }
}

public class BuildCommandOptions : ICommandOptions {
    public string Input { get; set; }
    
    public string? Version { get; set; }
}

public class BuildCommandOptionsHandler(IEngineService engineService) : ICommandOptionsHandle<BuildCommandOptions> {
    public Task<int> HandleAsync(BuildCommandOptions options, CancellationToken cancellationToken) {
        return engineService.BuildPlugin(new FileInfo(options.Input), options.Version);
    }
}