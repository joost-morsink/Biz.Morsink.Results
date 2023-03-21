using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#pragma warning disable CS0219

var code =
    @"
namespace GenerationTest;

using System;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Constraints;
using System.Collections.Generic;
using System.Collections.Immutable;
using NonEmptyString = Biz.Morsink.ValidObjects.Valid<string, Biz.Morsink.ValidObjects.Constraints.NotEmpty>;
using ZipCodeString = Biz.Morsink.ValidObjects.Valid<string, GenerationTest.DutchZipCode>;
using NaturalNumber = Biz.Morsink.ValidObjects.Valid<int, Biz.Morsink.ValidObjects.Constraints.MinValue<Biz.Morsink.ValidObjects.Math.Zero>>;

#nullable enable

public class DutchZipCode : RegexConstraint
{
    public static DutchZipCode Instance { get; } = new ();
    public DutchZipCode() : base(""^[0-9]{4}[A-Z]{2}$"")
    {
    }
}
public class Identifier : RegexConstraint
{
    public static Identifier Instance { get; } = new ();
    public Identifier() : base(""^[A-Za-z_][A-Za-z_0-9]*"")
    {
    }
}
[ValidObject(Mutable=true)]
public partial class Address
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
}
[ValidObject(Mutable=true)]
public partial class Person
{
    public NonEmptyString FirstName { get; }
    public NonEmptyString LastName { get; }
    public NaturalNumber Age { get; }
    public Address MainAddress { get; }
    public ImmutableList<Address> Addresses { get; }
   // public NaturalNumber LuckyNumber { get; }
   // public NaturalNumber TestNumber { get; }
    public IImmutableSet<Valid<string, Identifier>> Tags { get; }

    [ValidationMethod]
    private IEnumerable<string> Check()
    {
        if (!Equals(FirstName, LastName))
            yield return ""First and lastnames should be different."";
    }
    [ValidationMethod(""Age should be less than 100"")]
    private bool CheckAge()
        => Age.Value < 100;
}
[ValidObject]
public partial class DictContainer
{
    public ImmutableDictionary<string, string> Regular { get; }
    public IImmutableDictionary<Valid<string,Identifier>, Valid<string,NotEmpty>> Both { get; }
    public ImmutableSortedDictionary<string, Valid<string, NotEmpty>> Value { get; }
}
public class Program 
{
    public static void Main()
    {
        Console.WriteLine(""Hoi"");
    }
}
";


Compilation inputCompilation = CreateCompilation(code);

var generator = new Generator();

// Create the driver that will control the generation, passing in our generator
var driver = CSharpGeneratorDriver.Create(generator);

// Run the generation pass
driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out _);


var result = outputCompilation.Emit("test.exe");
var warnAndErrors = result.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning);
Console.WriteLine(result);

static Compilation CreateCompilation(string source)
{
    var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name is "System.Runtime")
        .Concat(new [] { typeof(object), typeof(Console), typeof(ValidObjectAttribute), typeof(Result), typeof(ImmutableList), typeof(INotifyPropertyChanged), typeof(INotifyCollectionChanged), typeof(CallerMemberNameAttribute), typeof(Enumerable)}.Select(x => x.Assembly))
        .Select(a => a.Location)
        .Distinct()
        .Select(x => MetadataReference.CreateFromFile(x))
        .ToArray();
    var opts = new CSharpCompilationOptions(OutputKind.ConsoleApplication);



    return CSharpCompilation.Create("compilation",
        new[] { CSharpSyntaxTree.ParseText(source) },
        refs,
        opts
        );
    
}

