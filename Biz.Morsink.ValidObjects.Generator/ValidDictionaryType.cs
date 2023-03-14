using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

class ValidDictionaryType : IValidType
{
    public ValidDictionaryType(INamedTypeSymbol type)
    {
        Type = type;
    }
    public INamedTypeSymbol Type { get; }
    public INamedTypeSymbol DecoratorType => Type.OriginalDefinition;
    public IValidType KeyType => IValidType.Create(Type.TypeArguments[0]);
    public IValidType ValueType => IValidType.Create(Type.TypeArguments[1]);
    public string RawTypeName => $"{DecoratorType.ContainingNamespace}.{DecoratorType.Name}<{KeyType.RawTypeName}, {ValueType.RawTypeName}>";
    public string TypeName => Type.ToDisplayString();
    public bool IsValidType => ValueType.IsValidType;
    public bool IsComplexValidType => false;
    public bool IsCollection => false;
    public bool IsDictionary => true;
    public Type? CollectionType => typeof(IImmutableDictionary<,>);
    public bool IsUnderlyingTypePrimitive => ValueType.IsUnderlyingTypePrimitive;
    public string? Constraint => null;

    public string DefaultValueAssignment => DecoratorType.Name == nameof(IImmutableDictionary<object,object>)
        ? $"System.Collections.Immutable.ImmutableDictionary<{KeyType.RawTypeName}, {ValueType.RawTypeName}>.Empty"
        : $"{DecoratorType.ContainingNamespace}.{DecoratorType.Name}<{KeyType.RawTypeName}, {ValueType.RawTypeName}>.Empty";
    public string ObjectValidator => DecoratorType.ContainingNamespace.ToString() == "System.Collections.Immutable"
        ? KeyType.IsValidType
            ? DecoratorType.Name switch
            {
                nameof(IImmutableDictionary<object, object>) => $"{ValueType.ObjectValidator}.ToDictionaryValidator({KeyType.ObjectValidator})",
                nameof(ImmutableDictionary<object, object>) => $"{ValueType.ObjectValidator}.ToHashDictionaryValidator({KeyType.ObjectValidator})",
                nameof(ImmutableSortedDictionary<object, object>) => $"{ValueType.ObjectValidator}.ToSortedDictionaryValidator({KeyType.ObjectValidator})",
                _ => "ERROR"
            }
            : DecoratorType.Name switch
            {
                nameof(IImmutableDictionary<object, object>) => $"{ValueType.ObjectValidator}.ToDictionaryValidator<{KeyType.TypeName}, {ValueType.TypeName}, {ValueType.RawTypeName}>()",
                nameof(ImmutableDictionary<object, object>) => $"{ValueType.ObjectValidator}.ToHashDictionaryValidator<{KeyType.TypeName}, {ValueType.TypeName}, {ValueType.RawTypeName}>()",
                nameof(ImmutableSortedDictionary<object, object>) => $"{ValueType.ObjectValidator}.ToSortedDictionaryValidator<{KeyType.TypeName}, {ValueType.TypeName}, {ValueType.RawTypeName}>()",
                _ => "ERROR"
            }
        : "ERROR";
    public string GetTryCreate(string name)
        => $"{ObjectValidator}.TryCreate({name}).Prefix(nameof({name}))";
    public string GetGetDto(string name)
        => IsValidType ? $"{ObjectValidator}.GetDto({name})" : name;
}
