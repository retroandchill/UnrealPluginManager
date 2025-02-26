using System.CommandLine;
using LanguageExt;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Cli.Commands;

using StringOption = System.CommandLine.Option<string>;

/// <summary>
/// Represents the search command used for querying plugins either from a local cache or a specified remote location.
/// </summary>
/// <remarks>
/// The <c>SearchCommand</c> is designed to execute searches based on a provided search term.
/// It supports an optional argument for specifying a remote to search against, with "local cache" as the default.
/// If the "all" option is provided for the remote, the search will be executed across all configured remotes.
/// This class extends the base <c>Command</c>, defining required arguments and options for search functionality.
/// </remarks>
public class SearchCommand : Command<SearchCommandOptions, SearchCommandHandler> {

    /// <summary>
    /// Represents a search command designed to query the local cache or specified remote locations for plugins matching a search term.
    /// </summary>
    /// <remarks>
    /// The command accepts a required argument for the search term and an optional option to specify a remote.
    /// If the remote is not specified, the local cache is searched by default. Specifying "all" for the remote
    /// allows searching across all configured remotes.
    /// This class extends the generic <c>Command</c>, providing specific arguments and options for search functionality.
    /// </remarks>
    public SearchCommand() : base("search", "Searches either the local cache or a remote for plugins matching the search term.") {
        AddArgument(new Argument<string>("searchTerm", "The search term to use."));
        AddOption(new StringOption(["--remote", "-r"], "The remote to search. If not specified, the local cache will be searched. Using all will search all remotes.") {
            IsRequired = false
        });
    }
}

/// <summary>
/// Represents the options for the SearchCommand.
/// </summary>
/// <remarks>
/// This class encapsulates the input parameters required for the execution of the SearchCommand,
/// including the search term and an optional remote name to specify the source of the search.
/// </remarks>
public class SearchCommandOptions : ICommandOptions {

    /// <summary>
    /// Gets or sets the search term used to filter plugins during the execution of the search command.
    /// </summary>
    /// <remarks>
    /// This property represents the keyword or query used to perform the search operation.
    /// It is required to identify plugins that match the criteria specified by the user.
    /// </remarks>
    public string SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the name of the remote source to query during the search operation.
    /// </summary>
    /// <remarks>
    /// This property specifies the target remote source for the search command.
    /// If no remote name is provided, the search defaults to querying the local cache.
    /// </remarks>
    public string? Remote { get; set; }
    
}

/// <summary>
/// Handles the execution of the SearchCommand.
/// </summary>
/// <remarks>
/// This class implements the logic for processing the SearchCommand based on the provided options.
/// It supports both local and remote plugin search functionality.
/// </remarks>
[AutoConstructor]
public partial class SearchCommandHandler : ICommandOptionsHandler<SearchCommandOptions> {
    private readonly IConsole _console;
    private readonly IPluginService _pluginService;
    private readonly IPluginManagementService _pluginManagementService;


    /// <inheritdoc />
    public Task<int> HandleAsync(SearchCommandOptions options, CancellationToken cancellationToken) {
        return options.Remote.Match(
            r => ReportRemotePlugins(options.SearchTerm, r),
            () => ReportLocalPlugins(options.SearchTerm));
    }

    private int ReportPlugins(IEnumerable<PluginOverview> plugins) {
        bool hasResult = false;
        foreach (var plugin in plugins) {
            hasResult = true;
            _console.WriteLine(plugin.Name);
            foreach (var version in plugin.Versions) {
                _console.WriteLine($"- {version.Version}");
            }
        }

        if (!hasResult) {
            _console.WriteLine("No results found.");
        }

        return 0;
    }

    private async Task<int> ReportLocalPlugins(string searchTerm) {
        var plugins = await _pluginService.ListPlugins(searchTerm);
        return ReportPlugins(plugins);
    }

    private async Task<int> ReportRemotePlugins(string searchTerm, string remote) {
        if (remote.Equals("ALL", StringComparison.InvariantCultureIgnoreCase)) {
            var plugins = await _pluginManagementService.GetPlugins(searchTerm);
            if (plugins.All(x => x.Value.IsFail)) {
                _console.WriteLine("Error: Communication with remote failed.");
                return -1;
            }
            foreach (var (key, value) in plugins) {
                _console.WriteLine($"Remote: {key}");
                value.Match(x => ReportPlugins(x),
                    e => _console.WriteLine($"Error: {e.Message}"));
                _console.WriteLine("");
            }
            
            return 0;
        } else {
            var plugins = await _pluginManagementService.GetPlugins(remote, searchTerm);
            return ReportPlugins(plugins);
        }
    }
}