using Riok.Mapperly.Abstractions;
using UnrealPluginManager.Core.Database.Entities.Storage;
using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Mappers;

/// <summary>
/// Provides mapping functionality for converting <see cref="FileResource"/> entities
/// to <see cref="ResourceInfo"/> objects. This class is generated with mapping
/// methods that facilitate transformations between database entities
/// and domain models.
/// </summary>
/// <remarks>
/// This static mapper uses attributes from the `Mapperly` library to
/// generate mapping methods. It allows for seamless integration between
/// entity objects and application-level models while ensuring minimal manual mapping effort.
/// </remarks>
[Mapper(UseDeepCloning = false, RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ResourceMapper {

  /// <summary>
  /// Converts an instance of <see cref="FileResource"/> to a <see cref="ResourceInfo"/>.
  /// </summary>
  /// <param name="resource">
  /// An instance of <see cref="FileResource"/> representing the file resource data to be converted.
  /// </param>
  /// <returns>
  /// A new instance of <see cref="ResourceInfo"/> containing the converted data from the input <see cref="FileResource"/>.
  /// </returns>
  public static partial ResourceInfo ToResourceInfo(this FileResource resource);
  
}