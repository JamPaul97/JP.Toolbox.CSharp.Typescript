using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace JP.Toolbox.CSharp.Typescript.Helpers
{
    internal class InterfaceConverter : IConverter
    {
        private List<IConverter> converters;
        private Type type;
        public Type Type
        {
            get => this.type;
            set => this.type = value;
        }
        public string Filename => this.type.GetInterfaceFilename(true);
        public string FilenameWithoutExtension => this.type.GetInterfaceFilename(false);

        public string Name => this.type.GetInterfaceName(this.converter.AddIToClassNames);
        public string Path => System.IO.Path.Combine(this.converter.MainDir, this.converter.InterfaceFolder, this.Filename);
        public string Namespace => throw new NotImplementedException();
        private Converter converter;
        public Converter Converter => this.converter;

        public string ImportStatement => $"import {{ {this.Name} }} from '../{this.converter.InterfaceFolder}/{this.FilenameWithoutExtension}.interface';";
        private string getGenericStatement(Type[] generics)
        {
            if (generics.Length == 0)
                return "";
            int currentGeneric = 0;
            List<string> identifiers = new List<string>();
            List<string> genericNames = new List<string>()
            {"T", "U", "V", "W", "X", "Y", "Z", "TT", "UU", "VV", "WW", "XX", "YY", "ZZ", "TTT", "UUU", "VVV", "WWW", "XXX", "YYY", "ZZZ"
            };
            foreach (var genericType in generics)
                identifiers.Add(genericNames[currentGeneric++]);
            if (identifiers.Distinct().Count() != identifiers.Count)
            {
                var duplicates = identifiers.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                throw new Exception($"Duplicate generic identifiers found: {string.Join(", ", duplicates)}");
            }
            return "<" + string.Join(", ", identifiers) + ">"; ;
        }
        public string Convert()
        {
            var sb = new StringBuilder();
            var sbImports = new StringBuilder();
            var _interfaces = this.type.GetInterfaces();
            var genericTypes = this.type.GetGenericArguments();
            sb.AppendLine($"export interface {type.GetInterfaceName(this.converter.AddIToInterfaceNames)}{this.getGenericStatement(genericTypes)} {(_interfaces.Length > 0 ? "implements" : "")} {string.Join(", ", _interfaces.Select(x => x.GetInterfaceName(this.converter.AddIToInterfaceNames)))} {{");
            foreach (var i in _interfaces)
                sbImports.AppendLine(this.converters.GetConverter(i).ImportStatement);
            foreach (var p in type.GetProperties())
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(long) || p.PropertyType == typeof(double) || p.PropertyType == typeof(float) || p.PropertyType == typeof(decimal))
                {
                    sb.AppendLine($"    {p.Name}: number;");
                    continue;
                }
                if (p.PropertyType == typeof(int?) || p.PropertyType == typeof(long?) || p.PropertyType == typeof(double?) || p.PropertyType == typeof(float?) || p.PropertyType == typeof(decimal?))
                {
                    sb.AppendLine($"    {p.Name}: number | null;");
                    continue;
                }
                if (p.PropertyType == typeof(string) || p.PropertyType == typeof(Guid))
                {
                    sb.AppendLine($"    {p.Name}: string;");
                    continue;
                }
                if (Nullable.GetUnderlyingType(p.PropertyType) != null && Nullable.GetUnderlyingType(p.PropertyType) == typeof(Guid))
                {
                    sb.AppendLine($"    {p.Name}: string | null;");
                    continue;
                }
                if (p.PropertyType == typeof(bool))
                {
                    sb.AppendLine($"    {p.Name}: boolean;");
                    continue;
                }
                if (p.PropertyType == typeof(bool?))
                {
                    sb.AppendLine($"    {p.Name}: boolean | null;");
                    continue;
                }
                if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTimeOffset))
                {
                    sb.AppendLine($"    {p.Name}: Date;");
                    continue;
                }
                if (Nullable.GetUnderlyingType(p.PropertyType) != null && (Nullable.GetUnderlyingType(p.PropertyType) == typeof(DateTime) || Nullable.GetUnderlyingType(p.PropertyType) == typeof(DateTimeOffset)))
                {
                    sb.AppendLine($"    {p.Name}: Date | null;");
                    continue;
                }
                if (p.IsList<string>() || p.IsList<Guid>())
                {
                    sb.AppendLine($"    {p.Name}: string[];");
                    continue;
                }
                if (p.IsList<int>() || p.IsList<long>() || p.IsList<double>() || p.IsList<float>() || p.IsList<decimal>())
                {
                    sb.AppendLine($"    {p.Name}: number[];");
                    continue;
                }
                if (p.IsList<bool>())
                {
                    sb.AppendLine($"    {p.Name}: boolean[];");
                    continue;
                }
                if (p.IsList<DateTime>() || p.IsList<DateTimeOffset>())
                {
                    sb.AppendLine($"    {p.Name}: Date[];");
                    continue;
                }
                if (p.IsList() && p.PropertyType.GenericTypeArguments.Any(t => t.IsEnum))
                {
                    var e = p.PropertyType.GenericTypeArguments.First(t => t.IsEnum);
                    if (!this.converters.HasConverter(e))
                        throw new Exception($"Enum '{e.FullName}' is not in the list of enums.");
                    sb.AppendLine($"    {p.Name}: {e.GetAttributeName()}[];");
                    sbImports.AppendLine(this.converters.GetConverter(e).ImportStatement);
                    continue;
                }
                if (p.IsList() && (p.PropertyType.GenericTypeArguments.Any(t => t.IsClass)))
                {
                    var e = p.PropertyType.GenericTypeArguments.First(t => t.IsClass);
                    if (!this.converters.HasConverter(e))
                        throw new Exception($"Class '{e.FullName}' is not in the list of classes.");
                    sb.AppendLine($"    {p.Name}: {e.GetAttributeName()}[];");
                    sbImports.AppendLine(this.converters.GetConverter(e).ImportStatement);
                    continue;
                }
                if (p.PropertyType.IsClass && !p.PropertyType.IsInterface)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine(this.converters.GetConverter(p.PropertyType).ImportStatement); continue;
                }
                if (p.PropertyType.IsInterface)
                {
                    sb.AppendLine($"    {p.Name}: {(this.converter.AddIToInterfaceNames ? "I" : "")}{p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine(this.converters.GetConverter(p.PropertyType).ImportStatement); continue;
                }
                if (p.PropertyType.IsEnum)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine(this.converters.GetConverter(p.PropertyType).ImportStatement); continue;
                }
                if (p.PropertyType.IsGenericType)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine(this.converters.GetConverter(p.PropertyType).ImportStatement); continue;
                }

                throw new Exception($"Property '{p.Name}' of type '{type.FullName}' is not supported.");
            }
            sb.AppendLine("}");
            var at = type.GetAttributeFilename();
            var sbImports2 = new StringBuilder();
            var lines = sbImports.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !sbImports2.ToString().Contains(line))
                    sbImports2.AppendLine(line);
            }
            sbImports = sbImports2;
            var data = sbImports.ToString() + "\n" + sb.ToString();
            return data;
        }
        public InterfaceConverter(Type type, List<IConverter> converters, Converter converter)
        {
            this.converters = converters;
            this.type = type;
            this.converter = converter;
        }
    }
}
