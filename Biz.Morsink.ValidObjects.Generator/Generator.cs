using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Biz.Morsink.ValidObjects.Generator;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax {AttributeLists.Count: > 0},
                (generatorContext, cancel) => GetGenerateTypes(generatorContext, cancel,
                    (ClassDeclarationSyntax) generatorContext.Node))
            .Where(x => x is not null)
            .Select((x, _) => x!);
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

    private ImmutableList<TypeToGenerate> GetTypesToGenerate(Compilation compilation,
        ImmutableArray<Generation> classes, CancellationToken cancel)
    {
        var builder = ImmutableList<TypeToGenerate>.Empty.ToBuilder();
        foreach (var cls in classes)
        {
            cancel.ThrowIfCancellationRequested();

            var sm = compilation.GetSemanticModel(cls.Class.SyntaxTree);
            if (sm.GetDeclaredSymbol(cls.Class) is { } symbol)
            {
                var propsymbols
                    = cls.Class.Members.OfType<PropertyDeclarationSyntax>()
                        .Select(pds => sm.GetDeclaredSymbol(pds))
                        .Where(ps => ps is not null && ps.SetMethod is null && ps.GetMethod is not null)
                        .Select(ps => ps!)
                        .ToImmutableArray();

                builder.Add(new(symbol, propsymbols, cls.Options));
            }
        }

        return builder.ToImmutable();
    }

    private Generation? GetGenerateTypes(GeneratorSyntaxContext generatorContext, CancellationToken cancel,
        ClassDeclarationSyntax generatorContextNode)
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

                if (fullName == "Biz.Morsink.ValidObjects.ValidObjectAttribute")
                {
                    var test = attribute.ArgumentList?.Arguments.FirstOrDefault();
                    var ne = test?.NameEquals?.Name.ToString();
                    var val = test?.Expression.ToString();
                    return new(generatorContextNode,  new(val == "true"));
                }
            }
        }

        return null;
    }
}

public record GenerationOptions(bool CellDtos);
public record Generation(ClassDeclarationSyntax Class, GenerationOptions Options);

// [Generate] 
// public partial class Person
// {
//     private Person(Valid<string, NotEmpty> name)
//     {
//         Name = name;
//     }
//     public Valid<string, NotEmpty> Name { get; }  
// }