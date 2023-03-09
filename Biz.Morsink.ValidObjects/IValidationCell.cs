using System.Diagnostics.CodeAnalysis;

namespace Biz.Morsink.ValidObjects;

public interface IValidationCell<TVo>
    where TVo : class
{
    bool IsValid { get; }
    ErrorList Errors { get; }
    [DisallowNull]
    TVo? ValidObject { get; set; }
    Result<TVo, ErrorList> AsResult();
    
}
public interface IValidationCell<TVo, TDto>
    : IValidationCell<TVo>
    where TVo : class
{
    TDto Value { get; set; }
}