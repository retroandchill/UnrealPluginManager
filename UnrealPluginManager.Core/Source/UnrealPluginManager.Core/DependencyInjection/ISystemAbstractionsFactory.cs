using System.IO.Abstractions;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.DependencyInjection;

/// <summary>
/// Provides a factory interface for creating system-level abstractions.
/// </summary>
/// <remarks>
/// This interface defines methods for instantiating the core abstractions required for system operations,
/// including file system interactions, environment variable management, process execution, and registry access.
/// Implementations of this interface may provide concrete or mocked versions of these abstractions, depending on the context.
/// </remarks>
public interface ISystemAbstractionsFactory {

  /// <summary>
  /// Creates and returns an instance of <see cref="IFileSystem"/>.
  /// </summary>
  /// <remarks>
  /// This method provides an abstraction over the file system, allowing for system-level operations
  /// such as file and directory management. The implementation returned can vary, such as a real file system
  /// or a mocked one, depending on the context (e.g., production or testing).
  /// </remarks>
  /// <returns>
  /// An object implementing the <see cref="IFileSystem"/> interface, representing the file system abstraction.
  /// </returns>
  IFileSystem CreateFileSystem();

  /// <summary>
  /// Creates and returns an instance of <see cref="IEnvironment"/>.
  /// </summary>
  /// <remarks>
  /// This method constructs an abstraction for interacting with environment variables and special folder paths.
  /// It can be used to manage environment-specific variables and retrieve system-defined folder paths.
  /// The returned implementation may vary based on the context, such as production use cases or test scenarios.
  /// </remarks>
  /// <returns>
  /// An object implementing the <see cref="IEnvironment"/> interface, providing methods for environment interaction.
  /// </returns>
  IEnvironment CreateEnvironment();

  /// <summary>
  /// Creates and returns an instance of <see cref="IProcessRunner"/>.
  /// </summary>
  /// <remarks>
  /// This method provides an abstraction for executing external processes, enabling the execution of commands
  /// with specified arguments and an optional working directory. The implementation returned can vary,
  /// such as a real process runner or a mocked version, based on the context (e.g., production or testing).
  /// </remarks>
  /// <returns>
  /// An object implementing the <see cref="IProcessRunner"/> interface, representing the process execution abstraction.
  /// </returns>
  IProcessRunner CreateProcessRunner();

  /// <summary>
  /// Creates and returns an instance of <see cref="IRegistry"/>.
  /// </summary>
  /// <remarks>
  /// This method provides an abstraction for accessing and managing Windows registry keys.
  /// The implementation returned may vary depending on the platform or context, providing a concrete registry access on Windows or a fallback implementation for unsupported platforms.
  /// </remarks>
  /// <returns>
  /// An object implementing the <see cref="IRegistry"/> interface, representing the abstraction for registry operations.
  /// </returns>
  IRegistry CreateRegistry();

}