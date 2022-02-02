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
            if (nts.ContainingNamespace.ToString() == "System.Collections.Immutable"
                && (nts.Name == nameof(ImmutableList<object>) 
                    || nts.Name == nameof(ImmutableHashSet<object>) 
                    || nts.Name == nameof(IImmutableSet<object>)
                    || nts.Name == nameof(ImmutableSortedSet<object>)))
                return new ValidCollectionType(nts);
        }
        return new ValidType(type);
    }
}