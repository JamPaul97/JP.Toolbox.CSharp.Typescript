using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
