using System.IO.Abstractions;
using UnrealPluginManager.Core.Files.Plugins;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and partitioning Unreal Engine plugins.
/// </summary>
public interface IPluginStructureService {
  /// <summary>
  /// Retrieves a list of installed binary directories for a given plugin.
  /// </summary>
  /// <param name="pluginDirectory">The directory containing the plugin's files.</param>
  /// <returns>A list of strings representing the paths to the installed binaries.</returns>
  List<string> GetInstalledBinaries(IDirectoryInfo pluginDirectory);

  /// <summary>
  /// Extracts the plugin submission details from a provided archive stream.
  /// </summary>
  /// <param name="archiveStream">The archive stream containing the plugin data to be extracted.</param>
  /// <returns>A <see cref="PluginSubmission"/> object containing the plugin manifest, patches, optional icon stream, and optional readme text.</returns>
  Task<PluginSubmission> ExtractPluginSubmission(Stream archiveStream);

  /// <summary>
  /// Compresses a plugin submission into a specified stream as an archive.
  /// </summary>
  /// <param name="submission">The plugin submission containing the manifest, patches, optional icon stream, and optional readme text.</param>
  /// <param name="stream">The output stream where the compressed plugin submission will be written.</param>
  /// <returns>A task that represents the asynchronous operation of compressing the plugin submission.</returns>
  Task CompressPluginSubmission(PluginSubmission submission, Stream stream);
}