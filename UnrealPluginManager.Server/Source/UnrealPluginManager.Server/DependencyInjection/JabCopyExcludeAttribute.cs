namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// An attribute used to indicate types that should be excluded from the copying process
/// in the context of dependency injection settings and configurations managed by the Jab library.
/// </summary>
/// <remarks>
/// This attribute is primarily applied to `ServiceProvider` implementations or similar constructs
/// to mark specific types that should not be copied or handled by Jab during the dependency injection setup.
/// </remarks>
/// <param name="excludedTypes">An array of types to be excluded from the copying process. These types will not be included in the dependency management scope.</param>
/// <example>
/// The intended usage assumes exclusion of specific types from a Jab `ServiceProvider`, ensuring those types are skipped during the registration process.
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public class JabCopyExcludeAttribute(params Type[] excludedTypes) : Attribute {

  /// <summary>
  /// Gets the collection of types that are excluded from the copying process
  /// in dependency injection configurations managed by the Jab library.
  /// </summary>
  /// <remarks>
  /// The excluded types specified in this property are used to prevent particular types
  /// from being processed during the dependency injection setup. Typically, this includes
  /// types that should not be registered or managed within the dependency management scope.
  /// </remarks>
  /// <value>
  /// A read-only list of <see cref="Type"/> objects representing the excluded types.
  /// </value>
  public IReadOnlyList<Type> ExcludedTypes { get; } = excludedTypes;

}