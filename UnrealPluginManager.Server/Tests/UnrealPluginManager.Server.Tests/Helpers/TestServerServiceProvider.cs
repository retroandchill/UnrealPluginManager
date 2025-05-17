using Jab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Tests.Helpers;
using UnrealPluginManager.Server.DependencyInjection;

namespace UnrealPluginManager.Server.Tests.Helpers;

[ServiceProvider]
[Import<IMockAbstractionsModule>]
[Import<IWebContextModule>]
[Import<IAuthModule>]
[Import<IServerIoModule>]
[Import<ICoreServicesModule>]
[Import<ITestCloudDatabaseModule>]
[Import<IKeycloakClientModule>]
[Singleton<ServiceProviderWrapper>(Instance = nameof(RuntimeServiceProvider))]
[JabCopyExclude(typeof(IConfiguration), typeof(IHostEnvironment), typeof(HttpClient), typeof(ILogger<>))]
public partial class TestServerServiceProvider(IServiceProvider runtimeServiceProvider) : IServerServiceProvider {
  
  private ServiceProviderWrapper RuntimeServiceProvider { get; } = new(runtimeServiceProvider);
  
  IServerServiceProvider.IScope IServerServiceProvider.CreateScope() {
    return CreateScope();
  }

  public partial class Scope : IServerServiceProvider.IScope;

}