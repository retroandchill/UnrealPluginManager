using UnrealPluginManager.Core.Solver;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Tests.Utils;

public class TestPermute {
    [Test]
    public void TestPermutations() {
        var numbers = Enumerable.Range(1, 10);
        var permutations = numbers.Permute(2)
            .Select(x => x.ToList())
            .Select(x => (x[0], x[1]))
            .ToList();
        Assert.AreEqual(90, permutations.Count);
    }
    
    [Test]
    public void TestCombinations() {
        var numbers = Enumerable.Range(1, 10);
        var combinations = numbers.Combinations(2)
            .Select(x => x.ToList())
            .Select(x => (x[0], x[1]))
            .ToList();
        Assert.AreEqual(45, combinations.Count);
    }
}