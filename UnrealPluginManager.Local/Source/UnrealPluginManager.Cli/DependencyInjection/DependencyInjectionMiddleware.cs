using System.CommandLine.Builder;

namespace UnrealPluginManager.Cli.DependencyInjection;

/// <summary>
/// Provides middleware for integrating dependency injection with the command line processing pipeline.
/// </summary>
public static class DependencyInjectionMiddleware {
  /// <summary>
  /// Adds dependency injection to the command line processing pipeline
  /// by configuring the service collection with the provided configuration action.
  /// </summary>
  /// <param name="builder">The <see cref="CommandLineBuilder"/> instance to apply the dependency injection middleware to.</param>
  /// <returns>The modified <see cref="CommandLineBuilder"/> instance, allowing further configuration.</returns>
  public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder) {
    return builder.AddMiddleware(async (context, next) => {
      await using var serviceProvider = new CliServiceProvider(context.Console);
      context.BindingContext.AddService<IServiceProvider>(_ => serviceProvider);
      await next(context);
    });
  }
}