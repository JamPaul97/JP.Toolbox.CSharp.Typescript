using JP.Toolbox.CSharp.Typescript.Attributes;
using System.Reflection;
using System.Text;

namespace JP.Toolbox.CSharp.Typescript
{
    public class Converter
    {
        private List<Type> classes = new List<Type>();
        private List<Type> enums = new List<Type>();
        private List<Type> interfaces = new List<Type>();
        private List<Type> genericClasses = new List<Type>();

        private readonly string dir;
        private readonly string interfacesDir;
        private readonly string enumsDir;

        public bool AddIToClassNames { get; set; } = true;
        public bool AddIToInterfaceNames { get; set; } = true;
        public string InterfaceFolder { get; private set; } = "interfaces";
        public string EnumFolder { get; private set; } = "enums";
        public Converter(string directory, string interfaceFolder = "interfaces", string enumFolder = "enums", bool addIToClassNames = true, bool addIToInterfaceNames = true)
        {
            this.dir = directory;
            this.AddIToClassNames = addIToClassNames;
            this.AddIToInterfaceNames = addIToInterfaceNames;
            this.InterfaceFolder = interfaceFolder;
            this.EnumFolder = enumFolder;
            this.interfacesDir = Path.Combine(this.dir, this.InterfaceFolder);
            this.enumsDir = Path.Combine(this.dir, this.EnumFolder);
            if (!Directory.Exists(this.dir)) Directory.CreateDirectory(this.dir);
            if (!Directory.Exists(this.interfacesDir)) Directory.CreateDirectory(this.interfacesDir);
            if (!Directory.Exists(this.enumsDir)) Directory.CreateDirectory(this.enumsDir);

        }

        public void AddClass(Type type)
        {
            if (!type.IsClass) throw new ArgumentException("Type must be a class", nameof(type));
            if (classes.Contains(type)) throw new Exception("Type already added");
            classes.Add(type);
        }
        public void AddEnum(Type type)
        {
            if (!type.IsEnum) throw new ArgumentException("Type must be an enum", nameof(type));
            if (enums.Contains(type)) throw new Exception("Type already added");
            enums.Add(type);
        }
        public void AddInterface(Type type)
        {
            if (!type.IsInterface) throw new ArgumentException("Type must be an interface", nameof(type));
            if (interfaces.Contains(type)) throw new Exception("Type already added");
            interfaces.Add(type);
        }
        public void AddGenericClass(Type type)
        {
            if (!type.IsClass) throw new ArgumentException("Type must be a class", nameof(type));
            if (!type.IsGenericType) throw new ArgumentException("Type must be a generic class", nameof(type));
            if (genericClasses.Contains(type)) throw new Exception("Type already added");
            genericClasses.Add(type);
        }

        public void Build()
        {
            // Step 1 : Check Type
            this.attributesChecks();

            this.dependenciesChecks();

            this.checkForDuplicates();

            this.generateEnums();

            this.generateClasses();

            this.generateInterfaces();

            this.generateGenericClasses();

        }

        private void attributesChecks()
        {
            //Step 1 : Check each class if the class has the TypescriptClassAttribute
            foreach (var x in this.classes)
                if (!x.HasAttribute<TypescriptClassAttribute>())
                    throw new Exception($"Class '{x.FullName}' does not have the TypescriptClassAttribute");
            //Step 2 : Check each class if the class has the TypescriptEnumAttribute
            foreach (var x in this.enums)
                if (!x.HasAttribute<TypescriptEnumAttribute>())
                    throw new Exception($"Enum '{x.FullName}' does not have the TypescriptEnumAttribute");
            //Step 3 : Check each class if the class has the TypescriptInterfaceAttribute
            foreach (var x in this.interfaces)
                if (!x.HasAttribute<TypescriptInterfaceAttribute>())
                    throw new Exception($"Interface '{x.FullName}' does not have the TypescriptInterfaceAttribute");
            //Step 4 : Check each class if the class has the TypescriptGenericClassAttribute
            foreach (var x in this.genericClasses)
                if (!x.HasAttribute<TypescriptGenericClassAttribute>())
                    throw new Exception($"Generic class '{x.FullName}' does not have the TypescriptGenericClassAttribute");
            //Step 5 : Check each generic class has a generic type
            foreach (var z in this.genericClasses)
            {
                if (z.GetGenericArguments().Length == 0)
                    throw new Exception($"Generic class '{z.FullName}' does not have a generic type");
                foreach (var x in z.GetGenericArguments())
                {
                    if (x.IsGenericType)
                        throw new NotImplementedException($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is a generic parameter");
                    if (x.IsClass && !x.HasAttribute<TypescriptClassAttribute>())
                        throw new Exception($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is a class but does not have the TypescriptClassAttribute");
                    if (x.IsEnum && !x.HasAttribute<TypescriptEnumAttribute>())
                        throw new Exception($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is an enum but does not have the TypescriptEnumAttribute");
                    if (x.IsInterface && !x.HasAttribute<TypescriptInterfaceAttribute>())
                        throw new Exception($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is an interface but does not have the TypescriptInterfaceAttribute");
                }

            }
        }

        private void dependenciesChecks()
        {
            //Step 1 : For each class go throught the properties and check if the property type is in the list of classes
            foreach (var c in this.classes)
                checkDependenciesProperties(c);

            //Step 2 : Foreach interface go throught the properties and check if the property type is in the list of classes
            foreach (var i in this.interfaces)
                checkDependenciesProperties(i);

            //Step 3 : Foreach generic class go throught the properties and check if the property type is in the list of classes
            foreach (var g in this.genericClasses)
                checkDependenciesProperties(g);
        }

        private void checkDependenciesProperties(Type c)
        {
            foreach (var p in c.GetProperties())
            {
                if (p.PropertyType.IsPrimitive) continue;
                if (p.PropertyType == typeof(string)) continue;
                if (p.IsList()) continue;
                if (p.PropertyType.IsClass)
                    if (!this.classes.Contains(p.PropertyType))
                        throw new Exception($"Class '{c.FullName}' has a property of type '{p.PropertyType.FullName}' that is not in the list of classes");
                if (p.PropertyType.IsEnum)
                    if (!this.enums.Contains(p.PropertyType))
                        throw new Exception($"Class '{c.FullName}' has a property of type '{p.PropertyType.FullName}' that is not in the list of enums");
                if (p.PropertyType.IsInterface)
                    if (!this.interfaces.Contains(p.PropertyType))
                        throw new Exception($"Class '{c.FullName}' has a property of type '{p.PropertyType.FullName}' that is not in the list of interfaces");
                if (p.PropertyType.IsGenericType)
                    if (!this.genericClasses.Contains(p.PropertyType))
                        throw new Exception($"Class '{c.FullName}' has a property of type '{p.PropertyType.FullName}' that is not in the list of generic classes");
            }
        }

        private void checkForDuplicates()
        {
            List<string> filenames = new List<string>();
            List<string> names = new List<string>();
            foreach (var c in this.classes)
            {
                var name = $"{c.GetAttributeName()}.class.ts";
                if (filenames.Contains(name))
                    throw new Exception($"Class '{c.FullName}' has the same filename as another class. '{name}'");
                filenames.Add(name);
                name = $"I{c.GetAttributeFilename()}";
                if (names.Contains(name))
                    throw new Exception($"Class '{c.FullName}' has the same name as another class. '{name}'");
                names.Add(name);
            }

            foreach (var e in this.enums)
            {
                var name = $"{e.GetAttributeName()}.enum.ts";
                if (filenames.Contains(name))
                    throw new Exception($"Enum '{e.FullName}' has the same filename as another enum. '{name}'");
                filenames.Add(name);
                name = $"{e.GetAttributeFilename()}";
                if (names.Contains(name))
                    throw new Exception($"Enum '{e.FullName}' has the same name as another enum. '{name}'");
                names.Add(name);

            }
            foreach (var i in this.interfaces)
            {
                var name = $"I{i.GetAttributeName()}.interface.ts";
                if (filenames.Contains(name))
                    throw new Exception($"Interface '{i.FullName}' has the same filename as another interace. '{name}'");
                filenames.Add(name);
                name = $"I{i.GetAttributeFilename()}";
                if (names.Contains(name))
                    throw new Exception($"Interface '{i.FullName}' has the same name as another interace. '{name}'");
                names.Add(name);
            }
            foreach (var g in this.genericClasses)
            {
                var name = $"{g.GetAttributeName()}.class.ts";
                if (filenames.Contains(name))
                    throw new Exception($"Generic class '{g.FullName}' has the same filename as another interace. '{name}'");
                filenames.Add(name);
                name = $"I{g.GetAttributeFilename()}";
                if (names.Contains(name))
                    throw new Exception($"Generic class '{g.FullName}' has the same name as another interace. '{name}'");
                names.Add(name);
            }
        }


        private void generateEnums()
        {
            foreach (var e in this.enums)
                this.generateEnum(e, true);
        }

        private string generateEnum(Type type, bool save = true)
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
            if (save)
            {
                var at = type.GetAttributeFilename();
                var filename = Path.Combine(this.enumsDir, $"{at}.enum.ts");
                File.WriteAllText(filename, sb.ToString());
            }
            return sb.ToString();
        }
        private void generateClasses()
        {
            foreach (var c in this.classes)
                this.generateClass(c, this.AddIToClassNames, true);
        }

        private void generateInterfaces()
        {
            foreach (var i in this.interfaces)
                this.generateClass(i, this.AddIToInterfaceNames, true);
        }

        private string generateClass(Type type, bool AddIToClassNames, bool save = true)
        {
            var sb = new StringBuilder();
            var sbImports = new StringBuilder();
            sb.AppendLine($"export interface {(AddIToClassNames ? "I" : "")}{type.GetAttributeName()} {{");
            foreach (var p in type.GetProperties())
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(long) || p.PropertyType == typeof(double) || p.PropertyType == typeof(float) || p.PropertyType == typeof(decimal))
                {
                    sb.AppendLine($"    {p.Name}: number;");
                    continue;
                }
                if (p.PropertyType == typeof(string) || p.PropertyType == typeof(Guid))
                {
                    sb.AppendLine($"    {p.Name}: string;");
                    continue;
                }
                if (p.PropertyType == typeof(bool))
                {
                    sb.AppendLine($"    {p.Name}: boolean;");
                    continue;
                }
                if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTimeOffset))
                {
                    sb.AppendLine($"    {p.Name}: Date;");
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
                    if (!this.enums.Contains(e))
                        throw new Exception($"Enum '{e.FullName}' is not in the list of enums.");
                    sb.AppendLine($"    {p.Name}: {e.GetAttributeName()}[];");
                    sbImports.AppendLine($"import {{ {e.GetAttributeName()} }} from '../{this.EnumFolder}/{e.GetAttributeFilename()}.enum';"); continue;
                }
                if (p.IsList() && p.PropertyType.GenericTypeArguments.Any(t => t.IsClass))
                {
                    var e = p.PropertyType.GenericTypeArguments.First(t => t.IsClass);
                    if (!this.classes.Contains(e))
                        throw new Exception($"Class '{e.FullName}' is not in the list of classes.");
                    sb.AppendLine($"    {p.Name}: {(AddIToClassNames ? "I" : "")}{e.GetAttributeName()}[];");
                    sbImports.AppendLine($"import {{ {(AddIToClassNames ? "I" : "")}{e.GetAttributeName()} }} from './{(AddIToClassNames ? "I" : "")}{e.GetAttributeFilename()}.interface';"); continue;
                }
                if (p.PropertyType.IsClass)
                {
                    sb.AppendLine($"    {p.Name}: {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()} }} from './{(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeFilename()}.interface';"); continue;
                }
                if (p.PropertyType.IsInterface)
                {
                    sb.AppendLine($"    {p.Name}: {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()} }} from './{(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeFilename()}.interface';"); continue;
                }
                if (p.PropertyType.IsEnum)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {p.PropertyType.GetAttributeName()} }} from '../{this.EnumFolder}/{p.PropertyType.GetAttributeFilename()}.enum';"); continue;
                }
                if (p.PropertyType.IsGenericType)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {p.PropertyType.GetAttributeName()} }} from './{p.PropertyType.GetAttributeFilename()}.interface';"); continue;
                }

                throw new Exception($"Property '{p.Name}' of type '{type.FullName}' is not supported.");
            }
            sb.AppendLine("}");
            var at = type.GetAttributeFilename();
            var sbImports2 = new StringBuilder();
            var lines = sbImports.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !sbImports2.ToString().Contains(line) && !line.Contains($"{(AddIToClassNames ? "I" : "")}{at}.interface"))
                    sbImports2.AppendLine(line);
            }
            sbImports = sbImports2;
            var data = sbImports.ToString() + "\n" + sb.ToString();
            if (save)
            {
                var filename = Path.Combine(this.interfacesDir, $"{(AddIToClassNames ? "I" : "")}{at}.interface.ts");
                File.WriteAllText(filename, data);
            }
            return data;
        }
        private void generateGenericClasses()
        {
            foreach (var c in this.genericClasses)
                this.generateGenericClass(c, true);
        }
        private string generateGenericClass(Type type, bool save = true)
        {
            var sb = new StringBuilder();
            var sbImports = new StringBuilder();
            sb.AppendLine($"export interface {(AddIToClassNames ? "I" : "")}{type.GetAttributeName()}<T> {{");
            foreach (var p in type.GetProperties())
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(long) || p.PropertyType == typeof(double) || p.PropertyType == typeof(float) || p.PropertyType == typeof(decimal))
                {
                    sb.AppendLine($"    {p.Name}: number;");
                    continue;
                }
                if (p.PropertyType == typeof(string) || p.PropertyType == typeof(Guid))
                {
                    sb.AppendLine($"    {p.Name}: string;");
                    continue;
                }
                if (p.PropertyType == typeof(bool))
                {
                    sb.AppendLine($"    {p.Name}: boolean;");
                    continue;
                }
                if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTimeOffset))
                {
                    sb.AppendLine($"    {p.Name}: Date;");
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
                    if (!this.enums.Contains(e))
                        throw new Exception($"Enum '{e.FullName}' is not in the list of enums.");
                    sb.AppendLine($"    {p.Name}: {e.GetAttributeName()}[];");
                    sbImports.AppendLine($"import {{ {e.GetAttributeName()} }} from './{e.GetAttributeFilename()}.enum';"); continue;
                }
                if (p.IsList() && p.PropertyType.GenericTypeArguments.Any(t => t.IsClass))
                {
                    var e = p.PropertyType.GenericTypeArguments.First(t => t.IsClass);
                    if (!this.classes.Contains(e))
                        throw new Exception($"Class '{e.FullName}' is not in the list of classes.");
                    sb.AppendLine($"    {p.Name}: {(AddIToClassNames ? "I" : "")}{e.GetAttributeName()}[];");
                    sbImports.AppendLine($"import {{ {(AddIToClassNames ? "I" : "")}{e.GetAttributeName()} }} from './{(AddIToClassNames ? "I" : "")}{e.GetAttributeFilename()}.interface';"); continue;
                }
                if (p.PropertyType.IsClass)
                {
                    sb.AppendLine($"    {p.Name}: {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()} }} from './{(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeFilename()}.interface';"); continue;
                }
                if (p.PropertyType.IsInterface)
                {
                    sb.AppendLine($"    {p.Name}: {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeName()} }} from './{(AddIToClassNames ? "I" : "")}{p.PropertyType.GetAttributeFilename()}.interface';"); continue;
                }
                if (p.PropertyType.IsEnum)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {p.PropertyType.GetAttributeName()} }} from '../{this.EnumFolder}/{p.PropertyType.GetAttributeFilename()}.enum';"); continue;
                }
                if (p.PropertyType.IsGenericType)
                {
                    sb.AppendLine($"    {p.Name}: {p.PropertyType.GetAttributeName()};");
                    sbImports.AppendLine($"import {{ {p.PropertyType.GetAttributeFilename()} }} from './{p.PropertyType.GetAttributeFilename()}.interface';"); continue;
                }

                throw new Exception($"Property '{p.Name}' of type '{type.FullName}' is not supported.");
            }
            sb.AppendLine("}");
            var at = type.GetAttributeFilename();
            var sbImports2 = new StringBuilder();
            var lines = sbImports.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !sbImports2.ToString().Contains(line) && !line.Contains($"{(AddIToClassNames ? "I" : "")}{at}.interface"))
                    sbImports2.AppendLine(line);
            }
            sbImports = sbImports2;

            var data = sbImports.ToString() + "\n" + sb.ToString();
            if (save)
            {

                var filename = Path.Combine(this.interfacesDir, $"{(AddIToClassNames ? "I" : "")}{at}.interface.ts");
                File.WriteAllText(filename, data);
            }
            return data;
        }

    }

}
