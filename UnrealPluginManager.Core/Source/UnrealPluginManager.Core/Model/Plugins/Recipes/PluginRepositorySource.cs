namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents the source for a plugin repository.
/// </summary>
public record struct PluginRepositorySource {
  /// <summary>
  /// Gets or sets the URL associated with the plugin repository.
  /// </summary>
  public required Uri Url { get; init; }

  /// <summary>
  /// Gets or sets the reference identifier or name associated with the plugin repository source.
  /// </summary>
  public string? Ref { get; init; }
}