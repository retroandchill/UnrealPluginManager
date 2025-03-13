namespace UnrealPluginManager.Core.Model.Plugins;

public class PluginVersionDetails : PluginVersionInfo {
  
  public string? Description { get; set; }

  public List<BinariesOverview> Binaries { get; set; } = [];

}