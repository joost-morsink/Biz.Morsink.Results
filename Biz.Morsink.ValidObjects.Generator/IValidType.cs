using System.Collections.Immutable;
using System.Data;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Biz.Morsink.ValidObjects.Generator;

public interface IValidType
{
    string RawTypeName { get; }
    string TypeName { get; }
    bool IsValidType { get; }
    bool IsComplexValidType { get; }
    IValidType? ElementType { get; }
    bool IsCollection { get; }
    bool IsDictionary { get; }
    Type? CollectionType { get; }

    CollectionKind? CollectionKind => CollectionType?.Name switch
    {
        null => null,
        $"{nameof(ImmutableList<object>)}`1" => Biz.Morsink.ValidObjects.Generator.CollectionKind.List,
        $"{nameof(IImmutableList<object>)}`1" => Biz.Morsink.ValidObjects.Generator.CollectionKind.List,
        $"{nameof(ImmutableDictionary<object,object>)}`2" => Biz.Morsink.ValidObjects.Generator.CollectionKind.Dictionary,
        $"{nameof(IImmutableSet<object>)}`1" => Biz.Morsink.ValidObjects.Generator.CollectionKind.Set,
        $"{nameof(ImmutableHashSet)}`1" => Biz.Morsink.ValidObjects.Generator.CollectionKind.Set,
        $"{nameof(ImmutableSortedSet<object>)}`1" => Biz.Morsink.ValidObjects.Generator.CollectionKind.Set,
        _ => null
    };
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