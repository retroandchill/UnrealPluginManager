using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using LanguageExt;

namespace UnrealPluginManager.Cli.Utils;

public static partial class EngineVersionUtils {
    public static Version GetEngineVersion(this IFileSystem fileSystem, string installDirectory) {
        var versionFile = Path.Combine(installDirectory, "Engine", "Source", "Runtime", "Launch", "Resources", "Version.h");
        int? localEngineVersionMajor = null;
        int? localEngineVersionMinor = null;
        int? localEngineVersionPatch = null;
        if (!fileSystem.File.Exists(versionFile)) {
            throw new InvalidDataException("Failed to find version file at " + versionFile);
        }

        using var file = new StreamReader(versionFile);
        var pattern = VersionDefineRegex();
        while (file.ReadLine() is { } currentLine) {
            var match = pattern.Match(currentLine);
            if (!match.Success) {
                continue;
            }
            
            var versionPart = Enum.Parse<VersionPart>(match.Groups[1].Value, true);
            var value = int.Parse(match.Groups[2].Value);
            switch (versionPart) {
                case VersionPart.Major:
                    localEngineVersionMajor = value;
                    break;
                case VersionPart.Minor:
                    localEngineVersionMinor = value;
                    break;
                case VersionPart.Patch:
                    localEngineVersionPatch = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(">" + versionPart + "< is not a valid version part.");
            }
        }

        if (!localEngineVersionMajor.HasValue || !localEngineVersionMinor.HasValue) {
            throw new InvalidDataException("Failed to parse version from " + versionFile);
        }
        
        
        return localEngineVersionPatch.HasValue ? new Version(localEngineVersionMajor.Value, localEngineVersionMinor.Value, localEngineVersionPatch.Value) : new Version(localEngineVersionMajor.Value, localEngineVersionMinor.Value);

    }

    [GeneratedRegex(@"#define\s+ENGINE_(MAJOR|MINOR|PATCH)_VERSION\s+(\d+)")]
    private static partial Regex VersionDefineRegex();
}