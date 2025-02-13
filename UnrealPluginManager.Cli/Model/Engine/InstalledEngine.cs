using System.IO.Abstractions;

namespace UnrealPluginManager.Cli.Model.Engine;

public record InstalledEngine(string Name, Version Version, IDirectoryInfo Directory, bool CustomBuild = false) {

    public string DisplayName => CustomBuild ? $"{Name}: Custom Build" : $"{Version}: Installed";

}