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

  [Test]
  public void TestToOrderedDictionary() {
    var dict1 = new OrderedDictionary<int, string> {
        { 1, "2" },
        { 2, "1" },
        { 3, "3" }
    };

    var dict2 = dict1.Where(x => x.Key > 1).ToOrderedDictionary();
    Assert.That(dict2, Has.Count.EqualTo(2));

    var dict3 = Enumerable.Range(0, 10).ToOrderedDictionary(x => x.ToString(), y => y);
    Assert.That(dict3, Has.Count.EqualTo(10));
  }
}