using System.CommandLine;
using Semver;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Commands;

/// <summary>
/// Represents the command for installing a plugin into the Unreal Engine.
/// </summary>
/// <remarks>
/// The <see cref="InstallCommand"/> class is a CLI command that allows the user
/// to install a specified plugin into an Unreal Engine instance. It supports
/// specifying the plugin name, the desired version, and the engine version
/// for compatibility during installation.
/// </remarks>
public class InstallCommand : Command<InstallCommandOptions, InstallCommandHandler> {
    /// <summary>
    /// Represents the command used for installing software plugins into a specified Unreal Engine instance.
    /// </summary>
    /// <remarks>
    /// The <see cref="InstallCommand"/> class is part of a CLI tool built for managing Unreal Engine plugins.
    /// This command utilizes arguments and options to determine which plugin to install, its version,
    /// and the target Unreal Engine version. The installation process relies on dependency injection for
    /// access to underlying services.
    /// </remarks>
    /// <example>
    /// This command is added to the CLI root command and provides functionality for plugin installation.
    /// Users can invoke this command with the appropriate arguments and options, such as plugin name,
    /// version, and Unreal Engine version.
    /// </example>
    public InstallCommand() : base("install", "Install the specified plugin into the engine") {
        AddArgument(new Argument<string>("input", "The name of plugin to install"));
        AddOption(new Option<SemVersionRange>(aliases: ["--version", "-v"], description: "The version of the plugin to install",
            parseArgument: r => r.Tokens.Count == 1 ? SemVersionRange.Parse(r.Tokens[0].Value) : SemVersionRange.AllRelease)  {
            IsRequired = false,
        });
        AddOption(new Option<string>(["--engine-version", "-e"], "The version of the engine to build the plugin for") {
            IsRequired = false,
        });
    }
}

/// <summary>
/// Represents the options required for the "install" command in the CLI tool.
/// </summary>
/// <remarks>
/// The <see cref="InstallCommandOptions"/> class defines the parameters needed
/// to execute the installation of a plugin. These parameters include the plugin name,
/// the version range of the plugin, and the target Unreal Engine version. These options
/// are used to customize and validate the installation process.
/// </remarks>
public class InstallCommandOptions : ICommandOptions {
    /// <summary>
    /// Gets or sets the input value representing the name of the plugin to be installed.
    /// </summary>
    /// <remarks>
    /// This property specifies the mandatory identifier for the plugin that the user
    /// wishes to install using the command-line interface. It is a required parameter
    /// utilized during the plugin installation process.
    /// </remarks>
    public required string Input { get; set; }

    /// <summary>
    /// Gets or sets the version range of the plugin to be installed.
    /// </summary>
    /// <remarks>
    /// This property specifies the acceptable version range of the plugin that the user
    /// intends to install. It supports specifying exact versions, ranges, or constraints,
    /// allowing flexibility in determining compatible versions during the installation process.
    /// </remarks>
    public SemVersionRange Version { get; set; } = SemVersionRange.AllRelease;

    /// <summary>
    /// Gets or sets the target Unreal Engine version for the installation process.
    /// </summary>
    /// <remarks>
    /// This property specifies the version of Unreal Engine that the plugin should be installed for.
    /// It allows users to target a specific engine version when executing the installation command.
    /// If not specified, it may default to a suitable version as determined by the system or the installer.
    /// </remarks>
    public string? EngineVersion { get; set; }
}

/// <summary>
/// Handles the logic for executing the install command in the CLI.
/// </summary>
/// <remarks>
/// The <see cref="InstallCommandHandler"/> is responsible for coordinating the installation
/// of a specified plugin by utilizing the <see cref="IEngineService"/> to handle the
/// installation process. It takes user-defined options such as the plugin name, version range,
/// and engine version, and ensures the appropriate installation steps are performed.
/// </remarks>
[AutoConstructor]
public partial class InstallCommandHandler : ICommandOptionsHandle<InstallCommandOptions> {
    private readonly IEngineService _engineService;

    /// <inheritdoc />
    public Task<int> HandleAsync(InstallCommandOptions options, CancellationToken cancellationToken) {
        return _engineService.InstallPlugin(options.Input, options.Version, options.EngineVersion);
    }
}