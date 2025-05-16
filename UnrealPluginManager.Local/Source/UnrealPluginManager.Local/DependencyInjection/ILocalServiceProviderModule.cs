using System.IO.Abstractions;
using System.Text.Json;
using Jab;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database;
using UnrealPluginManager.Local.Factories;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.WebClient.Api;

namespace UnrealPluginManager.Local.DependencyInjection;

/// <summary>
/// Represents a local service provider module for dependency injection in the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// This interface is annotated with service provider attributes, defining the provisioning of various services
/// for local usage within the application. These services include scoped and singleton dependencies aimed at
/// managing plugins, storage, API interaction, and other core functionalities of the application.
/// </remarks>
[ServiceProviderModule]
[Import(typeof(ISystemAbstractionsModule))]
[Singleton(typeof(IJsonService), Factory = nameof(CreateJsonService))]
[Singleton(typeof(IStorageService), typeof(LocalStorageService))]
[Singleton(typeof(HttpClient))]
[Singleton(typeof(IApiClientFactory), typeof(ApiClientFactory<IPluginsApi, PluginsApi>))]
[Scoped(typeof(UnrealPluginManagerContext), Factory = nameof(GetUnrealPluginManagerContext))]
[Scoped(typeof(LocalUnrealPluginManagerContext))]
[Scoped(typeof(IPluginStructureService), typeof(PluginStructureService))]
[Scoped(typeof(IPluginService), typeof(PluginService))]
[Scoped(typeof(IPluginManagementService), typeof(PluginManagementService))]
[Scoped(typeof(IEnginePlatformService), Factory = nameof(CreateEnginePlatformService))]
[Scoped(typeof(IEngineService), typeof(EngineService))]
[Scoped(typeof(IBinaryCacheService), typeof(BinaryCacheService))]
[Scoped(typeof(IRemoteService), typeof(RemoteService))]
[Scoped(typeof(ISourceDownloadService), typeof(SourceDownloadService))]
[Scoped(typeof(IInstallService), typeof(InstallService))]
public interface ILocalServiceProviderModule {

  /// <summary>
  /// Creates and returns an instance of JsonService configured with specific serialization options.
  /// The options include camel case property naming, camel case dictionary key naming, and indented output.
  /// </summary>
  /// <returns>
  /// An instance of <see cref="JsonService"/> pre-configured with JSON serializer options for camel case naming
  /// and indented formatting.
  /// </returns>
  static JsonService CreateJsonService() {
    var jsonSerializationOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    return new JsonService(jsonSerializationOptions);
  }

  /// <summary>
  /// Retrieves an instance of <see cref="LocalUnrealPluginManagerContext"/> for managing and accessing local Unreal plugin data.
  /// </summary>
  /// <param name="dbContext">
  /// An instance of <see cref="LocalUnrealPluginManagerContext"/> used for local database interactions.
  /// </param>
  /// <returns>
  /// The provided <see cref="LocalUnrealPluginManagerContext"/> instance.
  /// </returns>
  static LocalUnrealPluginManagerContext GetUnrealPluginManagerContext(LocalUnrealPluginManagerContext dbContext) {
    return dbContext;
  }

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