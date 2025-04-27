using System.IO.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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

    builder.ConfigureTestServices(services => {
      foreach (var service in types.SelectValid(t => services.Single(s => s.ServiceType == t))) {
        services.Remove(service);
      }

      services.SetUpMockDataProviders();

      services.AddAuthentication("Test")
          .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", null);
    });

    builder.UseEnvironment("Development");
  }

}