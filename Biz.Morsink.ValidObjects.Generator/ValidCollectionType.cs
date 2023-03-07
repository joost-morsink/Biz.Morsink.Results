using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

class ValidCollectionType : IValidType
{
    public ValidCollectionType(INamedTypeSymbol type)
    {
        Type = type;
        
    }
    public INamedTypeSymbol Type { get; }
    public INamedTypeSymbol DecoratorType => Type.OriginalDefinition;
    public IValidType ElementType => IValidType.Create(Type.TypeArguments[0]);
    public string RawTypeName => $"{DecoratorType.ContainingNamespace}.{DecoratorType.Name}<{ElementType.RawTypeName}>";
    public string TypeName => Type.ToDisplayString();
    public bool IsValidType => ElementType.IsValidType;
    public bool IsComplexValidType => false;
    public string DefaultValueAssignment => DecoratorType.Name == nameof(IImmutableSet<object>)
        ? $" = System.Collections.Immutable.ImmutableHashSet<{ElementType.RawTypeName}>.Empty;"
        : $" = {DecoratorType.ContainingNamespace}.{DecoratorType.Name}<{ElementType.RawTypeName}>.Empty;";
    public string ObjectValidator => DecoratorType.ContainingNamespace.ToString() == "System.Collections.Immutable"
        ? DecoratorType.Name switch
        {
           nameof(ImmutableList<object>) => $"{ElementType.ObjectValidator}.ToListValidator()",
           nameof(IImmutableSet<object>) => $"{ElementType.ObjectValidator}.ToSetValidator()",
           nameof(ImmutableHashSet<object>) => $"{ElementType.ObjectValidator}.ToHashSetValidator()",
           nameof(ImmutableSortedSet<object>) => $"{ElementType.ObjectValidator}.ToSortedSetValidator()",
           _ => "ERROR"
        } : "ERROR";
    public string GetTryCreate(string name)
        => $"{ObjectValidator}.TryCreate({name}).Prefix(nameof({name}))";
    public string GetGetDto(string name)
        => IsValidType ? $"{ObjectValidator}.GetDto({name})" : name;
}