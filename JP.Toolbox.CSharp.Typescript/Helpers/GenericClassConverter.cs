using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Toolbox.CSharp.Typescript.Helpers
{
    internal class GenericClassConverter : IConverter
    {
        private List<IConverter> converters;
        private Type type;
        public Type Type
        {
            get => this.type;
            set => this.type = value;
        }
        public string Filename => this.type.GetGenericClassFilename(true);
        public string FilenameWithoutExtension => this.type.GetGenericClassFilename(false);

        public string Name => this.type.GetGenericClassName();
        public string Path => System.IO.Path.Combine(this.converter.MainDir, this.converter.InterfaceFolder, this.Filename);
        public string Namespace => throw new NotImplementedException();

        public string ImportStatement => $"import {{ {this.Name} }} from '../{this.converter.ClassFolder}/{this.FilenameWithoutExtension}.class';";
        private Converter converter;
        public Converter Converter => this.converter;
        public string Convert()
        {
            throw new NotImplementedException();
            var sb = new StringBuilder();
            var sbImports = new StringBuilder();
            var _interfaces = this.type.GetInterfaces();
            var genericType = this.type.GetGenericArguments()[0];
            if(genericType.FullName != null)
                sbImports.AppendLine(this.converters.GetConverter(genericType).ImportStatement);
            sb.AppendLine($"export class {type.GetGenericClassName()} {(_interfaces.Length > 0 ? "implements" : "")} {string.Join(", ", _interfaces.Select(x => x.GetInterfaceName(this.converter.AddIToInterfaceNames)))} <{(genericType.FullName == null ? "T" : genericType.IsClass ? genericType.GetClassName() : genericType.IsEnum ? genericType.GetEnumName() : genericType.IsInterface ? genericType.GetInterfaceName(this.converter.AddIToInterfaceNames) : throw new Exception("Unknown type"))}> {{");
            foreach (var i in _interfaces)
                sbImports.AppendLine(this.converters.GetConverter(i).ImportStatement);
            var temp = Activator.CreateInstance(this.type);
            foreach (var p in type.GetProperties())
            {
                if (p.PropertyType == typeof(int))
                {
                    int value = int.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: number = {value};");
                    continue;
                }
                if (p.PropertyType == typeof(long))
                {
                    long value = long.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: number = {value};");
                    continue;
                }
                if (p.PropertyType == typeof(double))
                {
                    double value = double.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: number = {value};");
                    continue;
                }
                if (p.PropertyType == typeof(float))
                {
                    float value = float.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: number = {value};");
                    continue;
                }
                if (p.PropertyType == typeof(decimal))
                {
                    decimal value = decimal.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: number = {value};");
                    continue;
                }
                if (p.PropertyType == typeof(string))
                {
                    string value = p.GetValue(temp).ToString();
                    sb.AppendLine($"    {p.Name}: string = '{value}';");
                    continue;
                }
                if (p.PropertyType == typeof(Guid))
                {
                    Guid value = Guid.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: string = '{value}';");
                    continue;
                }
                if (p.PropertyType == typeof(bool))
                {
                    bool value = bool.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: boolean = {value.ToString().ToLower()};");
                    continue;
                }
                if (p.PropertyType == typeof(DateTime))
                {
                    DateTime value = DateTime.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: Date = new Date('{value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}');");
                    continue;
                }
                if (p.PropertyType == typeof(DateTimeOffset))
                {
                    DateTimeOffset value = DateTimeOffset.Parse(p.GetValue(temp).ToString());
                    sb.AppendLine($"    {p.Name}: Date = new Date('{value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}');");
                    continue;
                }
                throw new Exception($"Property '{p.Name}' of type '{type.FullName}' is not supported.");
            }
            sb.AppendLine("}");
            var sbImports2 = new StringBuilder();
            var lines = sbImports.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !sbImports2.ToString().Contains(line) && !line.Contains(type.GetGenericClassFilename(false)))
                    sbImports2.AppendLine(line);
            }
            sbImports = sbImports2;
            var data = sbImports.ToString() + "\n" + sb.ToString();
            return data;
        }
        public GenericClassConverter(Type type, List<IConverter> converters, Converter converter)
        {
            this.converters = converters;
            this.type = type;
            this.converter = converter;
        }
    }
}
