namespace Biz.Morsink.ValidObjects;

public interface IComplexObjectValidator<Vo, Int, Dto>
{
    Result<Vo, ErrorList> TryCreate(Dto dto);
    Result<Vo, ErrorList> TryCreate(Int dto);
    Result<Int, ErrorList> TryCreateIntermediate(Dto dto);
    Dto GetDto(Vo validObject);
    Dto GetDto(Int intermediate);
    Int GetIntermediate(Vo validObject);
}
