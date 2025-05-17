using System.IO.Abstractions;
using Jab;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Local.DependencyInjection;

/// <summary>
/// Represents a service provider module that encapsulates and provides scoped dependencies
/// for engine interaction services within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// The <see cref="IEngineInteractionModule"/> interface is marked with [ServiceProviderModule]
/// to define its role as a module containing service definitions used during dependency injection.
/// It includes several scoped service implementations relevant to plugin and engine management.
/// </remarks>
/// <dependencies>
/// This module includes the following scoped services:
/// - <see cref="IPluginManagementService"/> implemented by <see cref="PluginManagementService"/>.
/// - <see cref="IEnginePlatformService"/>, created via a factory method depending on the operating system.
/// - <see cref="IEngineService"/> implemented by <see cref="EngineService"/>.
/// - <see cref="IBinaryCacheService"/> implemented by <see cref="BinaryCacheService"/>.
/// - <see cref="ISourceDownloadService"/> implemented by <see cref="SourceDownloadService"/>.
/// - <see cref="IInstallService"/> implemented by <see cref="InstallService"/>.
/// </dependencies>
/// <factory>
/// Defines a factory method <c>CreateEnginePlatformService</c> for creating instances of
/// <see cref="IEnginePlatformService"/> depending on the operating system platform
/// (Windows or POSIX-based systems).
/// </factory>
[ServiceProviderModule]
[Scoped<IPluginManagementService, PluginManagementService>]
[Scoped<IEnginePlatformService>(Factory = nameof(CreateEnginePlatformService))]
[Scoped<IEngineService, EngineService>]
[Scoped<IBinaryCacheService, BinaryCacheService>]
[Scoped<ISourceDownloadService, SourceDownloadService>]
[Scoped<IInstallService, InstallService>]
public interface IEngineInteractionModule {
  
  /// <summary>
  /// Creates and returns an instance of <see cref="IEnginePlatformService"/> based on the operating system.
  /// On Windows, it creates a <see cref="WindowsEnginePlatformService"/> instance.
  /// On non-Windows environments, it creates a <see cref="PosixEnginePlatformService"/> instance.
  /// </summary>
  /// <param name="fileSystem">An instance of <see cref="IFileSystem"/> to interact with the file system.</param>
  /// <param name="registry">An instance of <see cref="IRegistry"/> to interact with the system registry (on Windows).</param>
  /// <returns>
  /// An implementation of <see cref="IEnginePlatformService"/> appropriate for the detected operating system.
  /// </returns>
  static IEnginePlatformService CreateEnginePlatformService(IFileSystem fileSystem, IRegistry registry) {
    return OperatingSystem.IsWindows()
        ? new WindowsEnginePlatformService(fileSystem, registry)
        : new PosixEnginePlatformService();
  }
  
}