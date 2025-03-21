﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

/// <summary>
/// Represents a dependency associated with a plugin.
/// </summary>
/// <remarks>
/// A Dependency defines the relationship between a parent plugin and another plugin it depends on,
/// including information about the dependent plugin's name, version range, type, and whether it is optional.
/// </remarks>
public class Dependency : IVersionedEntityChild {
  /// <summary>
  /// Gets or sets the unique identifier for the dependency.
  /// </summary>
  /// <remarks>
  /// The <c>Id</c> property serves as the primary key for the <c>Dependency</c> entity
  /// and is automatically generated by the database.
  /// </remarks>
  [Key]
  public Guid Id { get; set; } = Guid.CreateVersion7();

  /// <summary>
  /// Gets or sets the unique identifier for the parent plugin associated with the dependency.
  /// </summary>
  /// <remarks>
  /// The <c>ParentId</c> property defines the foreign key relationship between the <c>Dependency</c>
  /// entity and the parent <c>Plugin</c> entity. It is used to associate a dependency with its parent plugin.
  /// </remarks>
  public Guid ParentId { get; set; }

  /// <summary>
  /// Gets or sets the parent plugin for the dependency.
  /// </summary>
  /// <remarks>
  /// The Parent property establishes a navigation reference to the plugin that owns this dependency.
  /// It represents the relationship where a plugin depends on other plugins, facilitating
  /// the association of specific dependencies with their respective parent plugin.
  /// </remarks>
  public PluginVersion Parent { get; set; }

  IVersionedEntity IVersionedEntityChild.Parent => Parent;

  /// <summary>
  /// Gets or sets the name of the plugin.
  /// </summary>
  /// <remarks>
  /// The <c>PluginName</c> property represents a unique name identifier for the plugin.
  /// The name must begin with an uppercase letter and contain only alphanumeric characters.
  /// Whitespace or special characters are not allowed.
  /// The property is required and must have a length between 1 and 255 characters.
  /// </remarks>
  [Required]
  [MinLength(1)]
  [MaxLength(255)]
  [RegularExpression(@"^[A-Z][a-zA-Z0-9]+$", ErrorMessage = "Whitespace is not allowed.")]
  public required string PluginName { get; set; }

  /// <summary>
  /// Gets or sets the version requirement for the plugin dependency.
  /// </summary>
  /// <remarks>
  /// The <c>PluginVersion</c> property defines the version range or specific version of a plugin
  /// that this dependency supports or requires. It uses <c>SemVersionRange</c> for flexible
  /// semantic versioning constraints, allowing for compatibility checks between plugins.
  /// </remarks>
  [MinLength(1)]
  [MaxLength(255)]
  public SemVersionRange PluginVersion { get; set; } = SemVersionRange.All;

  /// <summary>
  /// Gets or sets a value indicating whether the dependency is optional.
  /// </summary>
  /// <remarks>
  /// The <c>Optional</c> property specifies if the dependency is not strictly required
  /// for the parent plugin's functionality. An optional dependency allows the parent plugin
  /// to function correctly even when the dependent plugin is not installed.
  /// </remarks>
  public bool Optional { get; set; }

  /// <summary>
  /// Gets or sets the type of the plugin dependency.
  /// </summary>
  /// <remarks>
  /// The <c>Type</c> property determines the nature of the plugin dependency,
  /// represented by the <c>PluginType</c> enumeration. It indicates whether
  /// the dependency is part of the engine, provided along with the plugin,
  /// or externally sourced.
  /// </remarks>
  public PluginType Type { get; set; } = PluginType.Provided;

  internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Dependency>()
        .HasOne(x => x.Parent)
        .WithMany(x => x.Dependencies)
        .HasForeignKey(x => x.ParentId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Dependency>()
        .HasIndex(x => x.ParentId);

    modelBuilder.Entity<Dependency>()
        .Property(x => x.PluginVersion)
        .HasConversion(
            x => x.ToString(),
            x => SemVersionRange.Parse(x, 2048));
  }
}