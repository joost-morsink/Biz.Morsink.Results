using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

public struct ValidTypes
{
    #pragma warning disable RS1024
    private readonly ConcurrentDictionary<ITypeSymbol, IValidType> _entries = new (SymbolEqualityComparer.Default);
    public IValidType Get(ITypeSymbol type)
        => _entries.GetOrAdd(type, t => IValidType.Create(t));
}
