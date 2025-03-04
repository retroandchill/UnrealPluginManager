using LanguageExt;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// A utility class for working with the Option monad from the LanguageExt library.
/// </summary>
public static class OptionUtils {
  /// <summary>
  /// Retrieves the value from an <see cref="Option{T}"/> instance if the option contains a value.
  /// Throws an <see cref="InvalidOperationException"/> if the option is None.
  /// </summary>
  /// <typeparam name="T">The type of the value contained within the Option.</typeparam>
  /// <param name="option">The Option instance from which to retrieve the value.</param>
  /// <returns>The value contained within the option.</returns>
  /// <exception cref="InvalidOperationException">Thrown if the Option contains no value.</exception>
  public static T OrElseThrow<T>(this Option<T> option) {
    return option.OrElseThrow(() => new InvalidOperationException("Option is None"));
  }

  /// <summary>
  /// Retrieves the value from an <see cref="Option{T}"/> instance if the option contains a value.
  /// Throws the exception created by the provided exception factory if the option is None.
  /// </summary>
  /// <typeparam name="T">The type of the value contained within the Option.</typeparam>
  /// <param name="option">The Option instance from which to retrieve the value.</param>
  /// <param name="exceptionFactory">A factory method to generate the exception to be thrown if the Option contains no value.</param>
  /// <returns>The value contained within the option.</returns>
  /// <exception cref="Exception">Thrown if the Option contains no value and the provided exception factory creates an exception.</exception>
  public static T OrElseThrow<T>(this Option<T> option, Func<Exception> exceptionFactory) {
    return option.Match(x => x,
                        () => throw exceptionFactory());
  }

  /// <summary>
  /// Retrieves the value from a <see cref="Fin{T}"/> instance if the operation was successful.
  /// Throws the associated exception if the operation failed.
  /// </summary>
  /// <typeparam name="T">The type of the value contained within the Fin.</typeparam>
  /// <param name="fin">The Fin instance from which to retrieve the value or exception.</param>
  /// <returns>The value contained within the Fin if the operation was successful.</returns>
  /// <exception cref="Exception">Thrown if the Fin represents a failed operation.</exception>
  public static T OrElseThrow<T>(this Fin<T> fin) {
    return fin.Match(x => x,
                     x => throw x.ToException());
  }

  public static Option<T> OrElse<T>(this Option<T> option, Func<Option<T>> fallback) {
    return option.Match(x => x, fallback);
  }

  public static Task<Option<T>> OrElseAsync<T>(this Option<T> option, Func<Task<Option<T>>> fallback) {
    return option.Match(x => Task.FromResult<Option<T>>(x), fallback);
  }

  /// <summary>
  /// Converts a nullable value of type <typeparamref name="T"/> to an enumerable containing the value if it is not null.
  /// In the case of nullable value types, the returned enumerable will contain the value if present.
  /// </summary>
  /// <typeparam name="T">The base type of the nullable value being processed.</typeparam>
  /// <param name="value">The nullable value to convert into an enumerable.</param>
  /// <returns>An enumerable containing the value if it is not null; otherwise, an empty enumerable.</returns>
  public static IEnumerable<T> ToEnumerable<T>(this T? value)
      where T : struct {
    if (value is not null) {
      yield return value.Value;
    }
  }

  /// <summary>
  /// Converts a nullable reference type instance into an enumerable sequence containing the value if it is not null.
  /// </summary>
  /// <typeparam name="T">The type of the value.</typeparam>
  /// <param name="value">The nullable value type instance to convert.</param>
  /// <returns>An enumerable sequence containing the value if it is not null, or an empty sequence if the value is null.</returns>
  public static IEnumerable<T> ToEnumerable<T>(this T? value)
      where T : class {
    if (value is not null) {
      yield return value;
    }
  }

  /// <summary>
  /// Converts a nullable value to an <see cref="Option{T}"/>.
  /// If the value is non-null, it is wrapped in an Option.
  /// If the value is null, returns a None.
  /// </summary>
  /// <typeparam name="T">The type of the object being converted.</typeparam>
  /// <param name="value">The nullable value to convert to an Option.</param>
  /// <returns>An <see cref="Option{T}"/> containing the value if non-null, or None if the value is null.</returns>
  public static Option<T> ToOption<T>(this T? value) {
    return value ?? Option<T>.None;
  }

  /// <summary>
  /// Converts a task producing a nullable value into a task producing an <see cref="Option{T}"/>.
  /// </summary>
  /// <typeparam name="T">The type of the value that the task might produce.</typeparam>
  /// <param name="value">The task producing a nullable value to be converted to an Option.</param>
  /// <returns>A task that produces an <see cref="Option{T}"/> containing the value if it is not null, or None if it is null.</returns>
  public static Task<Option<T>> ToOptionAsync<T>(this Task<T?> value) {
    return value.Map(ToOption);
  }

  /// <summary>
  /// Executes pattern matching logic on a nullable class type <see cref="T"/> value and produces a result of type <see cref="TResult"/>.
  /// </summary>
  /// <typeparam name="T">The type of the nullable class to be matched.</typeparam>
  /// <typeparam name="TResult">The type of the result produced by the matching logic.</typeparam>
  /// <param name="value">The nullable class instance to apply the match operation on.</param>
  /// <param name="some">The function to execute if the value is non-null.</param>
  /// <param name="none">The function to execute if the value is null.</param>
  /// <returns>The result of type <see cref="TResult"/> determined by executing either the <paramref name="some"/> or <paramref name="none"/> function.</returns>
  public static TResult Match<T, TResult>(this T? value, Func<T, TResult> some, Func<TResult> none) where T : class {
    return value is not null ? some(value) : none();
  }

  /// <summary>
  /// Projects the value contained within an <see cref="Option{T}"/> instance into a new <see cref="Option{TResult}"/> asynchronously
  /// using the provided function if the option contains a value.
  /// Returns None if the original option is None.
  /// </summary>
  /// <typeparam name="T">The type of the value contained within the original option.</typeparam>
  /// <typeparam name="TResult">The type of the value contained within the resulting option.</typeparam>
  /// <param name="option">The Option instance to project.</param>
  /// <param name="selector">A function that produces a new option asynchronously from the current value.</param>
  /// <returns>A task that represents the asynchronous operation, containing the projected <see cref="Option{TResult}"/> instance.</returns>
  public static Task<Option<TResult>> SelectManyAsync<T, TResult>(this Option<T> option, Func<T, Task<Option<TResult>>> selector) {
    return option.Match(
        selector,
        () => Task.FromResult(Option<TResult>.None)
    );
  }

  /// <summary>
  /// Returns the value contained in the given <see cref="Option{T}"/>
  /// if it has a value; otherwise, invokes the fallback function and returns its result.
  /// </summary>
  /// <typeparam name="T">The type of the value contained within the Option.</typeparam>
  /// <param name="option">The Option instance to retrieve the value from or use the fallback.</param>
  /// <param name="fallback">A function to compute the fallback value if the Option is None.</param>
  /// <returns>The value contained within the Option, or the result of the fallback function if the Option is None.</returns>
  public static T OrElseGet<T>(this Option<T> option, Func<T> fallback) {
    return option.Match(
        x => x,
        fallback
    );
  }

  /// <summary>
  /// Returns the value within an <see cref="Option{T}"/> instance if it contains a value;
  /// otherwise, it computes and returns a fallback value using the provided function.
  /// </summary>
  /// <typeparam name="T">The type of the value contained within the Option.</typeparam>
  /// <param name="option">The Option instance to evaluate.</param>
  /// <param name="fallback">A function to compute the fallback value if the Option is None.</param>
  /// <returns>The value contained within the Option, or the result of the fallback function if the Option is None.</returns>
  public static Task<T> OrElseGet<T>(this Option<T> option, Func<Task<T>> fallback) {
    return option.Match(
        Task.FromResult,
        fallback
    );
  }
}