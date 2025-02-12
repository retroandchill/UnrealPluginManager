namespace UnrealPluginManager.Cli.Model.Engine;

public class InstalledEngine {
    
    public required string Key { get; set; }
    
    public required Version Version { get; set; }

    public required string Name { get; set; }

    public required DirectoryInfo Directory { get; set; }

    public required bool CustomBuild { get; set; }

}