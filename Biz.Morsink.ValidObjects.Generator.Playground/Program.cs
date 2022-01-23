using System.Reflection;
using System.Runtime;
using Biz.Morsink.Results;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

Compilation inputCompilation = CreateCompilation($@"
namespace GenerationTest;

using System;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Constraints;

[Generate] 
public partial class Person {{
    public static void Main() 
    {{
        Console.WriteLine($""Hoi"");
    }}
    public String Hello {{ get; }}
    public Valid<String, NotEmpty> Name {{get; }}
    public string Test {{ get; set; }}
    public Address Address {{get;}} 
}}
[Generate]
public partial class Address {{
    public Valid<string, NotEmpty> Street {{get;}}
    public Valid<string, NotEmpty> Number {{get;}}
    public Valid<string, NotEmpty> Zipcode {{get;}}
    public Valid<string, NotEmpty> City {{get;}}
    public string Country {{get;}}
}}
");

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
        .Concat(new [] { typeof(object), typeof(Console), typeof(GenerateAttribute), typeof(Result)}.Select(x => x.Assembly))
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

