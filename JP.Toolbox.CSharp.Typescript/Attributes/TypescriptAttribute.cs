namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    public class TypescriptAttribute : Attribute
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public TypescriptAttribute(string name, string filename)
        {
            Name = name;
            Filename = filename;
        }
    }
}
