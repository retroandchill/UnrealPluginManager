using Jab;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.DependencyInjection;

[ServiceProvider]
[Import(typeof(ISystemAbstractionsModule))]
[Singleton(typeof(IStorageService), typeof(CloudStorageService))]
[Scoped(typeof(IPluginStructureService), typeof(PluginStructureService))]
[Scoped(typeof(IPluginService), typeof(PluginService))]
public partial class ServerServiceProvider {
}