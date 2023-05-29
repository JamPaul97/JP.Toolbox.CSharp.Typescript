using JP.Toolbox.CSharp.Typescript.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JP.Toolbox.CSharp.Typescript
{
    internal static class Extensions
    {
        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).Length > 0;
        }
        public static string GetAttributeName(this Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(TypescriptAttribute), true).FirstOrDefault();
            if (attribute == null) 
                throw new Exception($"Type '{type.FullName}' does not have the TypescriptAttribute");
            return ((TypescriptAttribute)attribute).Name;
        }

        public static string GetAttributeFilename(this Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(TypescriptAttribute), true).FirstOrDefault();
            if (attribute == null) throw new Exception($"Type '{type.FullName}' does not have the TypescriptAttribute");
            return ((TypescriptAttribute)attribute).Filename;
        }
        public static bool IsList(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsList<T>(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<T>);

        public static bool IsList(this PropertyInfo type)
        => type.PropertyType.IsGenericType && type.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsList<T>(this PropertyInfo type)
            => type.PropertyType.IsGenericType && type.PropertyType.GenericTypeArguments.Any(x=>x == typeof(T));
    }
}
