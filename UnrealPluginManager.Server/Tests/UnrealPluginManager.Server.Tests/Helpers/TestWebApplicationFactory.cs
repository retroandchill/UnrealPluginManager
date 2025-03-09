using System.IO.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Tests.Helpers;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {

  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    Type[] types = [
        typeof(UnrealPluginManagerContext),
        typeof(IFileSystem),
        typeof(IEnvironment),
        typeof(IProcessRunner),
        typeof(IRegistry)
    ];
    
    builder.ConfigureServices(services => {
      foreach (var service in types.SelectValid(t => services.Single(s => s.ServiceType == t))) {
        services.Remove(service);
      }

      services.SetUpMockDataProviders();
    });
    
    builder.UseEnvironment("Development");
  }
  
}