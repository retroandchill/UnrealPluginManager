using System.ComponentModel.DataAnnotations;

namespace UnrealPluginManager.Core.Model.Plugins;

public record PluginSummary([Required] string Name, string? Description);