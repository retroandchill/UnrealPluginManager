namespace UnrealPluginManager.Core.Utils;

public static class LinqExtensions {
    /// <summary>
    /// Projects each element of a sequence into a new form by using a provided transform function,
    /// ignoring elements where the transform function results in exceptions.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the transform function.</typeparam>
    /// <param name="source">The source sequence to project.</param>
    /// <param name="selector">A transform function to apply to each element in the source sequence.</param>
    /// <returns>An enumerable collection of the transformed elements that did not throw exceptions when processed by the selector function.</returns>
    public static IEnumerable<TResult> SelectValid<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector) {
        foreach (var item in source) {
            TResult result;
            try {
                result = selector(item);
            } catch (Exception) {
                continue;
            }
                
            yield return result;
        }
    }

    /// <summary>
    /// Projects each element of a sequence into a new form using a provided transform function
    /// that takes both the element and its index, ignoring elements where the transform function results in exceptions.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the transform function.</typeparam>
    /// <param name="source">The source sequence to project.</param>
    /// <param name="selector">A transform function to apply to each element in the source sequence, which takes the element and its index as parameters.</param>
    /// <returns>An enumerable collection of the transformed elements that did not throw exceptions when processed by the selector function.</returns>
    public static IEnumerable<TResult> SelectValid<T, TResult>(this IEnumerable<T> source, 
        Func<T, int, TResult> selector) {
        int i = 0;
        foreach (var item in source) {
            TResult result;
            try {
                result = selector(item, i);
            } catch (Exception) {
                continue;
            }
                
            yield return result;
        }
    }
    
}