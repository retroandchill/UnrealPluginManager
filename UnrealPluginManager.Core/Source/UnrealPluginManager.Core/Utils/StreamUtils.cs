namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides utility methods for working with streams.
/// </summary>
public static class StreamUtils {

  /// <summary>
  /// Converts the specified string to a memory stream.
  /// </summary>
  /// <param name="str">The string to be converted into a memory stream.</param>
  /// <returns>A memory stream containing the content of the string.</returns>
  public static Stream ToStream(this string str) {
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write(str);
    writer.Flush();
    stream.Position = 0;
    return stream;
  }

}