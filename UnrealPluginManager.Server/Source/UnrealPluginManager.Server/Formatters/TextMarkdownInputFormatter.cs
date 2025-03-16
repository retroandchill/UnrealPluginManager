using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace UnrealPluginManager.Server.Formatters;

/// <summary>
/// A custom input formatter for handling text/markdown content.
/// </summary>
/// <remarks>
/// This input formatter processes requests with a text/markdown media type and reads the content as a UTF-8 or Unicode encoded string.
/// </remarks>
public class TextMarkdownInputFormatter : TextInputFormatter {

  /// A custom input formatter that processes text/markdown media types.
  /// This formatter supports UTF-8 and Unicode character encodings and reads the request body to produce markdown content.
  public TextMarkdownInputFormatter() {
    SupportedMediaTypes.Add(MediaTypeNames.Text.Markdown);

    SupportedEncodings.Add(UTF8EncodingWithoutBOM);
    SupportedEncodings.Add(UTF16EncodingLittleEndian);
  }

  /// <inheritdoc />
  public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
                                                                        Encoding encoding) {
    var request = context.HttpContext.Request;

    using var reader = new StreamReader(request.Body, encoding);
    var content = await reader.ReadToEndAsync();
    return await InputFormatterResult.SuccessAsync(content);
  }

  /// <inheritdoc />
  protected override bool CanReadType(Type type) {
    return type == typeof(string);
  }

}