using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents a stored binary file associated with a specific Unreal Engine version and platform.
/// </summary>
/// <remarks>
/// This structure is primarily used to encapsulate metadata about plugin binaries, including:
/// - The version of the Unreal Engine the binary targets.
/// - The platform for which the binary is built (e.g., Windows, Linux).
/// - A reference to the file containing the binary.
/// </remarks>
/// <param name="EngineVersion">
/// The version of the Unreal Engine targeted by the binary.
/// </param>
/// <param name="Platform">
/// The platform for which the binary is built (e.g., Windows, Linux).
/// </param>
/// <param name="File">
/// A reference to the file containing the binary, represented as an abstraction that supports file operations.
/// </param>
public record struct StoredBinary(string EngineVersion, string Platform, IFileInfo File);