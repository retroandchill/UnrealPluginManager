using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides extension methods for IServiceCollection to add system abstractions
/// and core services for the Unreal Plugin Manager.
/// </summary>
public static class ServiceUtils {

    /// <summary>
    /// Adds system abstractions to the specified IServiceCollection. This includes
    /// implementations for file system, environment, process runner, and optionally
    /// registry services on Windows.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the system abstractions will be added.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddSystemAbstractions(this IServiceCollection services) {
        services.AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IEnvironment, SystemEnvironment>()
            .AddSingleton<IProcessRunner, ProcessRunner>();

        if (OperatingSystem.IsWindows()) {
            services.AddSingleton<IRegistry, WindowsRegistry>();
        }
        
        return services;
    }

    /// <summary>
    /// Adds core services to the specified IServiceCollection. This includes
    /// services essential for the core functionality of the Unreal Plugin Manager.
    /// </summary>
    /// <param name="services">The IServiceCollection to which the core services will be added.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddCoreServices(this IServiceCollection services) {
        return services
            .AddScoped<IPluginService, PluginService>()
            .AddScoped<IPluginStructureService, PluginStructureService>();
    }
    
}