using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Tests.Mocks;

namespace UnrealPluginManager.Core.Tests.Helpers;

public class MockAbstractionsFactory : ISystemAbstractionsFactory {

  public IFileSystem CreateFileSystem() {
    return new MockFileSystem();
  }

  public IEnvironment CreateEnvironment() {
    return new MockEnvironment();
  }

  public IProcessRunner CreateProcessRunner() {
    return new MockProcessRunner();
  }

  public IRegistry CreateRegistry() {
    return new MockRegistry();
  }
}