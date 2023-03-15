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

    private string If(bool condition, string str)
        => condition ? str : "";
    private string If(bool condition, Func<string> str)
        => condition ? str() : "";
    private string IfMutable(string str)
        => Options.Mutable ? str : "";
    private string IfMutable(Func<string> str)
        => Options.Mutable ? str() : "";
    public IValidType GetValidType(int index)
        => GetValidType(PropertySymbols[index].Type);
    public IValidType GetValidType(ITypeSymbol type)
        => ValidTypes.Get(type);
    public IEnumerable<(IPropertySymbol property, IValidType validType)> GetProperties()
        => PropertySymbols.AsEnumerable().Select(ps => (ps, GetValidType(ps.Type)));

    public string GetSource()
        => $@"{(IValidType.Create(Symbol).IsComplexValidType
            ? GetComplexSource()
            : GetSimpleSource())}
#pragma warning restore CS8019";

    public string Usings()
        => @"using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using Biz.Morsink.ValidObjects;
using Biz.Morsink.ValidObjects.Mutable;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;";
    public string GetSimpleSource()
        => @$"
namespace {Namespace};
{Usings()}
#nullable enable
#pragma warning disable CS8019

partial class {ClassName} : {GetValidObjectInterface()}, IHasStaticValidator<{ClassName}, {ClassName}.Dto>
{{
{GetValidator()}
{GetConstructor()}
{GetGetDto()}
{GetDto()}
{IfMutable(GetSimpleMutableImplementation)}
}}
";
    public string GetValidObjectInterface()
        => Options.Mutable
            ? $"IValidObjectWithToMutable<{ClassName}, {ClassName}.Dto, {ClassName}.Mutable, {ClassName}.Mutable.Dto>"
            : $"IValidObject<{ClassName}, {ClassName}.Dto>";
    
    public string GetComplexSource()
        => @$"
namespace {Namespace};
{Usings()}
#nullable enable
#pragma warning disable CS8019

partial class {ClassName} : {GetComplexValidObjectInterface()}, IHasStaticValidator<{ClassName}, {ClassName}.Dto>
{{
{GetValidator()}
{GetConstructor()}
{GetGetDto()}
{GetGetIntermediate()}
{GetComplexDto()}
{GetIntermediateDto()}
{IfMutable(GetComplexMutableImplementation)}
}}
";
    public string GetComplexValidObjectInterface()
        => Options.Mutable
            ? $"IComplexValidObjectWithToMutable<{ClassName}, {ClassName}.Intermediate, {ClassName}.Dto, {ClassName}.Mutable, {ClassName}.Mutable.Dto>"
            : $"IComplexValidObject<{ClassName}, {ClassName}.Intermediate, {ClassName}.Dto>";

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
    public partial record Dto : IDto<{ClassName}, Dto> {IfMutable(", IToMutable<Mutable>")}
    {{
{string.Join(Environment.NewLine, GetProperties().Select(ps => $"        public {ps.validType.RawTypeName} {ps.property.Name} {{get; init;}} = {ps.validType.DefaultValueAssignment};"))}
{GetTryCreate()}
{IfMutable(GetDtoToMutable)}
    }}";

    public string GetDtoToMutable()
        => $@"
        public Mutable GetMutable() => new ()
        {{
            {string.Join("," + Environment.NewLine + "            ",
                GetProperties().Select(ps => GetToMutablePropertyAssignment(ps.property,ps.validType)))}
        }};";

    public string GetToMutablePropertyAssignment(IPropertySymbol property, IValidType validType)
        => validType.IsUnderlyingTypePrimitive
            ? $"{property.Name} = {property.Name}"
            : validType.IsCollection
                ? validType.CollectionType!.Name switch
                {
                    $"{nameof(ImmutableList<object>)}`1" => $"{property.Name} = new ({property.Name}.Select(x => x.GetMutable()).ToImmutableList())",
                    $"{nameof(IImmutableSet<object>)}`1" => $"{property.Name} = new ({property.Name}.Select(x => x.GetMutable()).ToImmutableHashSet())",
                    _ => $"{property.Name} = default({validType.RawTypeName})"
                }
                : $"{property.Name} = {property.Name}.GetMutable()";
    public string GetComplexDto()
        => $@"
    public partial record Dto : IComplexDto<{ClassName}, Intermediate, Dto> {IfMutable(", IToMutable<Mutable>")}
    {{
{string.Join(Environment.NewLine, GetProperties().Select(ps => $"        public {ps.validType.RawTypeName} {ps.property.Name} {{get; init;}} = {ps.validType.DefaultValueAssignment};"))}
{GetTryCreate("TryCreateIntermediate", "Intermediate")}
        public Result<{ClassName}, ErrorList> TryCreate()
            => TryCreateIntermediate().Bind(x => x.TryCreate());
{GetDtoToMutable()}
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
        =>
            $@"    public static IObjectValidator<{ClassName}, Dto> Validator => Validators.Standard;
    public static class Validators
    {{
        public static readonly IObjectValidator<{ClassName}, Dto> Standard = ObjectValidator.For<{ClassName}, Dto>();

        public static readonly IObjectValidator<ImmutableList<{ClassName}>, ImmutableList<Dto>> List =
            Standard.ToListValidator();

        public static readonly IObjectValidator<IImmutableSet<{ClassName}>, IImmutableSet<Dto>> Set =
            Standard.ToSetValidator();

        {GetMutableValidators()}
     }}
    ";

    public string GetMutableValidators()
        => Options.Mutable
             ? $@"
         public static readonly IObjectValidator<{ClassName}, Mutable> Mutable =
             ObjectValidator.ForMutable<{ClassName}, Mutable, Mutable.Dto>();

         public static readonly IObjectValidator<{ClassName}, Mutable.Dto> MutableDto =
             ObjectValidator.ForMutableDto<{ClassName}, Mutable, Mutable.Dto>();

         public static readonly IObjectValidator<ImmutableList<{ClassName}>, ImmutableList<Mutable>> MutableList =
             ObjectValidator.MakeMutableListValidator<{ClassName}, Dto, Mutable, Mutable.Dto>();"
            : "";
    private static string letters = "tuvwxyz";

    public string GetIntermediateDto()
        => $@"
    public partial record Intermediate({string.Join(", ", GetProperties().Select(ps => $"{ps.validType.TypeName} {ps.property.Name}"))})
        : IIntermediateDto<{ClassName}, Dto> {IfMutable(", IToMutable<Mutable>")}
    {{
{GetGetDto()}
{GetIntermediateTryCreate()}
{IfMutable(GetIntermediateToMutable)}
    }}";

    public string GetNewMutableDtoWithValids(string varname)
        => $@"            var {varname} = new Mutable.Dto
            {{
                Cells = new ()
                {{
                    {string.Join("," + Environment.NewLine + "                    ",
                        from p in GetProperties()
                        where !p.validType.IsCollection
                        select $"{p.property.Name} = {{ ValidObject = {p.property.Name} }}")}
                }}
            }};
            {string.Join(Environment.NewLine + "                    ",
                from p in GetProperties()
                where p.validType.IsCollection
                select $"{varname}.{p.property.Name}.AddRange({p.property.Name}.Select(x => x.GetMutable()));")}";
public string GetIntermediateToMutable()
    => $@"
        public Mutable GetMutable() 
        {{
{GetNewMutableDtoWithValids("res")}
            return new(res);
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

    public string GetSimpleMutableImplementation() => $@"    public Mutable GetMutable() => new(this);
{GetGetMutableDto()}

    public class Mutable : ValidationCell<{ClassName}, Mutable.Dto>, IDto<{ClassName}>
    {{
{GetMutableDto()}
{GetMutableDtoConstructors()}
{string.Join(Environment.NewLine, GetProperties()
    .Select(p => GetMutableProperty(p.property,p.validType)))}
        Result<{ClassName}, ErrorList> IDto<{ClassName}>.TryCreate()
            => AsResult();
    }}";

    private string GetGetMutableDto()
        => $@"    public Mutable.Dto GetMutableDto()
        {{
{GetNewMutableDtoWithValids("res")}
            return res;
        }}";

    public string GetMutableExposedTypeName(IValidType validType)
        => validType.IsCollection
            ? $"MutableList<{validType.ElementType!.TypeName}.Mutable>"
            : validType.IsUnderlyingTypePrimitive
                ? validType.RawTypeName
                : $"{validType.TypeName}.Mutable";
    public string GetMutableTypeName(IValidType validType)
        => validType.IsCollection
            ? $"MutableList<{validType.ElementType!.TypeName}.Mutable>"
            : validType.IsUnderlyingTypePrimitive
                ? $"ValidationCell<Valid<{validType.RawTypeName},{validType.Constraint}>, {validType.RawTypeName}>"
        : $"{validType.TypeName}.Mutable";

    private string? GetMutableProperty(IPropertySymbol property, IValidType validType)
        => validType.IsCollection
            ? $@"        public {GetMutableExposedTypeName(validType)} {property.Name}
        {{
            get => Value.{property.Name};
            set
            {{
                if (!ReferenceEquals(Value.{property.Name}, value))
                {{
                    Value.{property.Name}.Clear();
                    Value.{property.Name}.AddRange(value);
                }}
            }}
        }}"
            : validType.IsUnderlyingTypePrimitive
                ? $@"        public {GetMutableExposedTypeName(validType)} {property.Name}
        {{
            get => Value.{property.Name};
            set => Value.{property.Name} = value;
        }}"
                : $@"        public {GetMutableExposedTypeName(validType)} {property.Name}
        {{
            get => Value.{property.Name};
            set
            {{
                if (!ReferenceEquals(Value.{property.Name}, value))
                {{
                    value.AsResult().Act(
                        vo => Value.{property.Name}.ValidObject = vo,
                        _ => Value.{property.Name}.Value = value.Value);
                }}
            }}
        }}";

    public string GetMutableDtoConstructors()
        => $@"       public Mutable({ClassName} validObject) : base(Validators.MutableDto, validObject)
        {{ }}
        public Mutable(Dto value) : base(Validators.MutableDto, value)
        {{ }}
        public Mutable() : base(Validators.MutableDto, new Dto())
        {{ }}";

    public string GetMutableDto()
        => $@"        public class Dto : IDto<{ClassName}>, IToMutable<Mutable>, INotifyPropertyChanged
        {{
{GetMutableDtoConstructor()}
{GetCells()}
{string.Join(Environment.NewLine,
    from p in GetProperties()
    select GetMutableDtoProperty(p.property,p.validType))}
{GetMutableDtoTryCreate()}
            public Mutable GetMutable() => new(this);
{GetPropertyChangedImplementation()}
        }}";

    public string GetMutableDtoTryCreate()
       => $@"            public Result<{ClassName}, ErrorList> TryCreate()
            => ({string.Join("," + Environment.NewLine + "                ",
                GetProperties().Select(p => $"Cells.{p.property.Name}.AsResult({If(p.validType.IsCollection, () => $"{p.validType.ElementType!.TypeName}.Validators.MutableList")}).Prefix(nameof({p.property.Name}))"))})
               .Apply(({string.Join(", ", GetProperties().Select((_,x) => letters[x]))})
                   => new {ClassName}({string.Join(", ", GetProperties().Select((_,x) => letters[x]))}));";

    public string GetMutableDtoConstructor()
        => $@"            public Dto() 
            {{
                Cells = new();
{string.Join(Environment.NewLine, GetProperties().Select(p => 
    GetRegisterPropertyChanged(p.property,p.validType)))}            
            }}";

    public string GetRegisterPropertyChanged(IPropertySymbol property, IValidType validType)
        => $"            Cells.{property.Name}.PropertyChanged += (sender, e) => OnPropertyChanged(nameof({property.Name}));"
    + If(validType.IsCollection, () => $"{Environment.NewLine}            Cells.{property.Name}.CollectionChanged += (sender, e) => OnPropertyChanged(nameof({property.Name}));");

    public string GetPropertyChangedImplementation() 
        => @"            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }";

    public string GetCells()
        => $@"            public struct CellStruct
            {{
                public CellStruct() {{ }}
{string.Join(Environment.NewLine, GetProperties().Select(p =>
    GetCellStructProperty(p.property, p.validType)))}
                
            }}
            public CellStruct Cells {{ get; internal set; }}";

    public string GetCellStructProperty(IPropertySymbol property, IValidType validType)
        => $@"                public {GetMutableTypeName(validType)} {property.Name} {{ get; }} = {GetCellStructPropertyValueAssignment(property,validType)}";

    public string GetCellStructPropertyValueAssignment(IPropertySymbol property, IValidType validType)
        => validType.IsCollection || validType.IsDictionary || !validType.IsUnderlyingTypePrimitive
            ? $"new ();"
            : $"{validType.DefaultValueAssignment}.Constrain().With<{validType.Constraint}>();";

    public string GetMutableDtoProperty(IPropertySymbol property, IValidType validType)
        => validType.IsCollection || validType.IsDictionary 
            ?$"            public MutableList<{validType.ElementType!.TypeName}.Mutable> {property.Name} => Cells.{property.Name};"
            : !validType.IsUnderlyingTypePrimitive
            ? $"            public {validType.TypeName}.Mutable {property.Name} => Cells.{property.Name};"
            : $@"            public {validType.RawTypeName} {property.Name}
            {{
                get => Cells.{property.Name}.Value;
                set => Cells.{property.Name}.Value = value;
            }}";

    public string GetComplexMutableImplementation() => GetSimpleMutableImplementation();
}