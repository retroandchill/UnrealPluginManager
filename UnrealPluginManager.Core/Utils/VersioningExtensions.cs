using System.Linq.Expressions;
using LinqKit;
using Semver;
using UnrealPluginManager.Core.Database.Entities;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides extension methods for versioning functionalities, particularly related to
/// handling and extracting information from semantic versioning components.
/// </summary>
public static class VersioningExtensions {

    /// <summary>
    /// Retrieves the numeric value of the second prerelease identifier in a semantic version.
    /// The method validates that exactly two prerelease identifiers are present and ensures
    /// the second identifier contains a numeric value.
    /// </summary>
    /// <param name="identifiers">
    /// A read-only list of prerelease identifiers extracted from a semantic version.
    /// </param>
    /// <returns>
    /// The numeric value of the second prerelease identifier as an integer.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the number of prerelease identifiers is not equal to two.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the second prerelease identifier does not contain a numeric value.
    /// </exception>
    public static int GetPrereleaseNumber(this IReadOnlyList<PrereleaseIdentifier> identifiers) {
        ArgumentOutOfRangeException.ThrowIfNotEqual(identifiers.Count, 2);
        var numericValue = identifiers[1].NumericValue;
        return numericValue is not null
            ? (int) numericValue
            : throw new ArgumentException("Invalid release candidate number.");
    }

    /// <summary>
    /// Filters a queryable collection of versioned entities to include only those
    /// whose semantic versions fall within the specified version range.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entities in the query, which must implement the <c>IVersionedEntity</c> interface.
    /// </typeparam>
    /// <param name="query">
    /// The queryable collection of entities to filter.
    /// </param>
    /// <param name="range">
    /// The semantic version range to evaluate against. Only entities with versions
    /// that fall within this range will be included in the results.
    /// </param>
    /// <returns>
    /// A filtered queryable collection containing the entities whose versions
    /// are within the specified semantic version range.
    /// </returns>
    public static IQueryable<TEntity> WhereVersionInRange<TEntity>(this IQueryable<TEntity> query,
        SemVersionRange range) where TEntity : class, IVersionedEntity {
        return query.Where(VersionRangeQuery<TEntity>(range));
    }


    private static Expression<Func<TEntity, bool>> VersionRangeQuery<TEntity>(this SemVersionRange range)
        where TEntity : class, IVersionedEntity {
        Expression<Func<TEntity, bool>> fullPredicate = PredicateBuilder.New<TEntity>(false);

        foreach (var unbrokenRange in range) {
            Expression<Func<TEntity, bool>> rangePredicate = PredicateBuilder.New<TEntity>(true);
            if (unbrokenRange.Start is not null) {
                rangePredicate = rangePredicate.And(GetStartComparison<TEntity>(unbrokenRange));
            }

            if (unbrokenRange.End is not null) {
                rangePredicate = rangePredicate.And(GetEndCompare<TEntity>(unbrokenRange));
            }

            if (!unbrokenRange.IncludeAllPrerelease) {
                Expression<Func<TEntity, bool>> prereleasePredicate =
                    PredicateBuilder.New<TEntity>(x => x.PrereleaseNumber == null);
                if (unbrokenRange.Start?.IsPrerelease ?? false) {
                    prereleasePredicate = prereleasePredicate.OrMajorMinorPatchEquals(unbrokenRange.Start);
                }

                if (unbrokenRange.End?.IsPrerelease ?? false) {
                    prereleasePredicate = prereleasePredicate.OrMajorMinorPatchEquals(unbrokenRange.End);
                }

                rangePredicate = rangePredicate.And(prereleasePredicate);
            }

            fullPredicate = fullPredicate.Or(rangePredicate);
        }

        return fullPredicate;
    }

    private static Expression<Func<TEntity, bool>> OrMajorMinorPatchEquals<TEntity>(
        this Expression<Func<TEntity, bool>> expr, SemVersion version)
        where TEntity : class, IVersionedEntity {
        return expr.Or(x => x.Major == version.Major && x.Minor == version.Minor && x.Patch == version.Patch);
    }

    private static Expression<Func<TEntity, bool>> GetStartComparison<TEntity>(UnbrokenSemVersionRange unbrokenRange)
        where TEntity : class, IVersionedEntity {
        ArgumentNullException.ThrowIfNull(unbrokenRange.Start);

        Expression<Func<TEntity, bool>> startCompare;
        if (unbrokenRange.StartInclusive) {
            startCompare = PredicateBuilder.New<TEntity>(x => x.Major >= unbrokenRange.Start.Major)
                .And(x => x.Minor >= unbrokenRange.Start.Minor)
                .And(x => x.Patch >= unbrokenRange.Start.Patch);
            if (!unbrokenRange.Start.IsPrerelease) {
                return startCompare;
            }

            var startPrerelease = unbrokenRange.Start.PrereleaseIdentifiers.GetPrereleaseNumber();
            startCompare = startCompare.And(x => x.PrereleaseNumber == null ||
                                                 x.PrereleaseNumber.Value >= startPrerelease);

        } else {
            startCompare = PredicateBuilder.New<TEntity>(x => x.Major > unbrokenRange.Start.Major)
                .Or(x => x.Major == unbrokenRange.Start.Major && x.Minor > unbrokenRange.Start.Minor)
                .Or(x => x.Major == unbrokenRange.Start.Major && x.Minor == unbrokenRange.Start.Minor &&
                         x.Patch > unbrokenRange.Start.Patch);

            if (!unbrokenRange.Start.IsPrerelease) {
                return startCompare;
            }

            var startPrerelease = unbrokenRange.Start.PrereleaseIdentifiers.GetPrereleaseNumber();
            startCompare = startCompare.Or(x => x.Major == unbrokenRange.Start.Major
                                                && x.Minor == unbrokenRange.Start.Minor
                                                && x.Patch == unbrokenRange.Start.Patch
                                                && (x.PrereleaseNumber == null ||
                                                    x.PrereleaseNumber.Value > startPrerelease));

        }
        return startCompare;
    }

    private static Expression<Func<TEntity, bool>> GetEndCompare<TEntity>(UnbrokenSemVersionRange unbrokenRange)
        where TEntity : class, IVersionedEntity {
        ArgumentNullException.ThrowIfNull(unbrokenRange.End);

        Expression<Func<TEntity, bool>> endCompare;
        if (unbrokenRange.EndInclusive) {
            endCompare = PredicateBuilder.New<TEntity>(x => x.Major <= unbrokenRange.End.Major)
                .And(x => x.Minor <= unbrokenRange.End.Minor)
                .And(x => x.Patch <= unbrokenRange.End.Patch);
            if (!unbrokenRange.End.IsPrerelease) {
                return endCompare;
            }

            var endPrerelease = unbrokenRange.End.PrereleaseIdentifiers.GetPrereleaseNumber();
            endCompare = endCompare.And(x => x.PrereleaseNumber != null &&
                                             x.PrereleaseNumber.Value <= endPrerelease);

        } else {
            endCompare = PredicateBuilder.New<TEntity>(x => x.Major < unbrokenRange.End.Major)
                .Or(x => x.Major == unbrokenRange.End.Major && x.Minor < unbrokenRange.End.Minor)
                .Or(x => x.Major == unbrokenRange.End.Major && x.Minor == unbrokenRange.End.Minor &&
                         x.Patch < unbrokenRange.End.Patch);

            if (!unbrokenRange.End.IsPrerelease) {
                return endCompare;
            }

            var endPrerelease = unbrokenRange.End.PrereleaseIdentifiers.GetPrereleaseNumber();
            endCompare = endCompare.Or(x => x.Major == unbrokenRange.End.Major
                                            && x.Minor == unbrokenRange.End.Minor
                                            && x.Patch == unbrokenRange.End.Patch
                                            && x.PrereleaseNumber != null &&
                                            x.PrereleaseNumber.Value < endPrerelease);

        }
        return endCompare;
    }
}