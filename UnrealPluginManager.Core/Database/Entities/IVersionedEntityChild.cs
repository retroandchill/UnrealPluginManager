namespace UnrealPluginManager.Core.Database.Entities;

/// <summary>
/// Represents a child entity that is associated with a versioned parent entity.
/// </summary>
public interface IVersionedEntityChild {

    /// <summary>
    /// Gets the parent entity associated with the current entity instance.
    /// The parent entity is expected to implement the <see cref="IVersionedEntity"/> interface.
    /// </summary>
    IVersionedEntity Parent { get; }
    
}