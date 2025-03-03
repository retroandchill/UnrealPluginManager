using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Tests.Pagination;

public class TestPaginationExtensions {
  [Test]
  public void TestPageToEndSynchronous() {
    var originalList = Enumerable.Range(1, 100).ToList();
    var asPages = originalList.AsPages().ToList();
    var newList = Enumerable.Range(0, 1).PageToEnd((x, p) => asPages[p.PageNumber - 1]).ToList();
    Assert.That(newList, Is.EqualTo(originalList));
  }
}