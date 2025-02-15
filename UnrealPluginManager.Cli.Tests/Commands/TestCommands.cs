using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using NUnit.Framework.Internal.Execution;
using Semver;
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.Database;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Cli.Tests.Mocks;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Cli.Tests.Commands;

public class TestCommands {
    
    private MockFileSystem _filesystem;
    private Parser _parser;
    private Mock<IEnvironment> _environment;
    private Mock<IEngineService> _engineService;
    
    [SetUp]
    public void Setup() {
        _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var rootCommand = new RootCommand {
            new BuildCommand(),
            new InstallCommand(),
            new VersionsCommand()
        };
        
        _engineService = new Mock<IEngineService>();
        _environment = new Mock<IEnvironment>();
        var builder = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseExceptionHandler(errorExitCode: 1)
            .UseDependencyInjection(services => {
                services.AddSingleton<IFileSystem>(_filesystem);
                services.AddSingleton(_environment.Object);
                services.AddSingleton(_engineService.Object);
            });
        
        _parser = builder.Build();
    }

    [Test]
    public async Task TestRequestVersions() {
        var installedEngines = new List<InstalledEngine> {
            new("5.4", new Version(5, 4), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.4")),
            new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
            new("5.6_Custom", new Version(5, 6), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.6_Custom"), true),
        };
        _engineService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);


        var console = new RedirectingConsole();
        var returnCode = await _parser.InvokeAsync("versions", console);
        var consoleOutput = console.GetWrittenOut().Split(Environment.NewLine);
        Assert.Multiple(() =>
        {
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(consoleOutput, Has.Length.EqualTo(4));
        });
        Assert.Multiple(() =>
        {
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
        Assert.Multiple(() =>
        {
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(consoleOutput, Has.Length.EqualTo(4));
        });
        Assert.Multiple(() =>
        {
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
        _engineService.Verify(x => x.BuildPlugin(It.Is<IFileInfo>(y => y.FullName == Path.GetFullPath("C:/dev/MyPlugin/MyPlugin.uplugin")), 
            It.Is<string?>(y => y == "5.5")));
        
        returnCode = await _parser.InvokeAsync("build C:/dev/MyPlugin/MyPlugin.uplugin");
        Assert.That(returnCode, Is.EqualTo(0));
        _engineService.Verify(x => x.BuildPlugin(It.Is<IFileInfo>(y => y.FullName == Path.GetFullPath("C:/dev/MyPlugin/MyPlugin.uplugin")), 
            It.Is<string?>(y => y == null)));
    }
    
    [Test]
    public async Task TestInstallPlugin() {
        var returnCode = await _parser.InvokeAsync("install MyPlugin");
        Assert.That(returnCode, Is.EqualTo(0));
        _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"), 
            It.Is<SemVersionRange>(y => y == SemVersionRange.AllRelease),
            It.Is<string?>(y => y == null)));
        
        returnCode = await _parser.InvokeAsync("install MyPlugin --version 1.0.0");
        Assert.That(returnCode, Is.EqualTo(0));
        _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"), 
            It.Is<SemVersionRange>(y => y == SemVersionRange.Equals(new SemVersion(1, 0, 0))),
            It.Is<string?>(y => y == null)));
        
        returnCode = await _parser.InvokeAsync("install MyPlugin --version >=1.0.0 --engine-version 5.5");
        Assert.That(returnCode, Is.EqualTo(0));
        _engineService.Verify(x => x.InstallPlugin(It.Is<string>(y => y == "MyPlugin"), 
            It.Is<SemVersionRange>(y => y == SemVersionRange.AtLeast(new SemVersion(1, 0, 0), false)),
            It.Is<string?>(y => y == "5.5")));
    }
    
}