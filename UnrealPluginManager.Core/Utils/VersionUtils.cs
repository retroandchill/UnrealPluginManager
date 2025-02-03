using Semver;

namespace UnrealPluginManager.Core.Utils;

public static class VersionUtils {
    public static SemVersion ToSemVersion(this Version version) {
        return SemVersion.Parse(version.ToString());
    }
}