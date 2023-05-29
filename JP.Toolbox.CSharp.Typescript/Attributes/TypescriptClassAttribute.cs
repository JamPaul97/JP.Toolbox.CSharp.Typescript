namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    /// <summary>
    /// This attribute is used to mark a class as a Typescript class
    /// </summary>
    public class TypescriptClassAttribute : TypescriptAttribute
    {
        public TypescriptClassAttribute(string name, string filename) : base(name, filename)
        {
        }
    }
}
