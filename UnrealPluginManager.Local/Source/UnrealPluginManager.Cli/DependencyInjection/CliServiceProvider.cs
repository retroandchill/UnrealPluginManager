using System.CommandLine;
using Jab;
using UnrealPluginManager.Local.DependencyInjection;

namespace UnrealPluginManager.Cli.DependencyInjection;

/// <summary>
/// Provides CLI-specific dependency injection services for the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// This service provider defines a set of scoped dependencies for handling CLI commands, associating specific
/// command options with their respective handlers. It also integrates services from a local service provider module.
/// </remarks>
[ServiceProvider]
[Import<ILocalServiceProviderModule>]
[Import<ICommandsModule>]
[Singleton<IConsole>(Instance = nameof(Console))]
public sealed partial class CliServiceProvider(IConsole console) {
  private IConsole Console { get; } = console;
}