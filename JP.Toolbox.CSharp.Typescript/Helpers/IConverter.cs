using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Toolbox.CSharp.Typescript.Helpers
{
    internal interface IConverter
    {
        string Convert();
        Type Type { get; }
        string Filename { get; }
        string FilenameWithoutExtension { get; }
        string Path { get; }
        string Name { get; }
        string Namespace { get; }
        string ImportStatement { get; }
        Converter Converter { get; }
        
    }
}
