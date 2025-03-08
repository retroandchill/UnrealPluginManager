using Microsoft.AspNetCore.Mvc.ModelBinding;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Server.Binding;

/// <summary>
/// A custom model binder for binding pagination parameters to a <see cref="Pageable"/> object.
/// </summary>
/// <remarks>
/// This model binder extracts pagination parameters, such as "page" and "size", from the HTTP request query string.
/// It parses these parameters and binds them to an instance of the <see cref="Pageable"/> type.
/// If the "page" or "size" parameters are not present in the query, default values are used.
/// </remarks>
public class PageableParameterBinding : IModelBinder {
  /// <inheritdoc />
  public Task BindModelAsync(ModelBindingContext bindingContext) {
    if (bindingContext.ModelType != typeof(Pageable)) {
      throw new InvalidOperationException();
    }

    var queryParams = bindingContext.HttpContext.Request.Query;
    queryParams.TryGetValue("page", out var page);
    queryParams.TryGetValue("size", out var size);
    string? pageString = page;
    string? sizeString = size;
    var pageNumber = pageString is not null ? int.Parse(pageString) : 1;
    var pageSize = sizeString is not null ? int.Parse(sizeString) : 10;

    bindingContext.Result = ModelBindingResult.Success(new Pageable(pageNumber, pageSize));
    return Task.CompletedTask;
  }
}