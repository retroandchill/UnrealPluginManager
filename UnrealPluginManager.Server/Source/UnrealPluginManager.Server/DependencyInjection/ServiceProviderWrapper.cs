namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// A wrapper for the <see cref="IServiceProvider"/> that implements the <see cref="IServiceProvider"/> interface.
/// Allows proxy access to services within a dependency injection container while maintaining structure and functionality.
/// </summary>
public readonly record struct ServiceProviderWrapper(IServiceProvider ServiceProvider) : IServiceProvider {
  
  /// <inheritdoc />
  public object? GetService(Type serviceType) {
    return ServiceProvider.GetService(serviceType);
  }
}