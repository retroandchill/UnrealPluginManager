using System.Text.Json;
using Semver;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Tests.Converters;

public class SemVersionJsonConverterTest {
  [Test]
  public void TestStringToSemVersion() {
    var versionOverview = new VersionOverview {
        Version = new SemVersion(1, 0, 0)
    };
    var serialized = JsonSerializer.Serialize(versionOverview);
    var deserialized = JsonSerializer.Deserialize<VersionOverview>(serialized);
    Assert.That(deserialized?.Version, Is.EqualTo(versionOverview.Version));

    var versionOverview2 = new VersionOverview {
        Version = new SemVersion(1, 0, 0, ["rc", "2"])
    };
    var serialized2 = JsonSerializer.Serialize(versionOverview2);
    var deserialized2 = JsonSerializer.Deserialize<VersionOverview>(serialized2);
    Assert.That(deserialized2?.Version, Is.EqualTo(versionOverview2.Version));

    var versionOverview3 = new VersionOverview {
        Version = new SemVersion(1, 0, 0, ["rc"], ["beta", "1"])
    };
    var serialized3 = JsonSerializer.Serialize(versionOverview3);
    Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<VersionOverview>(serialized3));
  }
}