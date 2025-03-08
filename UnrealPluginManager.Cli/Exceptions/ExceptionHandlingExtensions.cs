using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;

namespace UnrealPluginManager.Cli.Exceptions;

/// <summary>
/// Provides extension methods for configuring custom exception handling
/// in command-line applications built with System.CommandLine.
/// </summary>
public static class ExceptionHandlingExtensions {

  /// <summary>
  /// Adds a custom exception handler to the command-line builder.
  /// This method configures the command-line application to use a custom handler
  /// for unhandled exceptions, allowing for tailored exception processing and response.
  /// </summary>
  /// <param name="builder">The <see cref="CommandLineBuilder"/> instance to which the exception handler will be added.</param>
  /// <returns>The updated <see cref="CommandLineBuilder"/> instance with the custom exception handler configured.</returns>
  public static CommandLineBuilder UseCustomExceptionHandler(this CommandLineBuilder builder) {
    return builder.UseExceptionHandler(ProcessException);
  }

  /// <summary>
  /// Registers a custom exception handler within the dependency injection container.
  /// This method configures the dependency injection framework to resolve and use a custom
  /// exception handler for handling CLI application errors.
  /// </summary>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the custom exception handler will be added.</param>
  /// <returns>The updated <see cref="IServiceCollection"/> with the custom exception handler service registered.</returns>
  public static IServiceCollection AddCustomExceptionHandler(this IServiceCollection services) {
    return services
        .AddSingleton<ICliExceptionHandler, CliExceptionHandler>();
  }
  
  private static void ProcessException(Exception ex, InvocationContext context) {
    var serviceProvider = context.BindingContext.GetRequiredService<IServiceProvider>();
    var handler = serviceProvider.GetRequiredService<ICliExceptionHandler>();
    context.ExitCode = handler.HandleException(ex);
  }
  
}