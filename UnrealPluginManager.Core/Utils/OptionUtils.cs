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
    public static T Get<T>(this Option<T> option) => option.Match(
        x => x,
        () => throw new InvalidOperationException("Option is None")
    );

    /// <summary>
    /// Retrieves the value from a <see cref="Fin{T}"/> instance if the operation was successful.
    /// Throws the associated exception if the operation failed.
    /// </summary>
    /// <typeparam name="T">The type of the value contained within the Fin.</typeparam>
    /// <param name="fin">The Fin instance from which to retrieve the value or exception.</param>
    /// <returns>The value contained within the Fin if the operation was successful.</returns>
    /// <exception cref="Exception">Thrown if the Fin represents a failed operation.</exception>
    public static T Get<T>(this Fin<T> fin) => fin.Match(
        x => x,
        x => throw x.ToException()
    );

    /// <summary>
    /// Converts a nullable value of type <typeparamref name="T"/> to an enumerable containing the value if it is not null.
    /// In the case of nullable value types, the returned enumerable will contain the value if present.
    /// </summary>
    /// <typeparam name="T">The base type of the nullable value being processed.</typeparam>
    /// <param name="value">The nullable value to convert into an enumerable.</param>
    /// <returns>An enumerable containing the value if it is not null; otherwise, an empty enumerable.</returns>
    public static IEnumerable<T> AsEnumerable<T>(this T? value)
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
    public static IEnumerable<T> AsEnumerable<T>(this T? value)
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
    /// Matches a nullable value to one of two possible outcomes based on whether the value is present or null.
    /// Executes the given function if the value is present, or a fallback function if the value is null.
    /// </summary>
    /// <typeparam name="T">The type of the nullable value to be matched.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by executing one of the provided functions.</typeparam>
    /// <param name="value">The nullable value to be matched.</param>
    /// <param name="some">The function to execute if the value is not null.</param>
    /// <param name="none">The function to execute if the value is null.</param>
    /// <returns>The result of either the <paramref name="some"/> or <paramref name="none"/> function, depending on whether the <paramref name="value"/> is null.</returns>
    public static TResult Match<T, TResult>(this T? value, Func<T, TResult> some, Func<TResult> none) where T : struct {
        return value is not null ? some(value.Value) : none();
    }

    public static T OrElseGet<T>(this Option<T> option, Func<T> fallback) {
        return option.Match(
            x => x,
            fallback
        );
    }

    public static Task<T> OrElseGet<T>(this Option<T> option, Func<Task<T>> fallback) {
        return option.Match(
            Task.FromResult,
            fallback
        );
    }
}