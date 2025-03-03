using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents the root node of a dependency chain within the Unreal Plugin Manager framework.
/// </summary>
/// <remarks>
/// DependencyChainRoot is a special case of a dependency chain node, serving as the entry point of the dependency chain.
/// It implements the IDependencyChainNode interface and provides a predefined name and version, along with a list
/// of plugin dependencies that form the primary layer of the chain.
/// </remarks>
public class DependencyChainRoot : IDependencyChainNode {
  /// <summary>
  /// Gets the name of the dependency chain node.
  /// </summary>
  /// <remarks>
  /// The Name property provides a unique identifier or representation of the dependency chain node.
  /// For the `DependencyChainRoot` class, this value is always "$Root".
  /// </remarks>
  public string Name => "$Root";

  /// <inheritdoc />
  public SemVersion Version => new(1, 0, 0);

  /// <inheritdoc />
  public required List<PluginDependency> Dependencies { get; set; }

  /// <inheritdoc />
  public bool Installed => true;

  /// <inheritdoc />
  public int? RemoteIndex => null;
}