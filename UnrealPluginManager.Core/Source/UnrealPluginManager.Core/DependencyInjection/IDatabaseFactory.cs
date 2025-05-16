using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.DependencyInjection;

/// <summary>
/// Represents a factory interface for creating instances of database contexts.
/// </summary>
/// <typeparam name="TContext">The type of the database context that this factory creates. Must inherit from <see cref="DbContext"/>.</typeparam>
public interface IDatabaseFactory<out TContext> where TContext : DbContext {

  /// <summary>
  /// Creates and returns a new instance of the database context using the provided service provider.
  /// </summary>
  /// <param name="serviceProvider">
  /// The service provider used to resolve dependencies for the database context.
  /// </param>
  /// <returns>
  /// A new instance of the database context.
  /// </returns>
  TContext Create(IServiceProvider serviceProvider);

}