using JP.Toolbox.CSharp.Typescript.Attributes;
using JP.Toolbox.CSharp.Typescript.Helpers;
using System.ComponentModel.DataAnnotations;
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
        private List<IConverter> converters = new List<IConverter>();
        private readonly string dir;
        private readonly string interfacesDir;
        private readonly string enumsDir;
        private readonly string classesDir;

        public bool AddIToClassNames { get; set; } = true;
        public bool AddIToInterfaceNames { get; set; } = true;
        public string ClassFolder { get; private set; } = "clafsses";
        public string InterfaceFolder { get; private set; } = "interfaces";
        public string EnumFolder { get; private set; } = "enums";
        public string MainDir => this.dir;
        public Converter(string directory, string interfaceFolder = "interfaces", string enumFolder = "enums",string classesFolder = "classes", bool addIToClassNames = true, bool addIToInterfaceNames = true)
        {
            this.dir = directory;
            this.AddIToClassNames = addIToClassNames;
            this.AddIToInterfaceNames = addIToInterfaceNames;
            this.InterfaceFolder = interfaceFolder;
            this.EnumFolder = enumFolder;
            this.ClassFolder = classesFolder;
            this.interfacesDir = Path.Combine(this.dir, this.InterfaceFolder);
            this.enumsDir = Path.Combine(this.dir, this.EnumFolder);
            this.classesDir = Path.Combine(this.dir, this.ClassFolder);
            if (!Directory.Exists(this.dir)) Directory.CreateDirectory(this.dir);
            if (!Directory.Exists(this.interfacesDir)) Directory.CreateDirectory(this.interfacesDir);
            if (!Directory.Exists(this.enumsDir)) Directory.CreateDirectory(this.enumsDir);
            if (!Directory.Exists(this.classesDir)) Directory.CreateDirectory(this.classesDir);

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

            this.generateConverters();

            this.generateFiles();


            return;

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
            //foreach (var z in this.genericClasses)
            //{
            //    if (z.GetGenericArguments().Length == 0)
            //        throw new Exception($"Generic class '{z.FullName}' does not have a generic type");
            //    foreach (var x in z.GetGenericArguments())
            //    {
            //        if (x.IsGenericType)
            //            throw new NotImplementedException($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is a generic parameter");
            //        if (x.IsClass && !x.HasAttribute<TypescriptClassAttribute>() && !x.IsGenericTypeParameter)
            //            throw new Exception($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is a class but does not have the TypescriptClassAttribute");
            //        if (x.IsEnum && !x.HasAttribute<TypescriptEnumAttribute>())
            //            throw new Exception($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is an enum but does not have the TypescriptEnumAttribute");
            //        if (x.IsInterface && !x.HasAttribute<TypescriptInterfaceAttribute>())
            //            throw new Exception($"Generic class '{z.FullName}' has a generic type of '{x.FullName}' that is an interface but does not have the TypescriptInterfaceAttribute");
            //    }

            //}
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
                var filename = c.GetClassFilename(true);
                if (filenames.Contains(filename))
                    throw new Exception($"Generator has two classes with the same filename '{filename}'");
                filenames.Add(filename);
                var name = c.GetClassName();
                if (names.Contains(name))
                    throw new Exception($"Class '{c.FullName}' has the same name as another class. '{name}'");
                names.Add(name);
            }

            foreach (var e in this.enums)
            {
                var filename = e.GetEnumFilename(true);
                if (filenames.Contains(filename))
                    throw new Exception($"Generator has two enums with the same filename '{filename}'");
                filenames.Add(filename);
                var name = e.GetEnumName();
                if (names.Contains(name))
                    throw new Exception($"Enum '{e.FullName}' has the same name as another enum. '{name}'");
                names.Add(name);

            }
            foreach (var i in this.interfaces)
            {
                var filename = i.GetInterfaceFilename(true);
                if (filenames.Contains(filename))
                    throw new Exception($"Generator has two interfaces with the same filename '{filename}'");
                filenames.Add(filename);
                var name = i.GetInterfaceName(this.AddIToInterfaceNames);
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

        private void generateConverters()
        {
            this.converters = new List<IConverter>();
            foreach (var c in this.classes)
                this.converters.Add(new ClassConverter(c, this.converters, this));
            foreach (var e in this.enums)
                this.converters.Add(new EnumConverter(e, this.converters, this));
            foreach (var i in this.interfaces)
                this.converters.Add(new InterfaceConverter(i, this.converters, this));
            foreach (var g in this.genericClasses)
                this.converters.Add(new GenericClassConverter(g, this.converters, this));
        }

        private void generateFiles()
        {
            foreach(var x in this.converters)
            {
                var data = x.Convert();
                var filename = x.Path;
                File.WriteAllText(filename, data);
            }
        }
    }

}
