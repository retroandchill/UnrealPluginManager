using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Exceptions;

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
    .UseDependencyInjection(console => new CliServiceProvider(console));

await builder.Build().InvokeAsync(args);