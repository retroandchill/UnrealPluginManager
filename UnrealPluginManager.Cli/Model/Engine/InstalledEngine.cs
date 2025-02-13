using System.IO.Abstractions;

namespace UnrealPluginManager.Cli.Model.Engine;

public record InstalledEngine(string Name, Version Version, IDirectoryInfo Directory, bool CustomBuild = false) {

    public string DisplayName => CustomBuild ? $"{Name}: Custom Build" : $"{Version}: Installed";
    
    public string EngineDirectory => Path.Join(Directory.FullName, "Engine");
    
    public string BatchFilesDirectory => Path.Join(EngineDirectory, "Build", "BatchFiles");
    
    public string PluginDirectory => Path.Join(EngineDirectory, "Plugins");
    
    public string MarketplaceDirectory => Path.Join(PluginDirectory, "Marketplace");
    
    public string PackageDirectory => Path.Join(MarketplaceDirectory, ".UnrealPluginManager");

}