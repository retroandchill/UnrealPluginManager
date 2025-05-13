using System.IO.Abstractions;
using System.IO.Compression;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Files.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and processing the structure of Unreal Engine plugin directories.
/// </summary>
[AutoConstructor]
public partial class PluginStructureService : IPluginStructureService {
  private const string Binaries = "Binaries";
  private const string Intermediate = "Intermediate";
  private const string IntermediateBuild = $"{Intermediate}/Build";

  private readonly IJsonService _jsonService;

  /// <inheritdoc />
  public List<string> GetInstalledBinaries(IDirectoryInfo pluginDirectory) {
    return pluginDirectory.EnumerateDirectories(Binaries, SearchOption.TopDirectoryOnly)
        .Concat(pluginDirectory.EnumerateDirectories(IntermediateBuild, SearchOption.TopDirectoryOnly))
        .Select(x => x.Name)
        .Distinct()
        .ToList();
  }

  /// <inheritdoc />
  public async Task<PluginSubmission> ExtractPluginSubmission(Stream archiveStream) {
    using var zipArchive = new ZipArchive(archiveStream, ZipArchiveMode.Read, true);

    var manifestFile = zipArchive.GetEntry("plugin.json");
    if (manifestFile is null) {
      throw new BadSubmissionException("Plugin manifest file was not found!");
    }

    PluginManifest manifest;
    await using (var stream = manifestFile.Open()) {
      manifest = await _jsonService.DeserializeAsync<PluginManifest>(stream);
    }

    var patches = await manifest.Patches
        .Select(x => (Name: x, Entry: zipArchive.GetEntry(Path.Join("patches", x))))
        .ToAsyncEnumerable()
        .SelectAwait(async x => {
          if (x.Entry is null) {
            throw new BadSubmissionException($"Missing patch file: {x.Name}");
          }

          await using var patchStream = x.Entry.Open();
          using var reader = new StreamReader(patchStream);
          return await reader.ReadToEndAsync();
        })
        .ToListAsync();

    var iconStream = zipArchive.GetEntry("icon.png")?.Open();

    string? readme;
    await using (var readmeFile = zipArchive.GetEntry("README.md")?.Open()) {
      if (readmeFile is not null) {
        using var streamReader = new StreamReader(readmeFile);
        readme = await streamReader.ReadToEndAsync();
      } else {
        readme = null;
      }
    }

    return new PluginSubmission(manifest, patches, iconStream, readme);
  }

  /// <inheritdoc />
  public async Task CompressPluginSubmission(PluginSubmission submission, Stream stream) {
    using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true);
    var pluginEntry = zipArchive.CreateEntry("plugin.json");
    await using (var entryStream = pluginEntry.Open()) {
      var manifestJson = _jsonService.Serialize(submission.Manifest);
      await using var jsonStream = manifestJson.ToStream();
      await jsonStream.CopyToAsync(entryStream);
    }

    if (submission.Manifest.Patches.Count > 0) {
      zipArchive.CreateEntry("patches/");
      for (var i = 0; i < submission.Manifest.Patches.Count; i++) {
        var patchEntry = zipArchive.CreateEntry($"patches/{submission.Manifest.Patches[i]}");
        await using var entryStream = patchEntry.Open();
        await using var patchStream = submission.Patches[i].ToStream();
        await patchStream.CopyToAsync(entryStream);
      }
    }

    if (submission.IconStream is not null) {
      var iconEntry = zipArchive.CreateEntry("icon.png");
      await using var entryStream = iconEntry.Open();
      await submission.IconStream.CopyToAsync(entryStream);
    }

    if (submission.ReadmeText is not null) {
      var readmeEntry = zipArchive.CreateEntry("readme.md");
      await using var entryStream = readmeEntry.Open();
      await using var readmeStream = submission.ReadmeText.ToStream();
      await readmeStream.CopyToAsync(entryStream);
    }
  }
}