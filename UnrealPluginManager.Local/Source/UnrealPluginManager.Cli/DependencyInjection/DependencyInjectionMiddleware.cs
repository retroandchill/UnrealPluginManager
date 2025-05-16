using System.CommandLine;
using System.CommandLine.Builder;

namespace UnrealPluginManager.Cli.DependencyInjection;

/// <summary>
/// Provides middleware for integrating dependency injection with the command line processing pipeline.
/// </summary>
public static class DependencyInjectionMiddleware {
  /// <summary>
  /// Adds dependency injection to the command line processing pipeline
  /// by configuring the service collection using the specified service provider factory method.
  /// </summary>
  /// <typeparam name="TProvider">The type of the service provider, which must implement <see cref="IServiceProvider"/> and <see cref="IAsyncDisposable"/>.</typeparam>
  /// <param name="builder">The <see cref="CommandLineBuilder"/> instance to configure with dependency injection middleware.</param>
  /// <param name="serviceProviderFactory">A factory method to create an instance of the service provider, which receives an <see cref="IConsole"/> as input.</param>
  /// <returns>The configured <see cref="CommandLineBuilder"/>, allowing further customization.</returns>
  public static CommandLineBuilder UseDependencyInjection<TProvider>(this CommandLineBuilder builder,
                                                                     Func<IConsole, TProvider> serviceProviderFactory)
      where TProvider : IServiceProvider, IAsyncDisposable {
    return builder.AddMiddleware(async (context, next) => {
      var serviceProvider = serviceProviderFactory(context.Console);
      context.BindingContext.AddService<IServiceProvider>(_ => serviceProvider);
      await next(context);
    });
  }
}