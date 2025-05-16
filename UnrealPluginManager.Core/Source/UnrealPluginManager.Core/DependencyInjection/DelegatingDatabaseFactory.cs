using Microsoft.EntityFrameworkCore;
using Retro.ReadOnlyParams.Annotations;

namespace UnrealPluginManager.Core.DependencyInjection;

/// <summary>
/// A factory class that delegates the creation of database context instances to a provided delegate function.
/// </summary>
/// <typeparam name="TContext">
/// The type of the database context that this factory creates. Must inherit from <see cref="DbContext"/>.
/// </typeparam>
public class DelegatingDatabaseFactory<TContext>([ReadOnly] Func<IServiceProvider, TContext> factory)
    : IDatabaseFactory<TContext> where TContext : DbContext {

  /// <inheritdoc />
  public TContext Create(IServiceProvider serviceProvider) {
    return factory(serviceProvider);
  }
}