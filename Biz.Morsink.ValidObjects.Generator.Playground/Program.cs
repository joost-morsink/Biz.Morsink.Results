using System.Collections.Immutable;
using Biz.Morsink.Results;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Constraints;
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
using System.Collections.Immutable;
using NonEmptyString = Biz.Morsink.ValidObjects.Valid<string, Biz.Morsink.ValidObjects.Constraints.NotEmpty>;
using ZipCodeString = Biz.Morsink.ValidObjects.Valid<string, GenerationTest.DutchZipCode>;
using NaturalNumber = Biz.Morsink.ValidObjects.Valid<int, Biz.Morsink.ValidObjects.Constraints.MinValue<Biz.Morsink.ValidObjects.Math.Zero>>;

public class DutchZipCode : RegexConstraint
{
    public static DutchZipCode Instance { get; } = new ();
    public DutchZipCode() : base(""^[0-9]{4}[A-Z]{2}$"")
    {
    }
}
[Generate]
public partial class Address
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
}
[Generate]
public partial class Person
{
    public NonEmptyString FirstName { get; }
    public NonEmptyString LastName { get; }
    public NaturalNumber Age { get; }
    public ImmutableList<Address> Addresses { get; }
}
public class Program 
{
    public static void Main()
    {
        Console.WriteLine(""Hoi"");
    }
}
";
var oldcode = @"
namespace GenerationTest;

using System;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Constraints;
using System.Collections.Immutable;

[Generate] 
public partial class Person {{
    public static void Main() 
    {{
        Console.WriteLine($""Hoi"");
    }}
    public String Hello {{ get; }}
    public Valid<String, NotEmpty> Name {{get; }}
    public string Test {{ get; set; }}
    public ImmutableList<Address> Addresses {{get;}} 
}}
[Generate]
public partial class Address {{
    public Valid<string, NotEmpty> Street {{get;}}
    public Valid<string, NotEmpty> Number {{get;}}
    public Valid<string, NotEmpty> Zipcode {{get;}}
    public Valid<string, NotEmpty> City {{get;}}
    public string Country {{get;}}
}}
";

Compilation inputCompilation = CreateCompilation(code);

var generator = new Generator();

// Create the driver that will control the generation, passing in our generator
var driver = CSharpGeneratorDriver.Create(generator);

// Run the generation pass
driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);


var result = outputCompilation.Emit("test.exe");

Console.WriteLine(result);

static Compilation CreateCompilation(string source)
{
    var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "System.Runtime")
        .Concat(new [] { typeof(object), typeof(Console), typeof(GenerateAttribute), typeof(Result), typeof(ImmutableList)}.Select(x => x.Assembly))
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

