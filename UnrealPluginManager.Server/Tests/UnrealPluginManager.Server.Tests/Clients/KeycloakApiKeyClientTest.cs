using System.Net;
using System.Text.Json;
using Keycloak.AuthServices.Sdk;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Clients;

namespace UnrealPluginManager.Server.Tests.Clients;

public class KeycloakApiKeyClientTest {
  private static readonly JsonSerializerOptions JsonOptions = new() {
      AllowTrailingCommas = true
  };

  private ServiceProvider _serviceProvider;
  private IKeycloakApiKeyClient _client;
  private MockHttpMessageHandler _mockHttpMessageHandler;
  private IJsonService _jsonService;

  private const string BaseUrl = "https://test-keycloak.com";


  [SetUp]
  public void OneTimeSetUp() {
    var services = new ServiceCollection();
    _mockHttpMessageHandler = new MockHttpMessageHandler();


    // Register real JSON service
    services.AddSingleton<IJsonService>(new JsonService(JsonOptions));

    // Configure HTTP client
    services.AddHttpClient<IKeycloakApiKeyClient, KeycloakApiKeyClient>(client => {
      client.BaseAddress = new Uri(BaseUrl);
    }).ConfigurePrimaryHttpMessageHandler(() => _mockHttpMessageHandler);


    _serviceProvider = services.BuildServiceProvider();
    _jsonService = _serviceProvider.GetRequiredService<IJsonService>();
    _client = _serviceProvider.GetRequiredService<IKeycloakApiKeyClient>();
  }

  [TearDown]
  public void TearDown() {
    _mockHttpMessageHandler.Dispose();
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task CheckApiKey_ValidKey_ReturnsGuid() {
    // Arrange
    var expectedGuid = Guid.CreateVersion7();
    const string realm = "test-realm";
    const string apiKey = "test-api-key";

    _mockHttpMessageHandler.When(HttpMethod.Get, $"{BaseUrl}/realms/{realm}/api-keys")
        .WithHeaders("ApiKey", apiKey)
        .Respond("application/json", _jsonService.Serialize(expectedGuid));


    // Act
    var result = await _client.CheckApiKey(realm, apiKey);

    // Assert
    Assert.That(result, Is.Not.EqualTo(Guid.Empty));
  }

  [Test]
  public async Task CreateNewApiKey_ValidData_ReturnsCreatedApiKey() {
    // Arrange
    const string realm = "test-realm";
    const string username = "test-user";
    var expireOn = DateTimeOffset.UtcNow.AddDays(30);
    var expectedResponse = new CreatedApiKey {
        ApiKey = "generated-api-key",
        Id = Guid.NewGuid(),
        ExpiresOn = expireOn
    };

    _mockHttpMessageHandler.When(HttpMethod.Post, $"{BaseUrl}/realms/{realm}/api-keys")
        .WithJsonContent(new {
            Username = username,
            ExpiresOn = expireOn.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK")
        }, JsonOptions)
        .Respond("application/json", _jsonService.Serialize(expectedResponse));

    // Act
    var result = await _client.CreateNewApiKey(realm, username, expireOn);

    // Assert
    Assert.Multiple(() => {
      Assert.That(result, Is.Not.Null);
      Assert.That(result.ApiKey, Is.Not.Empty);
      Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    });
  }

  [Test]
  public void CreateNewApiKey_ApiReturnsError_ThrowsException() {
    // Arrange
    const string realm = "test-realm";
    const string username = "test-user";
    var expireOn = DateTimeOffset.UtcNow.AddDays(30);

    _mockHttpMessageHandler.When(HttpMethod.Post, $"{BaseUrl}/realms/{realm}/api-keys")
        .Respond(_ => new HttpResponseMessage {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("")
        });

    // Act & Assert
    Assert.ThrowsAsync<KeycloakHttpClientException>(async () =>
        await _client.CreateNewApiKey(realm, username, expireOn));
  }


}