using JP.Toolbox.CSharp.Typescript.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JP.Toolbox.CSharp.Typescript.Helpers
{
    internal class EnumConverter : IConverter
    {
        private List<IConverter> types;
        private Type type;
        public Type Type
        {
            get => this.type;
            set => this.type = value;
        }
        public string Filename => this.type.GetEnumFilename(true);
        public string FilenameWithoutExtension => this.type.GetEnumFilename(false);

        public string Name => this.type.GetEnumName();

        public string Namespace => throw new NotImplementedException();

        public string ImportStatement => $"import {{ {this.Name} }} from '../{this.converter.EnumFolder}/{this.FilenameWithoutExtension}.enum';";
        private Converter converter;
        public Converter Converter => this.converter;

        public string Path => System.IO.Path.Combine(this.converter.MainDir, this.converter.EnumFolder, this.Filename);

        public string Convert()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"export enum {type.GetAttributeName()} {{");
            List<string> values = new List<string>();
            foreach (var e in Enum.GetValues(type))
            {
                var attribute = e.GetType().GetField(e.ToString()).GetCustomAttribute(typeof(TypescriptEnumStringValue));
                if (attribute != null)
                {
                    var value = attribute.GetType().GetProperty("Value").GetValue(attribute);
                    if (value == null) throw new Exception($"Enum '{type.FullName}' has a value of null for '{e}'");
                    if (values.Contains(value.ToString()))
                        throw new Exception($"Enum '{type.FullName}' has a duplicate value of '{value}' for '{e}'");
                    sb.Append($"    {e} = '{value}',\n");
                    values.Add(value.ToString());
                }
                else sb.Append($"    {e} = {System.Convert.ToInt32(e)},\n");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
        public EnumConverter(Type type, List<IConverter> converters, Converter converter)
        {
            this.types = converters;
            this.type = type;
            this.converter = converter;
        }
    }
}
