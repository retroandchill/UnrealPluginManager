using System.IO.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.ApiGenerator.Swagger;
using UnrealPluginManager.ApiGenerator.Utils;
using UnrealPluginManager.Server.Tests;
using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.ApiGenerator.Tests.Application;

public class GenerateDocumentTest {
  [Test]
  public async Task TestGenerateDocument() {
    var app = WebApplication.CreateBuilder([])
        .SetUpCommonConfiguration()
        .SetUpMockDataProviders()
        .SetUpSwagger()
        .ConfigureEndpoints()
        .Build();

    app.Configure();
    await app.ProduceSwaggerDocument();

    var fileSystem = app.Services.GetRequiredService<IFileSystem>();
    Assert.That(fileSystem.FileInfo.New("openapi-spec.json").Exists, Is.True);
  }
}