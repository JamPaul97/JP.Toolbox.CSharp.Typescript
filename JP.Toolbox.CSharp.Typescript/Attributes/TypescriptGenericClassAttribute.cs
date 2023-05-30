namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    /// <summary>
    /// This attribute is used to mark a class as a Typescript generic class
    /// </summary>
    public class TypescriptGenericClassAttribute : TypescriptAttribute
    {
        public TypescriptGenericClassAttribute(string name, string filename) : base(name, filename)
        {
        }

        public string GetFilename(bool extension = false) => System.IO.Path.Combine(this.Path, this.Filename + (extension ? ".class.ts" : ""));
    }
}
