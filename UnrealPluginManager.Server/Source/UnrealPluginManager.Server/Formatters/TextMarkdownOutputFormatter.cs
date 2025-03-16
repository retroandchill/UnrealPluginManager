using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace UnrealPluginManager.Server.Formatters;

/// Represents a custom output formatter for handling `text/markdown` content types in ASP.NET Core applications.
/// This formatter extends the `TextOutputFormatter` class to process and return responses in markdown format.
/// Use this class to enable markdown output formatting when working with ASP.NET Core controllers.
/// It supports UTF-8 and Unicode encodings for text/markdown content.
/// Key Features:
/// - Adds support for `text/markdown` media type.
/// - Ensures the response data is written as a string.
/// - Can process response data explicitly of string type.
/// The formatter is typically registered as part of the MVC configuration pipeline within an ASP.NET Core application.
/// Implementation Details:
/// - Overrides the `CanWriteType` method to restrict the formatter to only handle string objects.
/// - Implements `WriteResponseBodyAsync` to write output content represented as strings.
public class TextMarkdownOutputFormatter : TextOutputFormatter {

  /// A custom output formatter for handling text/markdown content types in ASP.NET Core applications.
  /// Inherits from the TextOutputFormatter and adds support for markdown output with UTF-8 and Unicode encodings.
  /// Can process and write response data explicitly of string type.
  public TextMarkdownOutputFormatter() {
    SupportedMediaTypes.Add(MediaTypeNames.Text.Markdown);

    SupportedEncodings.Add(Encoding.UTF8);
    SupportedEncodings.Add(Encoding.Unicode);

  }

  /// <inheritdoc />
  public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {
    var response = context.HttpContext.Response;
    var content = context.Object?.ToString();
    ArgumentNullException.ThrowIfNull(content);
    await response.WriteAsync(content);
  }

  /// <inheritdoc />
  protected override bool CanWriteType(Type? type) {
    return type == typeof(string);
  }
}