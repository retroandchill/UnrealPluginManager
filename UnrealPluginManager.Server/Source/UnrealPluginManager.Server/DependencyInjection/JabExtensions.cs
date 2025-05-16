using System.Reflection;
using Jab;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring and injecting services into the dependency injection container
/// using a custom service provider pattern in conjunction with Jab attributes.
/// </summary>
public static class JabExtensions {

  /// <summary>
  /// Registers services into the dependency injection container based on custom attributes
  /// defined in the <c>ServerServiceProvider</c> type. It supports singleton, scoped, and
  /// transient service lifetimes and excludes types specified in the <c>JabCopyExcludeAttribute</c>.
  /// </summary>
  /// <param name="services">
  /// The <c>IServiceCollection</c> instance to which the services will be added.
  /// </param>
  /// <returns>
  /// The modified <c>IServiceCollection</c> instance with the registered services.
  /// </returns>
  public static IServiceCollection AddJabServices(this IServiceCollection services) {
    var providerType = typeof(ServerServiceProvider);
    var excludedTypes = providerType.GetCustomAttribute<JabCopyExcludeAttribute>()?.ExcludedTypes ?? [];

    services.AddSingleton(p => new ServerServiceProvider(p))
        .AddScoped(p => {
          var serverProvider = p.GetRequiredService<ServerServiceProvider>();
          return serverProvider.CreateScope();
        });

    foreach (var attribute in providerType.GetInjectionAttributes()) {
      dynamic typedAttribute = attribute;
      Type serviceType = typedAttribute.ServiceType;
      if (IsInvalidType(serviceType, excludedTypes)) {
        continue;
      }

      switch (attribute.GetType().FullName) {
        case var x when x == typeof(SingletonAttribute).FullName: {
          services.AddSingleton(serviceType, p => {
            var serverServiceProvider = p.GetRequiredService<ServerServiceProvider>();
            return serverServiceProvider.GetRequiredService(serviceType);
          });
          break;
        }
        case var x when x == typeof(ScopedAttribute).FullName: {
          services.AddScoped(serviceType, p => {
            var serverServiceProvider = p.GetRequiredService<ServerServiceProvider.Scope>();
            return serverServiceProvider.GetRequiredService(serviceType);
          });
          break;
        }
        case var x when x == typeof(TransientAttribute).FullName: {
          services.AddTransient(serviceType, p => {
            var serverServiceProvider = p.GetRequiredService<ServerServiceProvider>();
            return serverServiceProvider.GetRequiredService(serviceType);
          });
          break;
        }
      }
    }

    return services;
  }

  private static IEnumerable<Attribute> GetInjectionAttributes(this Type type) {

    return type.GetCustomAttributes()
        .Where(x => {
          var typeName = x.GetType().FullName;
          return typeName == typeof(ImportAttribute).FullName ||
                 typeName == typeof(SingletonAttribute).FullName ||
                 typeName == typeof(ScopedAttribute).FullName ||
                 typeName == typeof(TransientAttribute).FullName;
        })
        .SelectMany(x => {
          if (x.GetType().FullName != typeof(ImportAttribute).FullName) {
            return [x];
          }

          dynamic importAttribute = x;
          Type moduleType = importAttribute.ModuleType;
          return moduleType.GetInjectionAttributes();
        });

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