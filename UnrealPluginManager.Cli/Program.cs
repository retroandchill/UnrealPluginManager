using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Services;

var rootCommand = new RootCommand {
    new VersionsCommand()
};

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseDependencyInjection(services => {
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddScoped<IStorageService, LocalStorageService>();
        if (OperatingSystem.IsWindows()) {
            services.AddScoped<IEngineService, WindowsEngineService>();
        } else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()) {
            services.AddScoped<IEngineService, PosixEngineService>();
        } else {
            // TODO: Add Mac and Linux support
            throw new PlatformNotSupportedException("The given platform is not supported.");
        }
        
    });
    
await builder.Build().InvokeAsync(args);