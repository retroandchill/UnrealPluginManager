using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Jab;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Tests.Mocks;

namespace UnrealPluginManager.Core.Tests.Helpers;

[ServiceProviderModule]
[Singleton(typeof(IFileSystem), typeof(MockFileSystem))]
[Singleton(typeof(IEnvironment), typeof(MockEnvironment))]
[Singleton(typeof(IProcessRunner), typeof(MockProcessRunner))]
[Singleton(typeof(IRegistry), typeof(MockRegistry))]
public interface IMockAbstractionsModule;