using System.Runtime.Versioning;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.Tests.Mocks;

/// <summary>
/// Provides a mock implementation of the <see cref="IRegistry"/> interface for unit testing purposes.
/// </summary>
/// <remarks>
/// This class simulates the behavior of the Windows registry, allowing tests to interact with a virtual registry
/// instead of the actual system registry. It is designed to work with Windows and adheres to the <see cref="SupportedOSPlatformAttribute"/> for "windows".
/// </remarks>
[SupportedOSPlatform("windows")]
public class MockRegistry : IRegistry {

    /// <inheritdoc />
    public IRegistryKey LocalMachine { get; set; } = new MockRegistryKey();
    
    /// <inheritdoc />
    public IRegistryKey CurrentUser { get; set; } = new MockRegistryKey();
}