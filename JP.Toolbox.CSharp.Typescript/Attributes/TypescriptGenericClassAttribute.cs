using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
