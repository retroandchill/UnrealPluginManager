using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Model.EngineFile;

/// <summary>
/// Represents a contract for objects that hold a collection of plugin dependencies.
/// </summary>
public interface IDependencyHolder {
  /// <summary>
  /// Gets the list of plugin reference descriptors associated with a dependency holder.
  /// </summary>
  /// <remarks>
  /// This property represents a collection of plugin references, defined by the
  /// <see cref="PluginReferenceDescriptor"/> class. These references provide configuration
  /// details about plugins, such as their names, enablement status, and platform compatibility.
  /// The property can be utilized to access, manage, and evaluate plugin dependencies within
  /// the context of engine or project configurations.
  /// </remarks>
  List<PluginReferenceDescriptor> Plugins { get; }
}