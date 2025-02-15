using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Tests.Utils;

public class LinqExtensionsTest {
    private static readonly int[] Expected = [2, 4, 6, 8, 10];

    [Test]
    public void TestSelectValid() {
        var numbers = Enumerable.Range(1, 10)
            .SelectValid(x => {
                if (x % 2 != 0) {
                    throw new Exception();
                }

                return x;
            })
            .ToList();
        Assert.That(numbers, Is.EquivalentTo(Expected));
        
        var numbersWithIndices = Enumerable.Range(1, 10)
            .SelectValid((x, i) => {
                if (x % 2 != 0) {
                    throw new Exception();
                }

                return (x, i);
            })
            .ToList();
        Assert.That(numbersWithIndices, Is.EquivalentTo(Expected.Select((x, i) => (x, (i * 2) + 1)).ToList()));
    }
    
}