using Microsoft.AspNetCore.Mvc.ModelBinding;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Server.Binding;

/// <summary>
/// Provides a custom implementation of the <see cref="IModelBinderProvider"/> interface
/// for binding model types related to pagination.
/// </summary>
/// <remarks>
/// This provider specializes in binding models of type <see cref="Pageable"/>
/// using the <see cref="PageableParameterBinding"/> implementation.
/// </remarks>
public class PaginationModelBinderProvider : IModelBinderProvider {
  /// <inheritdoc />
  public IModelBinder? GetBinder(ModelBinderProviderContext context) {
    ArgumentNullException.ThrowIfNull(context);

    return context.Metadata.ModelType == typeof(Pageable) ? new PageableParameterBinding() : null;
  }
}