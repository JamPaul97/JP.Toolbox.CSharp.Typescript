namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    /// <summary>
    /// This attribute is used to mark an Enum as a Typescript enum
    /// </summary>
    public class TypescriptEnumAttribute : TypescriptAttribute
    {
        public TypescriptEnumAttribute(string name, string filename) : base(name, filename)
        {
        }
        public string GetFilename(bool extension = false) => System.IO.Path.Combine(this.Path, this.Filename + (extension ? ".enum.ts" : ""));
    }
}
