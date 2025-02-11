namespace UnrealPluginManager.Cli.Model.Engine;

public class InstalledEngine {
    
    public Version Version { get; set; }

    public string Name { get; set; }

    public DirectoryInfo Directory { get; set; }

    public bool CustomBuild { get; set; }

}