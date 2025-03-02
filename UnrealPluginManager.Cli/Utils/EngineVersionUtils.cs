using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using LanguageExt;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Cli.Utils;

/// <summary>
/// Provides utility methods for retrieving information about the engine version from the file system.
/// </summary>
public static partial class EngineVersionUtils {
    /// <summary>
    /// Retrieves the Unreal Engine version from the specified installation directory.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction used to access files.</param>
    /// <param name="installDirectory">The root directory of the Unreal Engine installation.</param>
    /// <returns>
    /// A <see cref="Version"/> object representing the engine's major, minor, and optional patch version.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Thrown if the version file cannot be found or the version data cannot be successfully parsed.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if an invalid version part is encountered while parsing the version file.
    /// </exception>
    public static Version GetEngineVersion(this IFileSystem fileSystem, string installDirectory) {
        var versionFile = Path.Combine(installDirectory, "Engine", "Source", "Runtime", "Launch", "Resources", "Version.h");
        var localEngineVersionMajor = new Option<int>();
        var localEngineVersionMinor = new Option<int>();
        var localEngineVersionPatch = new Option<int>();
        if (!fileSystem.File.Exists(versionFile)) {
            throw new FileNotFoundException("Failed to find version file at " + versionFile);
        }
        
        using var file = fileSystem.File.OpenText(versionFile);
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

        if (!localEngineVersionMajor.IsSome || !localEngineVersionMinor.IsSome) {
            throw new InvalidDataException("Failed to parse version from " + versionFile);
        }
        
        
        return localEngineVersionPatch.IsSome ? new Version(localEngineVersionMajor.OrElseThrow(), localEngineVersionMinor.OrElseThrow(), 
            localEngineVersionPatch.OrElseThrow()) : new Version(localEngineVersionMajor.OrElseThrow(), localEngineVersionMinor.OrElseThrow());

    }

    [GeneratedRegex(@"#define\s+ENGINE_(MAJOR|MINOR|PATCH)_VERSION\s+(\d+)")]
    private static partial Regex VersionDefineRegex();
}