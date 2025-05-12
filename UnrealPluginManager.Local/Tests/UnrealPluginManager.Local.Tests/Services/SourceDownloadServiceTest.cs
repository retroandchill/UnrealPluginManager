using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Local.Tests.Services;

public class SourceDownloadServiceTests {
  private ServiceProvider _serviceProvider;
  private MockFileSystem _fileSystem;
  private Mock<IProcessRunner> _processRunner;
  private Mock<HttpMessageHandler> _mockHttpHandler;
  private HttpClient _httpClient;
  private ISourceDownloadService _service;
  private const string TestContent = "Test content";
  private readonly string _testContentHash;


  public SourceDownloadServiceTests() {
    // Pre-compute the hash of our test content
    var bytes = Encoding.UTF8.GetBytes(TestContent);
    _testContentHash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
  }


  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();
    _fileSystem = new MockFileSystem();
    services.AddSingleton<IFileSystem>(_fileSystem);
    _processRunner = new Mock<IProcessRunner>();
    services.AddSingleton(_processRunner.Object);
    _mockHttpHandler = new Mock<HttpMessageHandler>();
    _httpClient = new HttpClient(_mockHttpHandler.Object);
    services.AddSingleton(_httpClient);

    services.AddSingleton<ISourceDownloadService, SourceDownloadService>();

    _serviceProvider = services.BuildServiceProvider();
    _service = _serviceProvider.GetRequiredService<ISourceDownloadService>();
  }

  [TearDown]
  public void TearDown() {
    _httpClient.Dispose();
    _serviceProvider.Dispose();
  }


  [Test]
  public async Task DownloadAndExtractSources_ValidSource_ExtractsFiles() {
    var directory = _fileSystem.DirectoryInfo.New("/test/output");

    // Create a test zip file in memory
    var zipContent = CreateTestZipFile(TestContent);

    // Arrange
    var sourceLocation = new SourceLocation {
        Url = new Uri("http://test.com/plugin.zip"),
        Sha = Convert.ToHexString(SHA256.HashData(zipContent)).ToLowerInvariant()
    };

    _mockHttpHandler.Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage {
            StatusCode = HttpStatusCode.OK,
            Content = new ByteArrayContent(zipContent)
        });

    // Act
    await _service.DownloadAndExtractSources(sourceLocation, directory);
    directory.Refresh();

    // Assert
    Assert.That(directory.Exists, Is.True);
    Assert.That(_fileSystem.Directory.Exists(directory.FullName), Is.True);
  }

  [Test]
  public async Task VerifySourceHash_ValidHash_Succeeds() {
    // Arrange
    var content = Array.Empty<byte>();
    var expectedHash = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();

    _fileSystem.Directory.CreateDirectory("/test");
    var file = _fileSystem.FileInfo.New("/test/file.zip");
    await _fileSystem.File.WriteAllBytesAsync(file.FullName, content);

    // Act & Assert
    Assert.DoesNotThrowAsync(() => _service.VerifySourceHash(file, expectedHash));
  }

  [Test]
  public void VerifySourceHash_InvalidHash_ThrowsBadSubmissionException() {
    // Arrange
    var content = Array.Empty<byte>();
    var incorrectHash = "incorrect_hash";

    _fileSystem.Directory.CreateDirectory("/test");
    var file = _fileSystem.FileInfo.New("/test/file.zip");
    _fileSystem.File.WriteAllBytes(file.FullName, content);

    // Act & Assert
    Assert.ThrowsAsync<BadSubmissionException>(() =>
        _service.VerifySourceHash(file, incorrectHash));
  }

  [Test]
  public async Task PatchSources_ValidPatches_AppliesSuccessfully() {
    // Arrange
    var directory = _fileSystem.DirectoryInfo.New("/test/plugin");
    var patches = new List<string> {
        "patch content"
    };

    _processRunner
        .Setup(x => x.RunProcess("git", It.IsAny<string[]>(), It.IsAny<string>()))
        .ReturnsAsync(0);

    // Act
    await _service.PatchSources(directory, patches);

    // Assert
    _processRunner.Verify(x =>
            x.RunProcess("git", It.IsAny<string[]>(), directory.FullName),
        Times.Once);
  }

  [Test]
  public void PatchSources_FailedPatch_ThrowsBadSubmissionException() {
    // Arrange
    var directory = _fileSystem.DirectoryInfo.New("/test/plugin");
    var patches = new List<string> {
        "patch content"
    };

    _processRunner
        .Setup(x => x.RunProcess("git", It.IsAny<string[]>(), It.IsAny<string>()))
        .ReturnsAsync(1);

    // Act & Assert
    Assert.ThrowsAsync<BadSubmissionException>(() =>
        _service.PatchSources(directory, patches));
  }

  private static byte[] CreateTestZipFile(string content) {
    using var memoryStream = new MemoryStream();
    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
      var testFile = archive.CreateEntry("test.txt");
      using var writer = new StreamWriter(testFile.Open(), Encoding.UTF8);
      writer.Write(content); // Don't use WriteLine to avoid line ending issues
    }
    return memoryStream.ToArray();
  }
}