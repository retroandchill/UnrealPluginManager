using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Tests.Mocks;

namespace UnrealPluginManager.Core.Tests;

public static class CoreTestHelpers {
  
  public static IServiceCollection AddMockSystemAbstractions(this IServiceCollection services) {
    services.AddSingleton<IFileSystem, MockFileSystem>()
        .AddSingleton<IEnvironment, MockEnvironment>()
        .AddSingleton<IProcessRunner, MockProcessRunner>();

    if (OperatingSystem.IsWindows()) {
      services.AddSingleton<IRegistry, MockRegistry>();
    }

    return services;
  }
  
}