using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.Database;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Cli.Utils;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

var rootCommand = new RootCommand {
    new VersionsCommand(),
    new BuildCommand(),
    new InstallCommand()
};

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseExceptionHandler(errorExitCode: 1)
    .UseDependencyInjection(services => {
        services.AddSystemAbstractions()
            .AddDbContext<UnrealPluginManagerContext, LocalUnrealPluginManagerContext>()
            .AddCoreServices()
            .AddCliServices();
    });
    
await builder.Build().InvokeAsync(args);