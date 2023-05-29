using JP.Toolbox.CSharp.Typescript.Attributes;
using System;

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
            rc.AddClass(typeof(NormalClass));
            rc.AddEnum(typeof(NumbersAsInt));
            rc.AddEnum(typeof(NumbersAsString));
            rc.AddInterface(typeof(c));
            rc.AddGenericClass(typeof(d<e>));

            rc.Build();
        }
        [TypescriptClass("A", "a")]
        private class NormalClass
        {
            public int[] ints { get; set; }
            public int b { get; set; }
            public NumbersAsInt test { get; set; }
            public NumbersAsString test2 { get; set; }
        }
        [TypescriptEnum("NumbersAsInt", "numbers-as-ints")]
        private enum NumbersAsInt
        {
            One = 1,
            Two = 2,
            Three = 3
        }
        [TypescriptEnum("NumbersAsString", "numbers-as-strings")]
        private enum NumbersAsString
        {
            [TypescriptEnumStringValue("One")]
            One = 1,
            [TypescriptEnumStringValue("Two")]
            Two = 2,
            [TypescriptEnumStringValue("Three")]
            Three = 3

        }

        [TypescriptInterface("C", "c")]
        private interface c {
            public string thisIsAString { get; set; }
            public int thisIsAnInt { get; set; }
            public bool thisIsABool { get; set; }
            public DateTime thisIsADateTime { get; set; }
            public NumbersAsInt thisIsAnEnum { get; set; }
            public NumbersAsString thisIsAnEnum2 { get; set; }
            public List<NumbersAsInt> thisIsAListOfEnums { get; set; }
            public List<NumbersAsString> thisIsAListOfEnums2 { get; set; }
            public List<string> thisIsAListOfStrings { get; set; }
            public List<int> thisIsAListOfInts { get; set; }
            public List<bool> thisIsAListOfBools { get; set; }
            public List<DateTime> thisIsAListOfDateTimes { get; set; }
            public List<NormalClass> thisIsAListOfNormalClasses { get; set; }
            public NormalClass thisIsANormalClass { get; set; }

            public int b { get; set; }
            public NumbersAsInt test { get; set; }
            public NumbersAsString test2 { get; set; }
        }
        [TypescriptClass("E", "e")]
        private class e { }
        [TypescriptGenericClass("D", "d")]
        private class d<e>
        {

        }
    }
}