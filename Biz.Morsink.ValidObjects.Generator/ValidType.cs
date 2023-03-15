using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
namespace Biz.Morsink.ValidObjects.Generator;

class ValidType : IValidType
{
    public ValidType(ITypeSymbol type)
    {
        Type = type;
        Constraint = Type.ContainingNamespace.ToString() == "Biz.Morsink.ValidObjects"
                     && Type.Name == "Valid"
                     && Type is INamedTypeSymbol nts and {Arity:2}
            ? nts.TypeArguments[1].ToString()
            : null;
        StaticValidator = Type.Interfaces
            .FirstOrDefault(itf => itf.ContainingNamespace.ToString() == "Biz.Morsink.ValidObjects"
                && itf.Name == "IHasStaticValidator"
                && itf.Arity == 2
                && SymbolEqualityComparer.Default.Equals(itf.TypeArguments[0], type));
        FullInterface = Type.Interfaces
            .FirstOrDefault(itf => itf.ContainingNamespace.ToString() == "Biz.Morsink.ValidObjects"
                && itf.Name == "IValidObject"
                && itf.Arity == 2
                && SymbolEqualityComparer.Default.Equals(itf.TypeArguments[0], type));
        VoInterface = Type.Interfaces
            .FirstOrDefault(itf => itf.ContainingNamespace.ToString() == "Biz.Morsink.ValidObjects"
                && itf.Name == "IValidObject"
                && itf.Arity == 1);
        GenerateAttribute = Type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "Biz.Morsink.ValidObjects.ValidObjectAttribute");
        GenerateMutable = GenerateAttribute?.NamedArguments.Any(n => n is {Key: "Mutable", Value.Value: bool and true}) ?? false;
        ValidationMethods = Type.GetMembers().Where(m
                => m.Kind == SymbolKind.Method
                && m.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "Biz.Morsink.ValidObjects.ValidationMethodAttribute"))
            .ToImmutableArray();
        
        RawType = StaticValidator?.TypeArguments[1]
            ?? FullInterface?.TypeArguments[1]
            ?? VoInterface?.TypeArguments[0]
            ?? Type;
    }
    public ImmutableArray<ISymbol> ValidationMethods { get; }
    public ITypeSymbol Type { get; }
    public string TypeName => Type.ToDisplayString();
    public ITypeSymbol RawType { get; }
    public string RawTypeName => GenerateAttribute != null ? $"{Type.ToDisplayString()}.Dto" : this.RawType.ToDisplayString();
    public INamedTypeSymbol? StaticValidator { get; }
    public INamedTypeSymbol? FullInterface { get; }
    public INamedTypeSymbol? VoInterface { get; }
    public AttributeData? GenerateAttribute { get; }
    public bool GenerateMutable { get; }

    public string GetTryCreate(string name)
        => FullInterface != null || GenerateAttribute != null
            ? $"{name}.TryCreate().Prefix(nameof({name}))"
            : StaticValidator != null
                ? $"{Type}.TryCreate({name}).Prefix(nameof({name}))"
                : $"new Result<{Type}, Biz.Morsink.ValidObjects.Errors.ErrorList>.Success({name})";

    public string GetGetDto(string name)
        => VoInterface != null || GenerateAttribute != null
            ? $"{name}.GetDto()"
            : StaticValidator != null
                ? $"{Type}.GetDto({name})"
                : name;

    public string ObjectValidator
        => StaticValidator != null || GenerateAttribute != null
            ? $"{Type}.Validator"
            : FullInterface != null
                ? $"ObjectValidator.For<{TypeName}, {RawTypeName}>()"
                : VoInterface != null
                    ? "(null)"
                    : "null";
    
    public bool IsValidType => GenerateAttribute != null || !SymbolEqualityComparer.Default.Equals(Type, RawType);
    public bool IsComplexValidType => GenerateAttribute != null && ValidationMethods.Length > 0;
    public IValidType? ElementType => null;
    public bool IsCollection => false;
    public bool IsDictionary => false;
    public Type? CollectionType => null;
    public string? Constraint { get; }

    public bool IsUnderlyingTypePrimitive =>
        GenerateAttribute == null;


    public string DefaultValueAssignment
    {
        get
        {
            if (GenerateAttribute != null)
                return $"new {RawTypeName}()";
            if (RawTypeName == "string")
                return "\"\"";
            return $"default({RawTypeName})";
        }
    }
}
