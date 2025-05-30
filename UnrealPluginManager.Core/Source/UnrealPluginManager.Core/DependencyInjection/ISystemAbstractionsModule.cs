﻿using System.IO.Abstractions;
using Jab;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.DependencyInjection;

/// <summary>
/// Represents a module that integrates system abstractions into the dependency injection container.
/// </summary>
/// <remarks>
/// This service provider module facilitates dependency injection for various system abstraction
/// implementations including file system operations, environment variables, process execution,
/// and registry interaction.
/// </remarks>
/// <example>
/// The module registers the following implementations:
/// - <see cref="IFileSystem"/> mapped to <see cref="FileSystem"/>
/// - <see cref="IEnvironment"/> mapped to <see cref="SystemEnvironment"/>
/// - <see cref="IProcessRunner"/> mapped to <see cref="ProcessRunner"/>
/// - <see cref="IRegistry"/> mapped to platform-specific implementation
/// </example>
[ServiceProviderModule]
[Singleton<IFileSystem, FileSystem>]
[Singleton<IEnvironment, SystemEnvironment>]
[Singleton<IProcessRunner, ProcessRunner>]
[Singleton<IRegistry>(Factory = nameof(CreateRegistry))]
public interface ISystemAbstractionsModule {
  /// Creates and returns an instance of the IRegistry interface.
  /// The implementation depends on the operating system:
  /// - Returns a WindowsRegistry instance for Windows-based operating systems.
  /// - Returns an UnsupportedPlatformRegistry instance for unsupported platforms.
  /// <returns>
  /// An implementation of the IRegistry interface appropriate for the platform.
  /// </returns>
  static IRegistry CreateRegistry() {
    return OperatingSystem.IsWindows() ? new WindowsRegistry() : new UnsupportedPlatformRegistry();
  }
}