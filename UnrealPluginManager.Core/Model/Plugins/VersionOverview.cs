using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

public class VersionOverview {
    
    public ulong Id { get; set; }
    
    public SemVersion Version { get; set; }
    
}