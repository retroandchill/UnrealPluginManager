using System.Diagnostics.CodeAnalysis;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for object validation and manipulation.
/// </summary>
public static class Objects {
  /// <summary>
  /// Ensures that the specified object is not null. If the object is null, an <see cref="ArgumentNullException"/> is thrown.
  /// </summary>
  /// <typeparam name="T">The type of the object being validated. Must be a class.</typeparam>
  /// <param name="obj">The object to validate for non-nullity.</param>
  /// <returns>The non-null object of type <typeparamref name="T"/>.</returns>
  /// <exception cref="ArgumentNullException">Thrown when the object is null.</exception>
  public static T RequireNonNull<T>([NotNull] this T? obj) where T : class {
    ArgumentNullException.ThrowIfNull(obj);
    return obj;
  }

  /// <summary>
  /// Ensures that the specified object is not null. If the object is null, the exception provided
  /// by the <paramref name="exceptionFactory"/> delegate is thrown.
  /// </summary>
  /// <typeparam name="T">The type of the object being validated. Must be a class.</typeparam>
  /// <param name="obj">The object to validate for non-nullity.</param>
  /// <param name="exceptionFactory">A function that creates the exception to throw if the object is null.</param>
  /// <returns>The non-null object of type <typeparamref name="T"/>.</returns>
  /// <exception cref="Exception">The exception created by <paramref name="exceptionFactory"/> when the object is null.</exception>
  public static T RequireNonNull<T>([NotNull] this T? obj, Func<Exception> exceptionFactory) where T : class {
    if (obj is null) {
      throw exceptionFactory();
    }

    return obj;
  }

  /// <summary>
  /// Ensures that the specified nullable struct is not null. If the struct is null, an <see cref="ArgumentNullException"/> is thrown.
  /// </summary>
  /// <typeparam name="T">The type of the struct being validated. Must be a value type.</typeparam>
  /// <param name="obj">The nullable struct to validate for non-nullity.</param>
  /// <returns>The non-null value of type <typeparamref name="T"/>.</returns>
  /// <exception cref="ArgumentNullException">Thrown when the nullable struct is null.</exception>
  public static T RequireNonNull<T>([NotNull] this T? obj) where T : struct {
    if (!obj.HasValue) {
      throw new ArgumentNullException(nameof(obj));
    }

    return obj.Value;
  }

}