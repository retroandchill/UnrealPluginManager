namespace UnrealPluginManager.Core.Meta;

/// <summary>
/// An attribute used to mark properties or fields where the default value should be treated as an empty collection.
/// This is used for generating OpenAPI documentation to indicate a default type.
/// </summary>
/// <remarks>
/// Applicable to properties or fields of collection types. When applied, this attribute indicates that the collection
/// should default to an empty instance rather than null.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DefaultAsEmptyAttribute : Attribute;