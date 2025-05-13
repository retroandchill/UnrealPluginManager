using System.CommandLine;
using JetBrains.Annotations;
using Semver;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Cli.Commands;

/// <summary>
/// Represents the "upload" command in the Unreal Plugin Manager CLI. This command
/// allows the user to upload a specified plugin to a designated remote repository.
/// </summary>
/// <remarks>
/// The UploadCommand class provides support for specifying the plugin name, its version,
/// and an optional remote to control where the plugin should be uploaded.
/// </remarks>
/// <example>
/// The command supports arguments and options to specify the desired behavior.
/// </example>
/// <seealso cref="Command{TOptions, TOptionsHandler}" />
public class UploadCommand : Command<UploadCommandOptions, UploadCommandOptionsHandler> {
  /// <summary>
  /// Represents a command for uploading a plugin to a remote destination in the Unreal Plugin Manager CLI.
  /// </summary>
  /// <remarks>
  /// This command provides functionality for specifying the plugin name, version, and optionally a remote target.
  /// It is utilized to publish or share plugins via specified endpoints or repositories.
  /// </remarks>
  /// <seealso cref="Command{TOptions, TOptionsHandler}" />
  public UploadCommand() : base("upload", "Uploads a plugin to the specified remote.") {
    AddArgument(new Argument<string>("name", "The name of the plugin to upload"));
    AddOption(new Option<SemVersion>(["-v", "--version"], description: "The version of the plugin to upload",
        parseArgument: r => SemVersion.Parse(r.Tokens[0].Value)) {
        IsRequired = true,
    });
    AddOption(new Option<string>(["-r", "--remote"], description: "The remote to upload the plugin to") {
        IsRequired = false,
    });
  }

}

/// <summary>
/// Represents the options used for the "upload" command in the Unreal Plugin Manager CLI. These options
/// define the parameters required to upload a plugin to a specified remote repository.
/// </summary>
/// <remarks>
/// The UploadCommandOptions class allows for specifying the plugin name, its version, and an optional
/// remote location where the plugin should be uploaded. These options are consumed by the command handler
/// to perform the upload operation.
/// </remarks>
/// <seealso cref="ICommandOptions" />
[UsedImplicitly]
public class UploadCommandOptions : ICommandOptions {
  /// <summary>
  /// Gets or sets the name of the plugin to be uploaded.
  /// </summary>
  /// <remarks>
  /// This property specifies the identifier for the plugin that is being uploaded.
  /// It is a required field and should contain a unique name representing the plugin.
  /// </remarks>
  [UsedImplicitly]
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets the version of the plugin to be uploaded.
  /// </summary>
  /// <remarks>
  /// This property specifies the version identifier of the plugin being uploaded.
  /// It is a required field and follows semantic versioning standards.
  /// </remarks>
  [UsedImplicitly]
  public required SemVersion Version { get; set; }

  /// <summary>
  /// Gets or sets the remote location where the plugin should be uploaded.
  /// </summary>
  /// <remarks>
  /// This property specifies the optional remote repository or destination
  /// where the plugin will be uploaded. If not set, the default remote
  /// configuration will be used.
  /// </remarks>
  [UsedImplicitly]
  public string? Remote { get; set; }

}

/// <summary>
/// Handles the processing of options for the "upload" command in the Unreal Plugin Manager CLI.
/// </summary>
/// <remarks>
/// The UploadCommandOptionsHandler class is responsible for interpreting and executing
/// the behavior defined by the options of the "upload" command. This includes overseeing
/// the logic required for handling specific parameters and ensuring correct operation based
/// on the user-provided input.
/// </remarks>
/// <seealso cref="ICommandOptionsHandler{UploadCommandOptions}" />
[AutoConstructor]
[UsedImplicitly]
public partial class UploadCommandOptionsHandler : ICommandOptionsHandler<UploadCommandOptions> {
  private readonly IConsole _console;
  private readonly IPluginManagementService _pluginManagementService;

  /// <inheritdoc />
  public async Task<int> HandleAsync(UploadCommandOptions options, CancellationToken cancellationToken) {
    _console.WriteLine($"Uploading plugin {options.Name}...");
    await _pluginManagementService.UploadPlugin(options.Name, options.Version, options.Remote);
    _console.WriteLine("Upload successful!");
    return 0;
  }
}