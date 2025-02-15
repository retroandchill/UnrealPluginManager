using System.CommandLine.IO;

namespace UnrealPluginManager.Cli.Tests.Mocks;

/// <summary>
/// A utility class for capturing and redirecting written output to a string buffer,
/// intended for use in testing and mock implementations of stream writers.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IStandardStreamWriter"/> interface, enabling
/// it to be used in scenarios requiring a writable standard stream. It provides
/// functionalities for writing text, retrieving the written content, and flushing the underlying buffer.
/// </remarks>
public class RedirectingStreamWriter : IStandardStreamWriter {
    
    private readonly StringWriter _writer = new();

    /// <inheritdoc />
    public void Write(string? value) {
        _writer.Write(value);
    }

    /// <summary>
    /// Retrieves the content written to the underlying string writer buffer.
    /// </summary>
    /// <returns>
    /// A string containing the entirety of the data written to the string writer.
    /// </returns>
    public string GetWrittenString() {
        return _writer.ToString();
    }
}