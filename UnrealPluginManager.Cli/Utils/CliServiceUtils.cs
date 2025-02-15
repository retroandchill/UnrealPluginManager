﻿using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Utils;

/// <summary>
/// Provides utility methods to register CLI services into the dependency injection container
/// for platform-specific and general service implementations.
/// </summary>
public static class CliServiceUtils {

    /// <summary>
    /// Registers CLI services into the dependency injection container. It configures platform-specific
    /// implementations based on the operating system and adds general service dependencies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the CLI services are registered.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown if the operating system is not Windows, macOS, or Linux.
    /// </exception>
    public static IServiceCollection AddCliServices(this IServiceCollection services) {
        if (OperatingSystem.IsWindows()) {
            services.AddScoped<IRegistry, WindowsRegistry>();
            services.AddScoped<IEnginePlatformService, WindowsEnginePlatformService>();
        } else if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()) {
            services.AddScoped<IEnginePlatformService, PosixEnginePlatformService>();
        } else {
            throw new PlatformNotSupportedException("The given platform is not supported.");
        }
        return services.AddScoped<IEngineService, EngineService>()
            .AddScoped<IStorageService, LocalStorageService>();
    }
    
}