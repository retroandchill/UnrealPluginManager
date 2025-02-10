namespace UnrealPluginManager.Cli.Commands;

public class VersionsCommand()
    : Command<VersionsCommandOptions, VerisionsCommandOptionsHandler>("versions",
        "Lists all installed engine versions.");

public class VersionsCommandOptions : ICommandOptions;

public class VerisionsCommandOptionsHandler : ICommandOptionsHandle<VersionsCommandOptions> {
    public Task<int> HandleAsync(VersionsCommandOptions options, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}