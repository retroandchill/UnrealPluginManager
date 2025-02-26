using System.IO.Abstractions;
using System.Numerics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Semver;

namespace UnrealPluginManager.Core.Database;

/// <summary>
/// Provides extension methods for configuring database models with specific property mappings and behavior.
/// </summary>
public static class DbModelExtensions {

    /// <summary>
    /// Configures the <see cref="SemVersion"/> type for a complex property in Entity Framework Core.
    /// This method maps individual properties of the <see cref="SemVersion"/> object to database columns
    /// and specifies conversion for numerical properties to ensure compatibility with database types.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="ComplexPropertyBuilder{SemVersion}"/> to configure the <see cref="SemVersion"/> mapping.
    /// </param>
    /// <returns>
    /// Returns the configured <see cref="ComplexPropertyBuilder{SemVersion}"/> to allow further chaining of configuration.
    /// </returns>
    public static OwnedNavigationBuilder<T,SemVersion> ConfigureSemVersion<T>(
        this OwnedNavigationBuilder<T, SemVersion> builder) where T : class {
        builder.Property(e => e.Major)
            .HasConversion(x => (long) x, x => new BigInteger(x));
        builder.Property(e => e.Minor)
            .HasConversion(x => (long) x, x => new BigInteger(x));
        builder.Property(e => e.Patch)
            .HasConversion(x => (long) x, x => new BigInteger(x));
        builder.Property(e => e.Prerelease)
            .HasMaxLength(32);
        builder.Property(e => e.Metadata)
            .HasMaxLength(32);
            
        builder.Ignore(e => e.IsPrerelease)
            .Ignore(e => e.IsRelease)
            .Ignore(e => e.PrereleaseIdentifiers)
            .Ignore(e => e.MetadataIdentifiers);

        builder.HasIndex(e => new {
            e.Major,
            e.Minor,
            e.Patch,
            e.Prerelease,
            e.Metadata
        })
        .IsUnique();
        
        return builder;
    }
    
}