namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for generating combinations and permutations
/// from a given collection of elements.
/// </summary>
public static class Combinatorics {
    /// <summary>
    /// Generates all possible combinations of a specified size from a given collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input collection.</typeparam>
    /// <param name="list">The collection of elements to generate combinations from.</param>
    /// <param name="count">The number of elements in each combination.</param>
    /// <returns>An enumerable of combinations, where each combination is represented as an enumerable of elements.</returns>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> list, uint count) {
        if (count == 0) {
            yield return [];
        } else {
            var startingIndex = 0;
            var asList = list.ToList();
            foreach (var startingElement in asList) {
                var remainingItems = asList.AllAfter(startingIndex);
                foreach (var permutationOfRemainder in remainingItems.Combinations(count - 1)) {
                    yield return new[] { startingElement }.Concat(permutationOfRemainder);
                }

                startingIndex++;
            }
        }
    }
    
    private static IEnumerable<T> AllAfter<T>(this IEnumerable<T> input, int indexToSkip) {
        var index = 0;
        foreach (var item in input) {
            if (index > indexToSkip) {
                yield return item;
            }
            index++;
        }
    }

    /// <summary>
    /// Generates all possible permutations of a specified size from a given collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input collection.</typeparam>
    /// <param name="list">The collection of elements to generate permutations from.</param>
    /// <param name="count">The number of elements in each permutation.</param>
    /// <returns>An enumerable of permutations, where each permutation is represented as an enumerable of elements.</returns>
    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> list, uint count) {
        if (count == 0) {
            yield return [];
        } else {
            var startingIndex = 0;
            var asList = list.ToList();
            foreach (var startingElement in asList) {
                var remainingItems = asList.AllExcept(startingIndex);
                foreach (var permutationOfRemainder in remainingItems.Permute(count - 1)) {
                    yield return new[] { startingElement }.Concat(permutationOfRemainder);
                }

                startingIndex++;
            }
        }
    }

    private static IEnumerable<T> AllExcept<T>(this IEnumerable<T> input, int indexToSkip) {
        var index = 0;
        foreach (var item in input) {
            if (index != indexToSkip) {
                yield return item;
            }
            index++;
        }
    }
}