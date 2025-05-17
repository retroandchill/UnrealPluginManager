namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Defines a server service provider interface that extends the <see cref="IServiceProvider"/> interface
/// to support scoped service creation.
/// </summary>
public interface IServerServiceProvider : IServiceProvider {
  /// <summary>
  /// Creates a new scope for resolving scoped services within the server service provider.
  /// </summary>
  /// <returns>
  /// An instance of <see cref="IServerServiceProvider.IScope"/> that represents the new service scope.
  /// </returns>
  IScope CreateScope();

  /// <summary>
  /// Represents an interface that combines the functionality of <see cref="IServiceProvider"/> and <see cref="IServiceScope"/>
  /// to define a scope for dependency injection within the service provider context.
  /// </summary>
  public interface IScope : IServiceProvider, IServiceScope;

}