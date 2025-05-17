using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Jab;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Tests.Mocks;

namespace UnrealPluginManager.Core.Tests.Helpers;

[ServiceProviderModule]
[Singleton<IFileSystem, MockFileSystem>]
[Singleton<IEnvironment, MockEnvironment>]
[Singleton<IProcessRunner, MockProcessRunner>]
[Singleton<IRegistry, MockRegistry>]
public interface IMockAbstractionsModule;