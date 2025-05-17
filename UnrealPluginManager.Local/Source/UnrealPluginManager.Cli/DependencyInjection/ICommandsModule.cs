using Jab;
using UnrealPluginManager.Cli.Commands;

namespace UnrealPluginManager.Cli.DependencyInjection;

/// <summary>
/// Defines a module that provides dependency injection mappings for various command handlers in the Unreal Plugin Manager CLI.
/// </summary>
/// <remarks>
/// This interface is annotated with attributes to configure the dependency injection container
/// for scoped services related to specific command options handlers.
/// Each command options handler is responsible for processing a specific type of command.
/// </remarks>
/// <example>
/// - BuildCommandHandler handles operations associated with <see cref="BuildCommandOptions"/>.
/// - InstallCommandHandler handles operations associated with <see cref="InstallCommandOptions"/>.
/// - SearchCommandHandler handles operations associated with <see cref="SearchCommandOptions"/>.
/// - UploadCommandHandler handles operations associated with <see cref="UploadCommandOptions"/>.
/// - VersionsCommandHandler handles operations associated with <see cref="VersionsCommandOptions"/>.
/// </example>
[ServiceProviderModule]
[Scoped<ICommandOptionsHandler<BuildCommandOptions>, BuildCommandHandler>]
[Scoped<ICommandOptionsHandler<InstallCommandOptions>, InstallCommandHandler>]
[Scoped<ICommandOptionsHandler<SearchCommandOptions>, SearchCommandHandler>]
[Scoped<ICommandOptionsHandler<UploadCommandOptions>, UploadCommandHandler>]
[Scoped<ICommandOptionsHandler<VersionsCommandOptions>, VersionsCommandHandler>]
public interface ICommandsModule;