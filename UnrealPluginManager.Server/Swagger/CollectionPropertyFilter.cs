using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using LanguageExt;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Meta;

namespace UnrealPluginManager.Server.Swagger;

public class CollectionPropertyFilter : ISchemaFilter {
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
        foreach (var property in schema.Properties) {
            var matchingMember = GetMemberInfo(context.Type, property.Key);
            if (matchingMember is null) {
                throw new ArgumentException($"Property/field '{property.Key}' does not exist on type {context.Type.FullName}");
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