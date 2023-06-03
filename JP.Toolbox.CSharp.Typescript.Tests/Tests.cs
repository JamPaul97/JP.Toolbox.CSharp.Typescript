using JP.Toolbox.CSharp.Typescript.Attributes;
using System;
using static JP.Toolbox.CSharp.Typescript.Tests.Tests;

namespace JP.Toolbox.CSharp.Typescript.Tests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test");
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var rc = new Converter(dir);

            rc.AddInterface(typeof(Settings));
            rc.Build();
        }
        [TypescriptInterfaceAttribute("Settings", "settings")]
        public class Settings : Settings2
        {
            public int port { get; set; } = 50;
            public string host { get; set; } = "localhost";

        }
        [TypescriptInterfaceAttribute("Settings2", "settings")]
        public class Settings2
        {
            public int port2 { get; set; } = 50;
            public string host2 { get; set; } = "localhost";

        }
    }
}