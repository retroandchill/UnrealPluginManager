using Microsoft.AspNetCore.Mvc.ModelBinding;
using Semver;

namespace UnrealPluginManager.Server.Binding;

/// <summary>
/// A custom model binder for binding <c>SemVersion</c> types in ASP.NET Core applications.
/// </summary>
/// <remarks>
/// This class provides model binding functionality specifically for the <c>SemVersion</c> type. It ensures that
/// string representations of semantic versions in request parameters are correctly parsed into <c>SemVersion</c> instances.
/// </remarks>
/// <threadsafety>
/// This type is thread-safe as it does not maintain instance-specific or shared state.
/// </threadsafety>
/// <exception cref="InvalidOperationException">
/// Thrown when the model type being bound does not match the expected <c>SemVersion</c> type.
/// </exception>
/// <seealso cref="IModelBinder"/>
public class SemVersionParameterBinding : IModelBinder {

  /// <inheritdoc />
  public Task BindModelAsync(ModelBindingContext bindingContext) {
    if (bindingContext.ModelType != typeof(SemVersion)) {
      throw new InvalidOperationException();
    }
    
    var modelName = bindingContext.ModelName;
    var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
    var value = valueProviderResult.FirstValue;
    bindingContext.Result = ModelBindingResult.Success(value is not null ? SemVersion.Parse(value) : null);
    return Task.CompletedTask;
  }
}