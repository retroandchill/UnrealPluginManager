using Jab;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Server.Database;

namespace UnrealPluginManager.Server.Tests.Helpers;

[ServiceProviderModule]
[Singleton<TestCloudUnrealPluginManagerContext>]
[Singleton<CloudUnrealPluginManagerContext>(Factory = nameof(GetDatabaseContext))]
[Singleton<UnrealPluginManagerContext>(Factory = nameof(GetDatabaseContext))]
public interface ITestCloudDatabaseModule {

  static TestCloudUnrealPluginManagerContext GetDatabaseContext(TestCloudUnrealPluginManagerContext context) {
    return context;
  }
  
}