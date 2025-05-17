using System.Reflection;
using Jab;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Represents data encapsulating an attribute and its associated service type.
/// </summary>
public record struct AttributeTypeData(Attribute Attribute, Type ServiceType);

/// <summary>
/// Provides extension methods for configuring and injecting services into the dependency injection container
/// using a custom service provider pattern in conjunction with Jab attributes.
/// </summary>
public static class JabExtensions {
  /// <summary>
  /// Registers services into the dependency injection container based on custom attributes
  /// defined in the <c>TProviderType</c> class. The method supports singleton, scoped, and
  /// transient lifetimes for services and excludes types marked with the <c>JabCopyExcludeAttribute</c>.
  /// </summary>
  /// <typeparam name="TProviderType">
  /// The type implementing <c>IServerServiceProvider</c> that contains the custom attributes
  /// defining the services to be registered.
  /// </typeparam>
  /// <param name="services">
  /// The <c>IServiceCollection</c> instance to which the services will be added.
  /// </param>
  /// <param name="serverServiceProviderFactory">
  /// A factory function to create an instance of the <c>TProviderType</c>.
  /// </param>
  /// <returns>
  /// The <c>IServiceCollection</c> instance enriched with the registered services.
  /// </returns>
  public static IServiceCollection AddJabServices<TProviderType>(this IServiceCollection services,
                                                                 Func<IServiceProvider, TProviderType>
                                                                     serverServiceProviderFactory)
      where TProviderType : class, IServerServiceProvider {
    var providerType = typeof(TProviderType);
    var excludedTypes = providerType.GetCustomAttribute<JabCopyExcludeAttribute>()?.ExcludedTypes ?? [];

    services.AddSingleton<IServerServiceProvider>(serverServiceProviderFactory)
        .AddScoped(p => {
          var serverProvider = p.GetRequiredService<IServerServiceProvider>();
          return serverProvider.CreateScope();
        });

    foreach (var (attribute, serviceType) in providerType.GetInjectionAttributes()
                 .Where(x => !IsInvalidType(x.ServiceType, excludedTypes))) {
      var attributeType = attribute.GetType();
      if (attributeType.IsBaseOf<SingletonAttribute>()) {
        services.AddSingleton(serviceType, p => {
          var serverServiceProvider = p.GetRequiredService<IServerServiceProvider>();
          return serverServiceProvider.GetRequiredService(serviceType);
        });
      } else if (attributeType.IsBaseOf<ScopedAttribute>()) {
        services.AddScoped(serviceType, p => {
          var serverServiceProvider = p.GetRequiredService<IServerServiceProvider.IScope>();
          return serverServiceProvider.GetRequiredService(serviceType);
        });
      } else if (attributeType.IsBaseOf<TransientAttribute>()) {
        services.AddTransient(serviceType, p => {
          var serverServiceProvider = p.GetRequiredService<IServerServiceProvider>();
          return serverServiceProvider.GetRequiredService(serviceType);
        });
      }
    }

    return services;
  }

  private static IEnumerable<AttributeTypeData> GetInjectionAttributes(this Type type) {
    return type.GetCustomAttributes()
        .Where(IsJabAttribute)
        .SelectMany(x => {
          if (!x.GetType().IsBaseOf(typeof(ImportAttribute))) {
            return [x.GetAttributeTypeData()];
          }

          var property = x.GetType().GetProperty(nameof(ImportAttribute.ModuleType));
          var moduleType = property?.GetValue(x) as Type;
          moduleType.RequireNonNull();
          return moduleType.GetInjectionAttributes();
        });
  }

  private static bool IsJabAttribute(Attribute attribute) {
    return attribute.GetType().IsBaseOfAny([
        typeof(ImportAttribute),
        typeof(SingletonAttribute),
        typeof(ScopedAttribute),
        typeof(TransientAttribute)
    ]);
  }

  private static bool IsBaseOf<T>(this Type type) {
    return type.IsBaseOf(typeof(T));
  }

  private static bool IsBaseOf(this Type type, Type baseType) {
    if (type.Namespace != nameof(Jab)) {
      return false;
    }

    if (baseType.FullName == type.FullName) {
      return true;
    }

    return type.BaseType is not null && IsBaseOf(type.BaseType, baseType);
  }

  private static AttributeTypeData GetAttributeTypeData(this Attribute attribute) {
    var typedAttribute = attribute.GetType().GetProperty(nameof(SingletonAttribute.ServiceType));
    var serviceType = typedAttribute?.GetValue(attribute) as Type;
    serviceType.RequireNonNull();
    return new AttributeTypeData(attribute, serviceType);
  }

  private static bool IsBaseOfAny(this Type type, IReadOnlyCollection<Type> baseTypes) {
    return baseTypes.Any(type.IsBaseOf);
  }

  private static bool IsInvalidType(Type type, IEnumerable<Type> excludedTypes) {
    foreach (var excludedType in excludedTypes) {
      if (type == excludedType) {
        return true;
      }

      if (excludedType.IsGenericTypeDefinition && type.IsGenericType &&
          type.GetGenericTypeDefinition() == excludedType) {
        return true;
      }
    }

    return false;
  }
}