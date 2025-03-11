using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Exceptions;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Database;
using UnrealPluginManager.Local.Utils;

var rootCommand = new RootCommand {
    new VersionsCommand(),
    new BuildCommand(),
    new InstallCommand(),
    new SearchCommand(),
    new UploadCommand()
};

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseCustomExceptionHandler()
    .UseDependencyInjection(services => {
      services.AddSystemAbstractions()
          .AddDbContext<UnrealPluginManagerContext, LocalUnrealPluginManagerContext>()
          .AddCoreServices()
          .AddLocalServices()
          .AddApis();
    });

await builder.Build().InvokeAsync(args);