using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;

namespace UnrealPluginManager.Cli.Commands;

/// <summary>
/// Represents a marker interface for command options used in the CLI commands.
/// </summary>
/// <remarks>
/// This interface is implemented by various command option classes to define
/// the configuration and input parameters required for specific CLI commands.
/// </remarks>
public interface ICommandOptions;

/// <summary>
/// Defines a contract for handling command options for CLI commands.
/// </summary>
/// <typeparam name="TOptions">The type of the command options to be handled.</typeparam>
/// <remarks>
/// This interface provides an abstraction for processing specific command options
/// passed to various CLI commands, enabling the implementation of command behavior
/// through the <c>HandleAsync</c> method.
/// </remarks>
public interface ICommandOptionsHandler<in TOptions> {
  /// <summary>
  /// Handles the specified command options asynchronously.
  /// </summary>
  /// <param name="options">The command options to handle.</param>
  /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
  /// <returns>
  /// A task that represents the asynchronous operation, returning an integer result
  /// where 0 typically indicates success, and other values may indicate errors or specific statuses.
  /// </returns>
  Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken);
}

/// <summary>
/// Serves as a generic base class for CLI commands, encapsulating the logic
/// for handling specific command options and their corresponding handlers.
/// </summary>
/// <typeparam name="TOptions">
/// The type of options that the command can process, implementing <see cref="ICommandOptions"/>.
/// </typeparam>
/// <typeparam name="TOptionsHandler">
/// The type of the handler that processes the command options,
/// implementing <see cref="ICommandOptionsHandler{TOptions}"/>.
/// </typeparam>
/// <remarks>
/// This class provides the foundation for creating commands in a CLI application.
/// It automatically sets up a command handler that binds the specified options and handles them
/// using the provided <typeparamref name="TOptionsHandler"/> implementation.
/// </remarks>
public abstract class Command<TOptions, TOptionsHandler> : Command
    where TOptions : class, ICommandOptions
    where TOptionsHandler : ICommandOptionsHandler<TOptions> {
  /// <summary>
  /// Serves as a base class for defining commands in the CLI tool, supporting specific option handlers
  /// for processing command-line arguments and related operations.
  /// </summary>
  /// <remarks>
  /// This abstract class simplifies the process of setting up commands by automatically configuring
  /// a handler that binds the provided options and invokes their corresponding operations through
  /// the specified handler implementation.
  /// </remarks>
  protected Command(string name, string description) : base(name, description) {
    Handler = CommandHandler.Create<TOptions, IServiceProvider, CancellationToken>(HandleOptions);
  }

  private static async Task<int> HandleOptions(TOptions options, IServiceProvider serviceProvider,
                                               CancellationToken cancellationToken) {
    var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(serviceProvider);
    return await handler.HandleAsync(options, cancellationToken);
  }
}