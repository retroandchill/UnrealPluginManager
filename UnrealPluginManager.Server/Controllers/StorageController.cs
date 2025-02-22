using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Filters;

namespace UnrealPluginManager.Server.Controllers;

/// <summary>
/// Provides endpoints for managing and retrieving storage-related resources, such as files or icons.
/// </summary>
/// <remarks>
/// This controller is responsible for handling file-related requests and interacts with the storage service to process and retrieve the necessary data.
/// It applies the <see cref="ApiExceptionFilterAttribute"/> to handle server-side exceptions gracefully for all its endpoints.
/// </remarks>
[ApiController]
[ApiExceptionFilter]
[Route("/files")]
[AutoConstructor]
public partial class StorageController {
    private readonly IStorageService _storageService;

    /// <summary>
    /// Retrieves an icon as a stream for the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file representing the icon to be retrieved.</param>
    /// <returns>A stream containing the requested image data.</returns>
    [HttpGet("icons/{fileName}")]
    [Produces(MediaTypeNames.Image.Png)]
    public Stream GetIcon(string fileName) {
        return _storageService.RetrieveIcon(fileName);
    }
    
}