using System.Collections.Immutable;
using System.Net.Mime;
using Biz.Morsink.ValidObjects.Constraints;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Biz.Morsink.ValidObjects.Generator;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                (generatorContext, cancel) => GetGenerateTypes(generatorContext, cancel, (ClassDeclarationSyntax)generatorContext.Node))
            .Where(x => x is not null)
            .Select((x,_) => (ClassDeclarationSyntax) x!);
        var ttg = context.CompilationProvider.Combine(classes.Collect())
            .Select((x, cancel) => GetTypesToGenerate(x.Left, x.Right, cancel));
        
        context.RegisterSourceOutput(ttg,
            (spc, cls) =>
            {
                foreach (var c in cls)
                {
                    var source = c.GetSource();
                    spc.AddSource($"{c.ClassName}.g.cs", source);
                }
            });

    }
    private ImmutableList<TypeToGenerate> GetTypesToGenerate(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, CancellationToken cancel)
    {
        var builder = ImmutableList<TypeToGenerate>.Empty.ToBuilder();
        foreach (var cls in classes)
        {
            cancel.ThrowIfCancellationRequested();
            
            var sm = compilation.GetSemanticModel(cls.SyntaxTree);
            if (sm.GetDeclaredSymbol(cls) is not INamedTypeSymbol symbol)
                continue;
            var propsymbols
                = cls.Members.OfType<PropertyDeclarationSyntax>()
                    .Select(pds => sm.GetDeclaredSymbol(pds))
                    .Where(ps => ps is not null && ps.SetMethod is null && ps.GetMethod is not null)
                    .Cast<IPropertySymbol>()
                    .ToImmutableArray();
            
            builder.Add(new (symbol.ContainingNamespace.ToDisplayString(), symbol.Name, propsymbols));
        }
        return builder.ToImmutable();
    }
    private ClassDeclarationSyntax? GetGenerateTypes(GeneratorSyntaxContext generatorContext, CancellationToken cancel, ClassDeclarationSyntax generatorContextNode)
    {
        foreach (var attributeList in generatorContextNode.AttributeLists)
        {
            cancel.ThrowIfCancellationRequested();
            
            foreach (var attribute in attributeList.Attributes)
            {
                if (generatorContext.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "Biz.Morsink.ValidObjects.GenerateAttribute")
                    return generatorContextNode;
            }
        }
        return null;
    }
}

// [Generate] 
// public partial class Person
// {
//     private Person(Valid<string, NotEmpty> name)
//     {
//         Name = name;
//     }
//     public Valid<string, NotEmpty> Name { get; }  
// }