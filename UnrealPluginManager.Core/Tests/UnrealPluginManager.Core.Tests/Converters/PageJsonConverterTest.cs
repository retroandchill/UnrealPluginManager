using System.Text.Json;
using Semver;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Tests.Converters;

public class PageJsonConverterTest {
  private static readonly JsonSerializerOptions Options = new();

  [Test]
  public void TestSerializePrimitivePage() {
    var page = new Page<int>([1, 2, 3, 4, 5], 5, 5, 10);
    var serialized = JsonSerializer.Serialize(page, Options);
    var deserialized = JsonSerializer.Deserialize<Page<int>>(serialized, Options);
    Assert.That(deserialized, Is.EqualTo(page));
  }

  [Test]
  public void TestSerializeComplexPage() {
    var plugins = new List<VersionOverview> {
        new() {
            Id = Guid.NewGuid(),
            Version = new SemVersion(1, 0, 0)
        },
        new() {
            Id = Guid.NewGuid(),
            Version = new SemVersion(1, 0, 1)
        },
        new() {
            Id = Guid.NewGuid(),
            Version = new SemVersion(2, 0, 0)
        }
    };
    var page = new Page<VersionOverview>(plugins, plugins.Count, 5, 10);
    var serialized = JsonSerializer.Serialize(page, Options);
    var deserialized = JsonSerializer.Deserialize<Page<VersionOverview>>(serialized, Options);
    Assert.That(deserialized, Has.Count.EqualTo(3));
  }
}