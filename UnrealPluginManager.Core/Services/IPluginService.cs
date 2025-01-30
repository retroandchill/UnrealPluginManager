using LanguageExt;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Services;

public interface IPluginService {

    IEnumerable<PluginSummary> GetPluginSummaries();

    IEnumerable<PluginSummary> GetDependencyList(string pluginName);
}