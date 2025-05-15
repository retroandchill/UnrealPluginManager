using System.CommandLine;
using Jab;
using UnrealPluginManager.Cli.Commands;
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
[Import(typeof(ILocalServiceProviderModule))]
[Singleton(typeof(IConsole), Instance = nameof(Console))]
[Scoped(typeof(ICommandOptionsHandler<BuildCommandOptions>), typeof(BuildCommandHandler))]
[Scoped(typeof(ICommandOptionsHandler<InstallCommandOptions>), typeof(InstallCommandHandler))]
[Scoped(typeof(ICommandOptionsHandler<SearchCommandOptions>), typeof(SearchCommandHandler))]
[Scoped(typeof(ICommandOptionsHandler<UploadCommandOptions>), typeof(UploadCommandHandler))]
[Scoped(typeof(ICommandOptionsHandler<VersionsCommandOptions>), typeof(VersionsCommandHandler))]
public sealed partial class CliServiceProvider(IConsole console) {
  private IConsole Console { get; } = console;
}