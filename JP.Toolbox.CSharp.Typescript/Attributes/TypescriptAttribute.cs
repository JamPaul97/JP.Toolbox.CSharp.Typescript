namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    public class TypescriptAttribute : Attribute
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }
        public TypescriptAttribute(string name, string filename, string path = "")
        {
            Name = name;
            Filename = filename;
            Path = path;
        }
    }
}
