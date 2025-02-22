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
        Some: x => x,
        None: () => throw new InvalidOperationException("Option is None")
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
}