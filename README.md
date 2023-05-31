# Typescript
A tool for (transpiling, converting)  C# Classes, Interfaces, Enums to Typescript

### DISCLAIMER
This is a personal, in development project. It is not recommended to use this in any application.
## Usage
```csharp
var convereter = new Converter(outputDirectory);
convereter.AddClass(typeof(MyClass));
convereter.AddEnum(typeof(MyEnum));
convereter.AddInterface(typeof(IMyInterface));
convereter.Build();
```

## Notes 
- Only supports public classes, interfaces, enums
- If a Type is used but not added to the  convereter, the convereter will throw an exception
- In order for the Type to work with the convereter, you must use the corrent Attributes
- Classes or Interfaces with same 'Name' or 'Filename' with throw an exception
- Enums with same 'Name' or 'Filename' with throw an exception
- NOTE** Enums will be exported as 'export enum'
- As of now, the convereter support Lists, but not Dictionaries or Arrays
## Attributes
The attributes are used to give the convereter more information about the class, interface or enum.
They take two arguments that are mandatory, 'Name' and 'Filename'. The 'Name' is the name of the class, interface or enum. 

-		[TypescriptClassAttribute] : Used for classes
-		[TypescriptInterfaceAttribute] : Used for interfaces
-		[TypescriptEnumAttribute] : Used for enums
-		[TypescriptGenericClassAttribute] : Used for generic classes

There is a special Attribute for enums [TypescriptEnumStringValue].
This attribute is used to give the enum values a string value, which is not possible in C#.
The attribute takes one argument, 'Value', which is the string value of the enum value.

## Attributes exambles and outputs 

```csharp
[TypescriptEnumAttribute("MyEnum", "my-enum")]
public enum MyDifferentEnum
{
	[TypescriptEnumStringValue("Value1")]
	Value1,
	[TypescriptEnumStringValue("Value2")]
	Value2
}
```

This will be exported as <u>'my-enum.enum.ts'</u> and will look like this:
```typescript
export enum MyEnum {
	Value1 = "Value1",
	Value2 = "Value2"
}
```

The class : 
```csharp
[TypescriptClassAttribute("MyClass", "my-class")]
public class ThisNameDoesNotMater
{
	public string Name { get; set; } = "MyName";
	public int Age { get; set; } = 20;
}
```

This will be exported as <u>'my-class.class.ts'</u> and will look like this:
```typescript
export class MyClass {
	Name: string = "MyName";
	Age: number = 20;
}
```

The class : 
```csharp
[TypescriptInterfaceAttribute("MyInterfaceClass", "my-interface-class")]
public class ThisNameDoesNotMater
{
	public string Name { get; set; } = "MyName";
	public int Age { get; set; } = 20;
}
```

This will be exported as <u>'my-interface-class.interface.ts'</u> and will look like this:
```typescript
export interface MyInterfaceClass {
	Name: string;
	Age: number;
}
```

The interface : 
```csharp
[TypescriptInterfaceAttribute("MyInterface", "my-interface")]
public interface ThisNameDoesNotMater<T,Q,K>
{
	string Name { get; set; }
	int Age { get; set; }
}
```

This will be exported as <u>'my-interface.interface.ts'</u> and will look like this:
```typescript
export interface MyInterface<T, Q, K> {
	Name: string;
	Age: number;
}
```

## TODO List
	- Add custom property name Attribute
	- Add ingore property Attribute 
	- Add support for Dictionaries
	- Add support for Arrays
	- Extensive testing

## Changelog
 2023.5.30 
  - Classes now can be entered as Interfaces and as classes

 2023.5.21 
  - Removed Generic Classes support
  - Added support for Generic Interfaces
  - Fixed class output
