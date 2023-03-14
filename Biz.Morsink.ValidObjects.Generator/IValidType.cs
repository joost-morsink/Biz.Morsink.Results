using System.Collections.Immutable;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

public interface IValidType
{
    string RawTypeName { get; }
    string TypeName { get; }
    bool IsValidType { get; }
    bool IsComplexValidType { get; }
    bool IsCollection { get; }
    bool IsDictionary { get; }
    Type? CollectionType { get; }
    bool IsUnderlyingTypePrimitive { get; }
    string? Constraint { get; }
    string DefaultValueAssignment { get; }

    string ObjectValidator { get; }
    string GetTryCreate(string name);
    string GetGetDto(string name);
    
    
    public static IValidType Create(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol nts)
        {
            if (nts.NullableAnnotation == NullableAnnotation.Annotated)
                return new ValidNullableType(Create(nts.WithNullableAnnotation(NullableAnnotation.NotAnnotated)));
            if (nts.ContainingNamespace.ToString() == "System.Collections.Immutable")
            {
                if (nts.Name == nameof(ImmutableList<object>)
                    || nts.Name == nameof(ImmutableHashSet<object>)
                    || nts.Name == nameof(IImmutableSet<object>)
                    || nts.Name == nameof(ImmutableSortedSet<object>))
                    return new ValidCollectionType(nts);
                if (nts.Name == nameof(IImmutableDictionary<object, object>)
                    || nts.Name == nameof(ImmutableDictionary<object, object>)
                    || nts.Name == nameof(ImmutableSortedDictionary<object, object>))
                    return new ValidDictionaryType(nts);
            }
        }
        return new ValidType(type);
    }
}