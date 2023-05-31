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
            rc.AddClass(typeof(Settings));
            rc.Build();
        }
        [TypescriptInterfaceAttribute("Settings", "settings")]
        [TypescriptClassAttribute("Settings", "settings")]
        public class Settings
        {
            public int port { get; set; } = 50;
            public string host { get; set; } = "localhost";

        }
        public class aa
        {
            public List<string> test;
        }
        [TypescriptInterface("BBA", "aBB")]
        public interface bb<T,Q>
        {
            public string[] test { get; set; }
            public int[] test2 { get; set; }
            public c test3 { get; set; }
        }
        [TypescriptInterface("A", "a")]
        public interface NormalClass
        {
            public int b { get; set; }
            public c Getc { get; set; }
            public NumbersAsInt test { get; set; }
            public NumbersAsString test2 { get; set; }
        }
        [TypescriptEnum("NumbersAsInt", "numbers-as-ints")]
        public enum NumbersAsInt
        {
            One = 1,
            Two = 2,
            Three = 3
        }
        [TypescriptEnum("NumbersAsString", "numbers-as-strings")]
        public enum NumbersAsString
        {
            [TypescriptEnumStringValue("One")]
            One = 1,
            [TypescriptEnumStringValue("Two")]
            Two = 2,
            [TypescriptEnumStringValue("Three")]
            Three = 3

        }

        [TypescriptInterface("C", "c")]
        public interface c {
            
            public int b { get; set; }
            public NumbersAsInt test { get; set; }
            public NumbersAsString test2 { get; set; }
        }
        [TypescriptClass("E", "e")]
        public class e { }
        [TypescriptGenericClass("D", "d")]
        public class d<T>
        {

        }
    }
}