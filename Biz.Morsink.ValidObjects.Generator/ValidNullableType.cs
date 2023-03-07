namespace Biz.Morsink.ValidObjects.Generator;

public class ValidNullableType : IValidType
{
    private readonly IValidType _underlyingType;
    public ValidNullableType(IValidType underlyingType)
    {
        _underlyingType = underlyingType;
    }
    public string RawTypeName => _underlyingType.RawTypeName + "?";
    public string TypeName => _underlyingType.TypeName + "?";
    public bool IsValidType => _underlyingType.IsValidType;
    public bool IsComplexValidType => false;
    public string DefaultValueAssignment => "";
    public string ObjectValidator => $"{_underlyingType.ObjectValidator}.ToNullableValidator()";
    public string GetTryCreate(string name)
        => $"{ObjectValidator}.TryCreate({name}).Prefix(nameof({name}))";
    public string GetGetDto(string name)
        => $"{ObjectValidator}.GetDto({name})";

}
