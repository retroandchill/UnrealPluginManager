﻿using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Meta;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// A custom implementation of the <see cref="ISchemaFilter"/> interface that modifies the OpenAPI schema
/// to handle properties or fields of types implementing <see cref="ICollection"/>. This class ensures certain
/// collection properties are annotated with default values as empty arrays or objects when decorated with
/// the <see cref="DefaultAsEmptyAttribute"/>.
/// </summary>
public class CollectionPropertyFilter : ISchemaFilter {
  /// <inheritdoc/>
  public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
    foreach (var property in schema.Properties) {
      var matchingMember = GetMemberInfo(context.Type, property.Key);
      if (matchingMember is null) {
        continue;
      }

      if (!typeof(ICollection).IsAssignableFrom(GetMemberType(matchingMember))) {
        continue;
      }

      var attribute = matchingMember.GetCustomAttribute<DefaultAsEmptyAttribute>();
      if (attribute is null) {
        continue;
      }

      property.Value.Nullable = false;
      if (typeof(IDictionary).IsAssignableFrom(GetMemberType(matchingMember))) {
        property.Value.Default = new ExplicitEmptyObject();
      } else {
        property.Value.Default = new ExplictEmptyArray();
      }
    }
  }

  private static MemberInfo? GetMemberInfo(Type type, string propertyName) {
    IEnumerable<MemberInfo> properties = type.GetProperties();
    IEnumerable<MemberInfo> fields = type.GetFields();
    return properties.Concat(fields)
        .FirstOrDefault(m => IsMatchingMember(m, propertyName));
  }

  private static bool IsMatchingMember(MemberInfo memberInfo, string propertyName) {
    var nameAttribute = memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
    if (nameAttribute is not null) {
      return nameAttribute.Name == propertyName;
    }

    return memberInfo.Name == char.ToUpper(propertyName[0]) + propertyName[1..];
  }

  private static Type GetMemberType(MemberInfo memberInfo) {
    return memberInfo switch {
        PropertyInfo property => property.PropertyType,
        FieldInfo field => field.FieldType,
        _ => throw new ArgumentException($"Unknown member type: {memberInfo.GetType().FullName}")
    };
  }
}