using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Server.Model;

public record PluginSubmissionMultipartRequest {

  [FromForm(Name = "manifest")]
  public required PluginManifest Manifest { get; init; }

  [FromForm(Name = "patches")]
  public List<string> Patches { get; init; } = [];

  [FromForm(Name = "icon")]
  public IFormFile? Icon { get; init; }

  [FromForm(Name = "readme")]
  public string? Readme { get; init; }

}