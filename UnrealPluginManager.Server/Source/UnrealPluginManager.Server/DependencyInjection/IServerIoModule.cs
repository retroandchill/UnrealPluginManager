using Jab;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Represents a server input/output module, providing specific service bindings
/// and implementations for server-side operations.
/// </summary>
/// <remarks>
/// This interface includes dependency injection declarations for server-wide
/// services such as JSON serialization and storage handling. It also defines
/// a factory method for creating a custom JSON service instance.
/// </remarks>
[ServiceProviderModule]
[Singleton<IJsonService>(Factory = nameof(CreateJsonService))]
[Singleton<IStorageService, CloudStorageService>]
public interface IServerIoModule {
  /// <summary>
  /// Creates an instance of the <see cref="JsonService"/> using the provided service provider.
  /// </summary>
  /// <param name="serviceProvider">
  /// An instance of <see cref="ServiceProviderWrapper"/> that provides access
  /// to the required services for configuring the <see cref="JsonService"/>.
  /// </param>
  /// <returns>
  /// A newly created instance of <see cref="JsonService"/> configured with
  /// options retrieved from the service provider.
  /// </returns>
  static JsonService CreateJsonService(IOptions<JsonOptions> options) {
    return new JsonService(options.Value.JsonSerializerOptions);
  }
}