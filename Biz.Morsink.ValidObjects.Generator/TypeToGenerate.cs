using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

public record TypeToGenerate(string Namespace, string ClassName, ImmutableArray<IPropertySymbol> PropertySymbols)
{
    public ValidTypes ValidTypes { get; } = new ValidTypes();
    public ValidType GetValidType(int index)
        => GetValidType(PropertySymbols[index].Type);
    public ValidType GetValidType(ITypeSymbol type)
        => ValidTypes.Get(type);
    public IEnumerable<(IPropertySymbol property, ValidType validType)> GetProperties()
        => PropertySymbols.AsEnumerable().Select(ps => (ps, GetValidType(ps.Type)));
    public string GetConstructor()
        => @$"
    private {ClassName}({string.Join(", ", PropertySymbols.Select(ps => $"{ps.Type.ToDisplayString()} {ps.Name}"))})
    {{
{string.Join(Environment.NewLine, PropertySymbols.Select(ps => $"        this.{ps.Name} = {ps.Name};"))}
    }}
";
    public string GetGetDto()
        => $@"
    public Dto GetDto()
        => new Dto 
        {{
            {string.Join("," + Environment.NewLine + "            ",
                GetProperties().Select(ps => $"{ps.property.Name} = {ps.validType.GetGetDto(ps.property.Name)}"))}
        }};";
    public string GetDto()
        => $@"
    public partial record Dto : IDto<{ClassName},Dto>
    {{
{string.Join(Environment.NewLine, GetProperties().Select(ps => $"        public {ps.validType.RawTypeName} {ps.property.Name} {{get; init;}}{ps.validType.DefaultValueAssignment}"))}
{GetTryCreate()}
    }}";
    public string GetTryCreate()
    {
        var validProps = GetProperties().Where(p => p.validType.IsValidType).ToList();
        if (validProps.Count == 1)
            return $@"
        public Result<{ClassName}, ErrorList> TryCreate()
            => {validProps[0].validType.GetTryCreate(validProps[0].property.Name)}.Select(x => new {ClassName}({string.Join(", ", GetProperties().Select(p => p.validType.IsValidType ? "x" : p.property.Name))}));";

        if (validProps.Count > 1)
        {
            var dict = validProps.Select(vp => vp.property.Name).Zip(letters, (n, l) => (n, l)).ToDictionary(x => x.n, x => x.l);
            return $@"
        public Result<{ClassName}, ErrorList> TryCreate()
            =>
                ({string.Join("," + Environment.NewLine + "                ",
                    validProps.Select(ps => ValidTypes.Get(ps.property.Type).GetTryCreate(ps.property.Name)))}
                ).Apply(({string.Join(", ", Enumerable.Range(0, validProps.Count).Select(x => letters[x]))})
                => new {ClassName}({string.Join(", ", PropertySymbols.Select(ps => dict.TryGetValue(ps.Name, out var lttr) ? lttr.ToString() : ps.Name))}));";
        }
        return "ERROR";
    }
    private static string letters = "tuvwxyz";
}