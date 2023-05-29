using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Toolbox.CSharp.Typescript.Attributes
{
    public class TypescriptEnumStringValue : Attribute
    {
        public string Value { get; set; }
        public TypescriptEnumStringValue(string value)
        {
            Value = value;
        }
    }
}
