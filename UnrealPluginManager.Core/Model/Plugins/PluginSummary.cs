using System.ComponentModel.DataAnnotations;
using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

public record PluginSummary([Required] string Name, [Required] SemVersion Version, string? Description);