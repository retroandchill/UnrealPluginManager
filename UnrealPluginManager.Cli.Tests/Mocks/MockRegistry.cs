using System.Runtime.Versioning;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Cli.Tests.Mocks;

[SupportedOSPlatform("windows")]
public class MockRegistry : IRegistry {
    public IRegistryKey LocalMachine { get; set; } = new MockRegistryKey();
    public IRegistryKey CurrentUser { get; set; } = new MockRegistryKey();
}