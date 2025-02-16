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

    public static IEnumerable<T> AsEnumerable<T>(this T? value)
        where T : struct {
        if (value is not null) {
            yield return value.Value;
        }
    }

    public static IEnumerable<T> AsEnumerable<T>(this T? value)
        where T : class {
        if (value is not null) {
            yield return value;
        }
    }

}