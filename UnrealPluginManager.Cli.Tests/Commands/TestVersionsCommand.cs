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
using UnrealPluginManager.Cli.Commands;
using UnrealPluginManager.Cli.Database;
using UnrealPluginManager.Cli.DependencyInjection;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Cli.Tests.Mocks;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Cli.Tests.Commands;

public class TestVersionsCommand {
    
    private MockFileSystem _filesystem;
    private Parser _parser;
    private Mock<IEnvironment> _environment;
    private Mock<IEngineService> _engineService;
    
    [SetUp]
    public void Setup() {
        _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var rootCommand = new RootCommand {
            new VersionsCommand()
        };
        
        _engineService = new Mock<IEngineService>();
        _environment = new Mock<IEnvironment>();
        var builder = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseExceptionHandler(errorExitCode: 1)
            .UseDependencyInjection(services => {
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
    }
    
}