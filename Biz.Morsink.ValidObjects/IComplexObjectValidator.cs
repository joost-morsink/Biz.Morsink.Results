namespace Biz.Morsink.ValidObjects;

public interface IComplexObjectValidator<Vo, Int, Dto> : IObjectValidator<Vo, Dto>
{
    Result<Vo, ErrorList> TryCreate(Int dto);
    Result<Int, ErrorList> TryCreateIntermediate(Dto dto); 
    Dto GetDto(Int intermediate);
    Int GetIntermediate(Vo validObject);
}
