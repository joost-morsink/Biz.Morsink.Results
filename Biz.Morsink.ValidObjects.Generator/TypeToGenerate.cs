using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Biz.Morsink.Results.Errors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
namespace Biz.Morsink.ValidObjects.Generator;

[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public record TypeToGenerate(INamedTypeSymbol Symbol, ImmutableArray<IPropertySymbol> PropertySymbols, GenerationOptions Options)
{
    public string Namespace => Symbol.ContainingNamespace.ToDisplayString();
    public string ClassName => Symbol.Name;
    public ValidTypes ValidTypes { get; } = new ();
    public ImmutableArray<(IMethodSymbol, string?)> ValidationMethods => Symbol.GetMembers()
        .OfType<IMethodSymbol>()
        .SelectMany(m => m.GetAttributes()
            .Where(a => a.AttributeClass?.ToDisplayString() == "Biz.Morsink.ValidObjects.ValidationMethodAttribute")
            .Select(a => (m, a.ConstructorArguments
                .Select(tc => tc.Value?.ToString())
                .FirstOrDefault())))
        .ToImmutableArray();

    public IValidType GetValidType(int index)
        => GetValidType(PropertySymbols[index].Type);
    public IValidType GetValidType(ITypeSymbol type)
        => ValidTypes.Get(type);
    public IEnumerable<(IPropertySymbol property, IValidType validType)> GetProperties()
        => PropertySymbols.AsEnumerable().Select(ps => (ps, GetValidType(ps.Type)));
    public string GetSource()
        => IValidType.Create(Symbol).IsComplexValidType
            ? GetComplexSource()
            : GetSimpleSource();
    public string GetSimpleSource()
        => @$"
namespace {Namespace};
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Biz.Morsink.ValidObjects;
#nullable enable

partial class {ClassName} : IValidObject<{ClassName}, {ClassName}.Dto>, IHasStaticValidator<{ClassName}, {ClassName}.Dto>
{{
{GetValidator()}
{GetConstructor()}
{GetGetDto()}
{GetDto()}
}}
";
    public string GetComplexSource()
        => @$"
namespace {Namespace};
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Biz.Morsink.ValidObjects;

#nullable enable

partial class {ClassName} : IComplexValidObject<{ClassName}, {ClassName}.Intermediate, {ClassName}.Dto>, IHasStaticValidator<{ClassName}, {ClassName}.Dto>
{{
{GetValidator()}
{GetConstructor()}
{GetGetDto()}
{GetGetIntermediate()}
{GetComplexDto()}
{GetIntermediateDto()}
}}
";
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
    public string GetGetIntermediate()
        => $@"
    public Intermediate GetIntermediate()
        => new Intermediate({string.Join(", ", GetProperties().Select(ps => ps.property.Name))});";
    public string GetDto()
        => $@"
    public partial record Dto : IDto<{ClassName}, Dto>
    {{
{string.Join(Environment.NewLine, GetProperties().Select(ps => $"        public {ps.validType.RawTypeName} {ps.property.Name} {{get; init;}}{ps.validType.DefaultValueAssignment}"))}
{GetTryCreate()}
    }}";
    public string GetComplexDto()
        => $@"
    public partial record Dto : IComplexDto<{ClassName}, Intermediate, Dto>
    {{
{string.Join(Environment.NewLine, GetProperties().Select(ps => $"        public {ps.validType.RawTypeName} {ps.property.Name} {{get; init;}}{ps.validType.DefaultValueAssignment}"))}
{GetTryCreate("TryCreateIntermediate", "Intermediate")}
        public Result<{ClassName}, ErrorList> TryCreate()
            => TryCreateIntermediate().Bind(x => x.TryCreate());
    }}";
    public string GetTryCreate(string methodName = "TryCreate", string? className = null)
    {
        className ??= ClassName;
        var validProps = GetProperties().Where(p => p.validType.IsValidType).ToList();
        if (validProps.Count == 1)
            return $@"
        public Result<{ClassName}, ErrorList> {methodName}()
            => {validProps[0].validType.GetTryCreate(validProps[0].property.Name)}.Select(x => new {className}({string.Join(", ", GetProperties().Select(p => p.validType.IsValidType ? "x" : p.property.Name))}));";

        if (validProps.Count > 1)
        {
            var dict = validProps.Select(vp => vp.property.Name).Zip(letters, (n, l) => (n, l)).ToDictionary(x => x.n, x => x.l);
            return $@"
        public Result<{className}, ErrorList> {methodName}()
            =>
                ({string.Join("," + Environment.NewLine + "                ",
                    validProps.Select(ps => ValidTypes.Get(ps.property.Type).GetTryCreate(ps.property.Name)))}
                ).Apply(({string.Join(", ", Enumerable.Range(0, validProps.Count).Select(x => letters[x]))})
                => new {className}({string.Join(", ", PropertySymbols.Select(ps => dict.TryGetValue(ps.Name, out var lttr) ? lttr.ToString() : ps.Name))}));";
        }
        return "ERROR";
    }
    public string GetValidator()
        => $"    public static IObjectValidator<{ClassName}, Dto> Validator {{ get; }} = ObjectValidator.For<{ClassName}, Dto>();";
    private static string letters = "tuvwxyz";

    public string GetIntermediateDto()
        => $@"
    public partial record Intermediate({string.Join(", ", GetProperties().Select(ps => $"{ps.validType.TypeName} {ps.property.Name}"))})
        : IIntermediateDto<{ClassName}, Dto> 
    {{
{GetGetDto()}
{GetIntermediateTryCreate()}
    }}";

    public string GetIntermediateTryCreate()
    {
        var validationMethods = ValidationMethods;
        return $@"
        public Result<{ClassName}, ErrorList> TryCreate()
        {{
            var res = new {ClassName}({string.Join(", ", PropertySymbols.Select(ps => ps.Name))});
            return res.{CallValidationMethod(validationMethods[0].Item1, validationMethods[0].Item2)}{string.Concat(validationMethods.Skip(1).Select(m => $".Concat(res.{CallValidationMethod(m.Item1, m.Item2)})"))}
            .IfValidThen(() => res);
        }}";
    }
    private bool IsEnumerableOf(INamedTypeSymbol symbol, string @namespace, string type)
        => (symbol.ContainingNamespace.ToDisplayString() == "System.Collections.Generic"
                && symbol.Name == nameof(IEnumerable<object>)
                || symbol.Interfaces.Any(i => IsEnumerableOf(i, @namespace, type)))
            && symbol.TypeArguments.Length == 1
            && symbol.TypeArguments[0].ContainingNamespace.ToDisplayString() == @Namespace
            && symbol.TypeArguments[0].Name == type; 
    public string CallValidationMethod(ISymbol method, string? message)
    {
        if (method is IMethodSymbol ms)
        {
            if (ms.ReturnType is INamedTypeSymbol rt
                && IsEnumerableOf(rt, "Biz.Morsink.Results.Errors", "Error")
                || ms.ReturnType.ContainingNamespace.ToDisplayString() == "Biz.Morsink.Results.Errors"
                && ms.ReturnType.Name == nameof(ErrorList))
                return $"{method.Name}()";
        }
        return $"{method.Name}().ToErrorList({(message!=null ? @$"@""{message.Replace("\"","\"\"")}""" : "")})";
    }
}