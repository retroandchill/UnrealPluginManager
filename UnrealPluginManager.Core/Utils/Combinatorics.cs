namespace UnrealPluginManager.Core.Utils;

public static class Combinatorics {
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