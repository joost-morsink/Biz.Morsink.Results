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
    public string DefaultValueAssignment => $" = {DecoratorType.ContainingNamespace}.{DecoratorType.Name}<{ElementType.RawTypeName}>.Empty;";
    public string ObjectValidator => DecoratorType.ContainingNamespace.ToString() == "System.Collections.Immutable" 
        && DecoratorType.Name == "ImmutableList"
        ? $"{ElementType.ObjectValidator}.ToListValidator()"
        : $"{ElementType.ObjectValidator}.ToSetValidator()";
    public string GetTryCreate(string name)
        => $"{ObjectValidator}.TryCreate({name}).Prefix(nameof({name}))";
    public string GetGetDto(string name)
        => $"{ObjectValidator}.GetDto({name})";
}
