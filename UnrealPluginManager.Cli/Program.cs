using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.DependencyInjection;

var rootCommand = new RootCommand {
    new VersionsCommand()
};

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseDependencyInjection(services => {
        
    });
    
await builder.Build().InvokeAsync(args);