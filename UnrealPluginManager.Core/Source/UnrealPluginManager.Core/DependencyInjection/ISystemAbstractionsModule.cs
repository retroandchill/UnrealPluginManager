using System.IO.Abstractions;
using Jab;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(IFileSystem), Factory = nameof(CreateFileSystem))]
[Singleton(typeof(IEnvironment), Factory = nameof(CreateEnvironment))]
[Singleton(typeof(IProcessRunner), Factory = nameof(CreateProcessRunner))]
[Singleton(typeof(IRegistry), Factory = nameof(CreateRegistry))]
public interface ISystemAbstractionsModule {

  public static IFileSystem CreateFileSystem(ISystemAbstractionsFactory systemAbstractionsFactory) {
    return systemAbstractionsFactory.CreateFileSystem();
  }

  public static IEnvironment CreateEnvironment(ISystemAbstractionsFactory systemAbstractionsFactory) {
    return systemAbstractionsFactory.CreateEnvironment();
  }

  public static IProcessRunner CreateProcessRunner(ISystemAbstractionsFactory systemAbstractionsFactory) {
    return systemAbstractionsFactory.CreateProcessRunner();
  }

  public static IRegistry CreateRegistry(ISystemAbstractionsFactory systemAbstractionsFactory) {
    return systemAbstractionsFactory.CreateRegistry();
  }
}