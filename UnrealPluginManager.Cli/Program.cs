using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testably.Abstractions;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.Database;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;

var rootCommand = new RootCommand {
    new VersionsCommand(),
    new BuildCommand()
};

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseExceptionHandler(errorExitCode: 1)
    .UseDependencyInjection(services => {
        services.AddSingleton<IFileSystem, RealFileSystem>();
        services.AddDbContext<UnrealPluginManagerContext, SqliteUnrealPluginManagerContext>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IPluginService, PluginService>();
        if (OperatingSystem.IsWindows()) {
            services.AddScoped<IEnginePlatformService, WindowsEnginePlatformService>();
        } else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()) {
            services.AddScoped<IEnginePlatformService, PosixEnginePlatformService>();
        } else {
            throw new PlatformNotSupportedException("The given platform is not supported.");
        }
        services.AddScoped<IEngineService, EngineService>();
    });
    
await builder.Build().InvokeAsync(args);