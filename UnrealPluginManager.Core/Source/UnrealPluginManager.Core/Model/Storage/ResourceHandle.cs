using System.IO.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents a handle for a resource, consisting of a file reference and a resource name.
/// </summary>
/// <remarks>
/// This structure is used to uniquely identify and describe resources managed within the system.
/// It combines a reference to the file (via IFileInfo) with a descriptive name for the resource to make it easily identifiable.
/// </remarks>
/// <param name="File">The file info for the resource</param>
/// <param name="ResourceName">The base name used to lookup the resource <see cref="IStorageService"/></param>
public record struct ResourceHandle(string ResourceName, IFileInfo File);