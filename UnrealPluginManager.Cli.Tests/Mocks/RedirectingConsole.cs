using System.CommandLine;
using System.CommandLine.IO;

namespace UnrealPluginManager.Cli.Tests.Mocks;

/// <summary>
/// Provides a mock implementation of the <see cref="System.CommandLine.IConsole"/> interface, allowing for the redirection
/// and capturing of console output and error streams for testing purposes.
/// </summary>
/// <remarks>
/// This class uses internal <see cref="RedirectingStreamWriter"/> instances to manage and capture the
/// output and error streams. It allows for inspecting the content written to the console during testing.
/// </remarks>
public class RedirectingConsole : IConsole {
  private readonly RedirectingStreamWriter _outWriter = new();
  private readonly RedirectingStreamWriter _errorWriter = new();

  /// <inheritdoc />
  public IStandardStreamWriter Out => _outWriter;

  /// <inheritdoc />
  public bool IsOutputRedirected => true;

  /// <inheritdoc />
  public IStandardStreamWriter Error => _errorWriter;

  /// <inheritdoc />
  public bool IsErrorRedirected => true;

  /// <inheritdoc />
  public bool IsInputRedirected => false;

  /// <summary>
  /// Retrieves the content written to the standard output stream managed by the current console instance.
  /// </summary>
  /// <returns>
  /// A string containing all the data captured from the redirected standard output stream.
  /// </returns>
  public string GetWrittenOut() {
    return _outWriter.GetWrittenString();
  }

  /// <summary>
  /// Retrieves the content written to the standard error stream managed by the current console instance.
  /// </summary>
  /// <returns>
  /// A string containing all the data captured from the redirected standard error stream.
  /// </returns>
  public string GetWrittenError() {
    return _errorWriter.GetWrittenString();
  }
}