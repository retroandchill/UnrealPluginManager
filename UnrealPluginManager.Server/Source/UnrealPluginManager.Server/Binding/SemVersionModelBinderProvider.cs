using Microsoft.AspNetCore.Mvc.ModelBinding;
using Semver;

namespace UnrealPluginManager.Server.Binding;

/// <summary>
/// Provides a model binder for binding <c>SemVersion</c> and <c>SemVersionRange</c> types
/// in ASP.NET Core applications.
/// </summary>
/// <remarks>
/// This class implements the <c>IModelBinderProvider</c> interface to supply custom binding logic
/// for models of type <c>SemVersion</c> and <c>SemVersionRange</c>.
/// </remarks>
/// <threadsafety>
/// This type is thread-safe as it does not maintain instance-specific or shared state.
/// </threadsafety>
public class SemVersionModelBinderProvider : IModelBinderProvider {

  /// <inheritdoc />
  public IModelBinder? GetBinder(ModelBinderProviderContext context) {
    if (typeof(SemVersion).IsAssignableFrom(context.Metadata.ModelType)) {
      return new SemVersionParameterBinding();
    }
    
    return typeof(SemVersionRange).IsAssignableFrom(context.Metadata.ModelType) ? new SemVersionRangeParameterBinding() : null;

  }
}