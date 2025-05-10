using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and processing the structure of Unreal Engine plugin directories.
/// </summary>
public class PluginStructureService : IPluginStructureService {
  private const string Binaries = "Binaries";
  private const string Intermediate = "Intermediate";
  private const string IntermediateBuild = $"{Intermediate}/Build";

  /// <inheritdoc />
  public List<string> GetInstalledBinaries(IDirectoryInfo pluginDirectory) {
    return pluginDirectory.EnumerateDirectories(Binaries, SearchOption.TopDirectoryOnly)
        .Concat(pluginDirectory.EnumerateDirectories(IntermediateBuild, SearchOption.TopDirectoryOnly))
        .Select(x => x.Name)
        .Distinct()
        .ToList();
  }
}