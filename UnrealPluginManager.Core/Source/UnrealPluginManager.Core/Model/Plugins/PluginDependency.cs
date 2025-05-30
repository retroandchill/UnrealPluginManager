﻿using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents a dependency of a plugin within the Unreal Plugin Manager framework.
/// </summary>
/// <remarks>
/// A plugin dependency contains the name of the dependent plugin, its type, and a version specification
/// indicating the compatible versions of the plugin.
/// </remarks>
public class PluginDependency {

  /// <summary>
  /// Gets the name of the plugin.
  /// </summary>
  /// <remarks>
  /// The name represents the unique identifier for the plugin and must conform to
  /// specified naming conventions, such as starting with an uppercase letter and
  /// not including whitespace. This property is required and has a length
  /// limitation between 1 and 255 characters.
  /// </remarks>
  public required string PluginName { get; set; }

  /// <summary>
  /// Gets the version range of the dependent plugin.
  /// </summary>
  /// <remarks>
  /// The version range specifies the compatible versions of the plugin that
  /// this dependency applies to. It supports semantic versioning constraints
  /// and allows for defining a range or specific version that meets the compatibility requirements.
  /// </remarks>
  [JsonConverter(typeof(SemVersionRangeJsonConverter))]
  public required SemVersionRange PluginVersion { get; set; }
}