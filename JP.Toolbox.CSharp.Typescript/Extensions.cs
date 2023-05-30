using JP.Toolbox.CSharp.Typescript.Attributes;
using JP.Toolbox.CSharp.Typescript.Helpers;
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

        internal static bool HasConverter(this List<IConverter> converters, Type type)
        {
            foreach (var x in converters)
                if (x.Type == type) return true;
            return false;
        }
        internal static IConverter GetConverter(this List<IConverter> converters, Type type)
        {
            foreach (var x in converters)
                if (x.Type == type) return x;
            throw new Exception($"No converter found for type '{type.FullName}'");
        }
        public static string GetAttributeFilename(this Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(TypescriptAttribute), true).FirstOrDefault();
            if (attribute == null) throw new Exception($"Type '{type.FullName}' does not have the TypescriptAttribute");
            return ((TypescriptAttribute)attribute).Filename;
        }
        public static bool IsList(this Type type)
        => type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsList<T>(this Type type)
        => (type.IsArray && type.GetGenericTypeDefinition() == typeof(T[])) || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<T>);

        public static bool IsList(this PropertyInfo type)
        => type.PropertyType.IsArray || type.PropertyType.IsGenericType && type.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsList<T>(this PropertyInfo type)
            => (type.PropertyType.IsArray && type.PropertyType == typeof(T[])) || type.PropertyType.IsGenericType && type.PropertyType.GenericTypeArguments.Any(x => x == typeof(T));

        public static string GetClassFilename(this Type type, bool extension)
            => type.GetCustomAttribute<TypescriptClassAttribute>().GetFilename(extension);
        public static string GetClassName(this Type type)
         => type.GetCustomAttribute<TypescriptClassAttribute>().Name;
        public static string GetEnumFilename(this Type type, bool extension)
            => type.GetCustomAttribute<TypescriptEnumAttribute>().GetFilename(extension);
        public static string GetEnumName(this Type type)
            => type.GetCustomAttribute<TypescriptEnumAttribute>().Name;
        public static string GetInterfaceFilename(this Type type, bool extension)
            => type.GetCustomAttribute<TypescriptInterfaceAttribute>().GetFilename(extension);
        public static string GetInterfaceName(this Type type, bool addI)
            => $"{(addI ? "I" : "")}{type.GetCustomAttribute<TypescriptInterfaceAttribute>().Name}";

        public static string GetGenericClassFilename(this Type type, bool extension)
            => type.GetCustomAttribute<TypescriptGenericClassAttribute>().GetFilename(extension);
        public static string GetGenericClassName(this Type type)
            => type.GetCustomAttribute<TypescriptGenericClassAttribute>().Name;
    }
}
