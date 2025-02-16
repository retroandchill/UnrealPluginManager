using System.IO.Abstractions;
using Riok.Mapperly.Abstractions;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Mappers;

/// <summary>
/// A static partial class responsible for mapping between
/// data transfer objects (DTOs) and database entities related to plugins.
/// Provides mappings to transform PluginDescriptor and PluginReferenceDescriptor
/// objects into corresponding Plugin, PluginVersion, and Dependency entities.
/// Utilizes Riok.Mapperly library for advanced mapping configurations.
/// </summary>
[Mapper(UseDeepCloning = true, RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class PluginMapper {

    /// <summary>
    /// Maps a <see cref="PluginDescriptor"/> object to a <see cref="Plugin"/> object using the specified plugin name.
    /// </summary>
    /// <param name="descriptor">The <see cref="PluginDescriptor"/> object containing source data.</param>
    /// <param name="name">The name of the plugin to assign in the created <see cref="Plugin"/> instance.</param>
    /// <returns>A new <see cref="Plugin"/> object mapped from the provided <see cref="PluginDescriptor"/> and name.</returns>
    [MapperIgnoreTarget(nameof(Plugin.Id))]
    [MapperIgnoreTarget(nameof(Plugin.Versions))]
    [MapProperty(nameof(PluginDescriptor.CreatedBy), nameof(Plugin.AuthorName))]
    [MapProperty(nameof(PluginDescriptor.CreatedByUrl), nameof(Plugin.AuthorWebsite))]
    public static partial Plugin ToPlugin(this PluginDescriptor descriptor, string name);

    /// <summary>
    /// Maps a <see cref="PluginDescriptor"/> object to a <see cref="PluginVersion"/> object.
    /// </summary>
    /// <param name="descriptor">The <see cref="PluginDescriptor"/> object containing source data to be mapped.</param>
    /// <returns>A new <see cref="PluginVersion"/> object populated with data from the provided <see cref="PluginDescriptor"/> object.</returns>
    [MapperIgnoreTarget(nameof(PluginVersion.Id))]
    [MapperIgnoreTarget(nameof(PluginVersion.VersionString))]
    [MapperIgnoreTarget(nameof(PluginVersion.Parent))]
    [MapperIgnoreTarget(nameof(PluginVersion.ParentId))]
    [MapperIgnoreTarget(nameof(PluginVersion.VersionString))]
    [MapperIgnoreTarget(nameof(PluginVersion.Binaries))]
    [MapProperty(nameof(PluginDescriptor.VersionName), nameof(PluginVersion.Version))]
    [MapProperty(nameof(PluginDescriptor.Plugins), nameof(PluginVersion.Dependencies))]
    public static partial PluginVersion ToPluginVersion(this PluginDescriptor descriptor);

    /// <summary>
    /// Maps a <see cref="PluginReferenceDescriptor"/> object to a <see cref="Dependency"/> object with the specified properties transformed accordingly.
    /// </summary>
    /// <param name="descriptor">The <see cref="PluginReferenceDescriptor"/> object containing source data for mapping.</param>
    /// <returns>A new <see cref="Dependency"/> object mapped from the provided <see cref="PluginReferenceDescriptor"/> instance.</returns>
    [MapperIgnoreTarget(nameof(Dependency.Id))]
    [MapperIgnoreTarget(nameof(Dependency.Parent))]
    [MapperIgnoreTarget(nameof(Dependency.ParentId))]
    [MapProperty(nameof(PluginReferenceDescriptor.Name), nameof(Dependency.PluginName))]
    [MapProperty(nameof(PluginReferenceDescriptor.PluginType), nameof(Dependency.Type))]
    [MapProperty(nameof(PluginReferenceDescriptor.VersionMatcher), nameof(Dependency.PluginVersion))]
    public static partial Dependency ToDependency(this PluginReferenceDescriptor descriptor);

    /// <summary>
    /// Maps an <see cref="EngineFileData"/> object to a <see cref="PluginBinaries"/> object.
    /// </summary>
    /// <param name="descriptor">The <see cref="EngineFileData"/> object containing the source data for mapping.</param>
    /// <returns>A new <see cref="PluginBinaries"/> object created based on the provided <see cref="EngineFileData"/>.</returns>
    [MapperIgnoreTarget(nameof(PluginBinaries.Id))]
    [MapperIgnoreTarget(nameof(PluginBinaries.Parent))]
    [MapperIgnoreTarget(nameof(PluginBinaries.ParentId))]
    [MapProperty(nameof(EngineFileData.FileInfo), nameof(PluginBinaries.FilePath))]
    public static partial PluginBinaries ToPluginBinaries(this EngineFileData descriptor);

    /// <summary>
    /// Converts an <see cref="EngineFileData"/> object into a collection of <see cref="PluginBinaries"/> objects.
    /// </summary>
    /// <param name="descriptor">The <see cref="EngineFileData"/> object containing file data to be transformed into a collection of <see cref="PluginBinaries"/> instances.</param>
    /// <returns>A collection of <see cref="PluginBinaries"/> objects corresponding to the transformed file data represented by the provided <see cref="EngineFileData"/>.</returns>
    public static ICollection<PluginBinaries> ToPluginBinaries(this EngineFileData? descriptor) {
        return descriptor.AsEnumerable()
            .Select(ToPluginBinaries)
            .ToList();
    }

    /// <summary>
    /// Returns a <see cref="SemVersion"/> object without any modifications.
    /// </summary>
    /// <param name="version">The <see cref="SemVersion"/> object to be returned.</param>
    /// <returns>The same <see cref="SemVersion"/> object provided as input.</returns>
    private static SemVersion PassSemVersion(SemVersion version) => version;

    /// <summary>
    /// Passes through the provided <see cref="SemVersionRange"/> object without modification.
    /// </summary>
    /// <param name="version">The <see cref="SemVersionRange"/> object to be passed through.</param>
    /// <returns>The same <see cref="SemVersionRange"/> object that was provided as input.</returns>
    private static SemVersionRange PassSemVersionRange(SemVersionRange version) => version;

    /// <summary>
    /// Passes through the provided <see cref="IFileInfo"/> object.
    /// </summary>
    /// <param name="fileInfo">The <see cref="IFileInfo"/> object to be passed through unmodified.</param>
    /// <returns>The same <see cref="IFileInfo"/> object that was passed as input.</returns>
    private static IFileInfo PassFileInfo(IFileInfo fileInfo) => fileInfo;
}