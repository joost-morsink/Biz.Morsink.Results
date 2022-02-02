using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

public struct ValidTypes
{
    #pragma warning disable RS1024
    private readonly ConcurrentDictionary<ITypeSymbol, IValidType> _entries = new (SymbolEqualityComparer.Default);
    private readonly ConcurrentDictionary<ITypeSymbol, IValidType> _nullEntries = new (SymbolEqualityComparer.Default);
    public IValidType Get(ITypeSymbol type)
        => (type.NullableAnnotation == NullableAnnotation.Annotated
            ? _nullEntries : _entries).GetOrAdd(type, t => IValidType.Create(t));
}
