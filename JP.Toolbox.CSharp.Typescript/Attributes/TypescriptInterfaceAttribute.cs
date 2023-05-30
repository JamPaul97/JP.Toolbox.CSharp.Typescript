namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    /// <summary>
    /// This attribute is used to mark an Interface as a Typescript interface
    /// </summary>
    public class TypescriptInterfaceAttribute : TypescriptAttribute
    {
        public TypescriptInterfaceAttribute(string name, string filename) : base(name, filename)
        {
        }

        public string GetFilename(bool extension = false) => System.IO.Path.Combine(this.Path, this.Filename + (extension ? ".interface.ts" : ""));
    }
}
