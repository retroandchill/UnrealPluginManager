using Microsoft.AspNetCore.Builder;
using UnrealPluginManager.Core.Tests.Helpers;

namespace UnrealPluginManager.Server.Tests;

/// <summary>
/// Provides helper methods for configuring server-side setups specifically for use in testing environments.
/// This class extends functionality for setting up mock data providers within a WebApplicationBuilder instance.
/// </summary>
public static class ServerSetupHelpers {
    /// <summary>
    /// Configures the WebApplicationBuilder instance with mock data providers for testing purposes.
    /// This extension method sets up necessary services and dependencies to provide mock implementations
    /// required for application testing in a controlled environment.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to be configured.</param>
    /// <returns>The configured WebApplicationBuilder instance with mock data providers set up.</returns>
    public static WebApplicationBuilder SetUpMockDataProviders(this WebApplicationBuilder builder) {
        builder.Services.SetUpMockDataProviders();
        return builder;
    }
}