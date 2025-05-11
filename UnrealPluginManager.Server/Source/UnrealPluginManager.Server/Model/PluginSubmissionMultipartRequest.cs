using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Server.Model;

public record PluginSubmissionMultipartRequest {

  [FromForm]
  public required PluginManifest Manifest { get; init; }

  [FromForm]
  public List<string> Patches { get; init; } = [];

  [FromForm]
  public IFormFile? Icon { get; init; }

  [FromForm]
  public string? Readme { get; init; }

}