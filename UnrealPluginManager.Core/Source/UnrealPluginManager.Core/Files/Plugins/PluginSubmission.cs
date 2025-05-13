using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Core.Files.Plugins;

/// <summary>
/// Represents a submission for a plugin, containing the manifest, patches, optional icon stream, and optional readme text.
/// </summary>
/// <remarks>
/// The <see cref="PluginSubmission"/> struct is used to encapsulate all the data relevant to a plugin submission in a structured manner.
/// This includes the metadata describing the plugin, a list of relevant patch file paths, an optional icon stream, and an optional readme text.
/// </remarks>
/// <param name="Manifest">
/// The <see cref="PluginManifest"/> instance containing metadata and configuration details for the plugin.
/// </param>
/// <param name="Patches">
/// A read-only list of strings representing the patch files associated with the plugin.
/// </param>
/// <param name="IconStream">
/// An optional <see cref="Stream"/> for the plugin's icon image, or null if no icon is associated with the plugin.
/// </param>
/// <param name="ReadmeText">
/// An optional string containing the content of the plugin's readme, or null if no readme content is provided.
/// </param>
/// <remarks>
/// This struct implements both <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/> to ensure resources,
/// particularly the optional <paramref name="IconStream"/>, are disposed correctly.
/// </remarks>
public readonly record struct PluginSubmission(
    PluginManifest Manifest,
    IReadOnlyList<string> Patches,
    Stream? IconStream = null,
    string? ReadmeText = null) : IDisposable, IAsyncDisposable {
  /// <inheritdoc />
  public void Dispose() {
    IconStream?.Dispose();
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync() {
    if (IconStream != null) await IconStream.DisposeAsync();
  }
}