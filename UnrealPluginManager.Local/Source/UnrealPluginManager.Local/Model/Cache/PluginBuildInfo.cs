using Semver;

namespace UnrealPluginManager.Local.Model.Cache;

public record PluginBuildInfo {
  public Guid Id { get; init; }

  public required string PluginName { get; init; }

  public required SemVersion PluginVersion { get; init; }

  public required string EngineVersion { get; init; }

  public List<string> Platforms { get; init; } = [];

  public required string DirectoryName { get; init; }

  public required DateTimeOffset BuiltOn { get; init; }

  public Dictionary<string, SemVersion> BuiltWith { get; init; } = [];
}