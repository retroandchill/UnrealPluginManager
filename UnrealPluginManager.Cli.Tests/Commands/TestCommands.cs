using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Exceptions;
using UnrealPluginManager.Cli.Tests.Mocks;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Model.Engine;
using UnrealPluginManager.Local.Model.Installation;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Cli.Tests.Commands;

public class TestCommands {
  private MockFileSystem _filesystem;
  private Parser _parser;
  private Mock<IEnvironment> _environment;
  private Mock<IPluginService> _pluginService;
  private Mock<IPluginManagementService> _pluginManagementService;
  private Mock<IEngineService> _engineService;
  private Mock<IInstallService> _installService;

  [SetUp]
  public void Setup() {
    _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
    var rootCommand = new RootCommand {
        new BuildCommand(),
        new InstallCommand(),
        new VersionsCommand(),
        new SearchCommand()
    };

    _pluginService = new Mock<IPluginService>();
    _engineService = new Mock<IEngineService>();
    _pluginManagementService = new Mock<IPluginManagementService>();
    _installService = new Mock<IInstallService>();
    _environment = new Mock<IEnvironment>();
    var builder = new CommandLineBuilder(rootCommand)
        .UseDefaults()
        .UseCustomExceptionHandler()
        .UseDependencyInjection(services => {
          services.AddSingleton<IFileSystem>(_filesystem);
          services.AddSingleton(_environment.Object);
          services.AddSingleton(_engineService.Object);
          services.AddSingleton(_pluginService.Object);
          services.AddSingleton(_pluginManagementService.Object);
          services.AddSingleton(_installService.Object);
        });

    _parser = builder.Build();
  }

  [Test]
  public async Task TestRequestVersions() {
    var installedEngines = new List<InstalledEngine> {
        new("5.4", new Version(5, 4), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.4")),
        new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
        new("5.6_Custom", new Version(5, 6), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.6_Custom"),
            true),
    };
    _engineService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);


    var console = new RedirectingConsole();
    var returnCode = await _parser.InvokeAsync("versions", console);
    var consoleOutput = console.GetWrittenOut().Split(Environment.NewLine);
    Assert.Multiple(() => {
      Assert.That(returnCode, Is.EqualTo(0));
      Assert.That(consoleOutput, Has.Length.EqualTo(4));
    });
    Assert.Multiple(() => {
      Assert.That(consoleOutput[0], Is.EqualTo("- 5.4: Installed"));
      Assert.That(consoleOutput[1], Is.EqualTo("- 5.5: Installed *"));
      Assert.That(consoleOutput[2], Is.EqualTo("- 5.6_Custom: Custom Build"));
      Assert.That(consoleOutput[3], Is.Empty);
    });

    _environment.Setup(x => x.GetEnvironmentVariable(EnvironmentVariables.PrimaryUnrealEngineVersion))
        .Returns("5.6_Custom");
    console = new RedirectingConsole();
    returnCode = await _parser.InvokeAsync("versions", console);
    consoleOutput = console.GetWrittenOut().Split(Environment.NewLine);
    Assert.Multiple(() => {
      Assert.That(returnCode, Is.EqualTo(0));
      Assert.That(consoleOutput, Has.Length.EqualTo(4));
    });
    Assert.Multiple(() => {
      Assert.That(consoleOutput[0], Is.EqualTo("- 5.4: Installed"));
      Assert.That(consoleOutput[1], Is.EqualTo("- 5.5: Installed"));
      Assert.That(consoleOutput[2], Is.EqualTo("- 5.6_Custom: Custom Build *"));
      Assert.That(consoleOutput[3], Is.Empty);
    });
  }

  [Test]
  public async Task TestRequestBuild() {
    var returnCode = await _parser.InvokeAsync("build C:/dev/MyPlugin/MyPlugin.uplugin --version 5.5");
    Assert.That(returnCode, Is.EqualTo(0));
    _engineService.Verify(x =>
                              x.BuildPlugin(
                                  It.Is<IFileInfo>(y => y.FullName ==
                                                        Path.GetFullPath("C:/dev/MyPlugin/MyPlugin.uplugin")),
                                  It.Is<string?>(y => y == "5.5")));

    returnCode = await _parser.InvokeAsync("build C:/dev/MyPlugin/MyPlugin.uplugin");
    Assert.That(returnCode, Is.EqualTo(0));
    _engineService.Verify(x =>
                              x.BuildPlugin(
                                  It.Is<IFileInfo>(y => y.FullName ==
                                                        Path.GetFullPath("C:/dev/MyPlugin/MyPlugin.uplugin")),
                                  It.Is<string?>(y => y == null)));
  }

  [Test]
  public async Task TestInstallPlugin() {
    _installService.Setup(x => x.InstallPlugin(It.IsAny<string>(), It.IsAny<SemVersionRange>(), It.IsAny<string?>(),
                                               It.IsAny<IReadOnlyCollection<string>>()))
        .ReturnsAsync([new VersionChange("MyPlugin", null, new SemVersion(1, 0, 0))]);
    var returnCode = await _parser.InvokeAsync("install MyPlugin");
    Assert.That(returnCode, Is.EqualTo(0));
    _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"),
                                               It.Is<SemVersion>(y => y == new SemVersion(1, 0, 0)),
                                               It.Is<string?>(y => y == null),
                                               It.IsAny<IReadOnlyCollection<string>>()));

    returnCode = await _parser.InvokeAsync("install MyPlugin --version 1.0.0");
    Assert.That(returnCode, Is.EqualTo(0));
    _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"),
                                               It.Is<SemVersion>(y => y ==
                                                                           new SemVersion(1, 0, 0)),
                                               It.Is<string?>(y => y == null),
                                               It.IsAny<IReadOnlyCollection<string>>()));

    returnCode = await _parser.InvokeAsync("install MyPlugin --version >=1.0.0 --engine-version 5.5");
    Assert.That(returnCode, Is.EqualTo(0));
    _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"),
                                               It.Is<SemVersion>(y => y ==
                                                                      new SemVersion(1, 0, 0)),
                                               It.Is<string?>(y => y == "5.5"),
                                               It.IsAny<IReadOnlyCollection<string>>()));

    _engineService.Setup(x => x.GetInstalledPluginVersion("MyPlugin", "5.5"))
        .Returns(() => Task.FromResult<LanguageExt.Option<SemVersion>>(SemVersion.Parse("1.0.0")));
    _engineService.Invocations.Clear();
    returnCode = await _parser.InvokeAsync("install MyPlugin --version >=1.0.0 --engine-version 5.5");
    Assert.That(returnCode, Is.EqualTo(0));
    _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"),
                                               It.Is<SemVersion>(y => y ==
                                                                           new SemVersion(1, 0, 0)),
                                               It.Is<string?>(y => y == "5.5"),
                                               It.IsAny<IReadOnlyCollection<string>>()),
                          Times.Never);
  }

  [Test]
  public async Task TestSearch() {
    var versions = new SemVersion[] {
        new(1, 0, 0), new(1, 0, 1), new(1, 0, 2), new(1, 0, 3), new(1, 1, 0), new(1, 1, 1), new(1, 2, 0),
        new(2, 0, 0), new(2, 0, 1), new(3, 0, 0)
    };

    var count = 1;
    var plugins = Enumerable.Range(0, 100)
        .Select(i => new PluginOverview {
            Id = (ulong)i + 1,
            Name = $"TestPlugin{i}",
            Versions = versions
                .Take((i % 10) + 1)
                .Select(x => new VersionOverview {
                    Id = (ulong)count++,
                    Version = x,
                })
                .ToList()
        })
        .ToList();
    _pluginService.Setup(x => x.ListPlugins("*", default))
        .Returns(Task.FromResult(new Page<PluginOverview>(plugins)));

    var returnCode = await _parser.InvokeAsync("search *");
    Assert.That(returnCode, Is.EqualTo(0));
    _pluginService.Verify(x => x.ListPlugins("*", default));

    _pluginManagementService.Setup(x => x.GetPlugins("default", "*"))
        .Returns(Task.FromResult(plugins));
    returnCode = await _parser.InvokeAsync("search * --remote default");
    Assert.That(returnCode, Is.EqualTo(0));
    _pluginManagementService.Verify(x => x.GetPlugins("default", "*"));

    var split = plugins
        .Select((x, i) => new {
            Index = i,
            Value = x
        })
        .GroupBy(x => x.Index / 10)
        .Select((x, i) => new KeyValuePair<string, Fin<List<PluginOverview>>>(
                    $"group{i}",
                    i != 3 ? x.Select(y => y.Value).ToList() : Fin<List<PluginOverview>>.Fail("Error")))
        .ToOrderedDictionary();
    _pluginManagementService.Setup(x => x.GetPlugins("*"))
        .Returns(Task.FromResult(split));
    returnCode = await _parser.InvokeAsync("search * --remote all");
    Assert.That(returnCode, Is.EqualTo(0));
    _pluginManagementService.Verify(x => x.GetPlugins("*"));

    var allErrors = Enumerable.Range(0, 100)
        .Select(i => new KeyValuePair<string, Fin<List<PluginOverview>>>(
                    $"group{i}",
                    Fin<List<PluginOverview>>.Fail("Error")))
        .ToOrderedDictionary();
    _pluginManagementService.Setup(x => x.GetPlugins("*"))
        .Returns(Task.FromResult(allErrors));

    returnCode = await _parser.InvokeAsync("search * --remote all");
    Assert.That(returnCode, Is.EqualTo(-1));
    _pluginManagementService.Verify(x => x.GetPlugins("*"));
  }
}