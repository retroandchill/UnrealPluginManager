namespace UnrealPluginManager.Core.Model.Plugins.Recipes;

/// <summary>
/// Represents the location of a source, containing information about its URL and associated SHA hash.
/// </summary>
public record struct SourceLocation {

  /// <summary>
  /// Gets or sets the URL representing the location of the source.
  /// </summary>
  public required Uri Url { get; init; }

  /// <summary>
  /// Gets or sets the SHA hash associated with the source location.
  /// </summary>
  public required string Sha { get; init; }

}