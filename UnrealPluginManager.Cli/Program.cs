using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.Database;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Utils;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Utils;

var rootCommand = new RootCommand {
    new VersionsCommand(),
    new BuildCommand(),
    new InstallCommand(),
    new SearchCommand()
};

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseExceptionHandler(errorExitCode: 1)
    .UseDependencyInjection(services => {
        services.AddSystemAbstractions()
            .AddDbContext<UnrealPluginManagerContext, LocalUnrealPluginManagerContext>()
            .AddCoreServices()
            .AddCliServices()
            .AddApiFactories();
    });
    
await builder.Build().InvokeAsync(args);