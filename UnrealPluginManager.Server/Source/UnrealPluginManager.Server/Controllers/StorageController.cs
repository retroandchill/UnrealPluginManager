using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Controllers;

/// <summary>
/// Provides endpoints for managing and retrieving storage-related resources, such as files or icons.
/// </summary>
/// <remarks>
/// This controller is responsible for handling file-related requests and interacts with the storage service to process and retrieve the necessary data.
/// </remarks>
[ApiController]
[Route("/files")]
[AutoConstructor]
public partial class StorageController {
  private readonly IStorageService _storageService;

  /// <summary>
  /// Retrieves an icon as a stream for the specified file name.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to search for</param>
  /// <returns>A stream containing the requested image data.</returns>
  [HttpGet("icons/{pluginName}")]
  [Produces(MediaTypeNames.Image.Png)]
  public Stream GetIcon(string pluginName) {
    return _storageService.RetrievePluginIcon(pluginName)
        .OrElseThrow(() => new FileNotFoundException($"Icon for plugin {pluginName} not found."))
        .OpenRead();
  }
}