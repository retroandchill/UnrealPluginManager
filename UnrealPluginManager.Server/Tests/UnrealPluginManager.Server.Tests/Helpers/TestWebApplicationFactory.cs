using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Tests.Helpers;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.DependencyInjection;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {

  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.ConfigureTestServices(services => {
      services.RemoveAll<ServerServiceProvider>();

      services.AddDbContext<TestCloudUnrealPluginManagerContext>(ServiceLifetime.Singleton, ServiceLifetime.Singleton)
          .AddSingleton(p => p.GetRequiredService<TestCloudUnrealPluginManagerContext>()
              .DefferDeletion());

      services.AddSingleton(p => new ServerServiceProvider(p, new MockAbstractionsFactory(),
          new DelegatingDatabaseFactory<CloudUnrealPluginManagerContext>(_ =>
              p.GetRequiredService<TestCloudUnrealPluginManagerContext>())));

      services.AddAuthentication("Test")
          .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", null);
    });

    builder.UseEnvironment("Development");
  }

}