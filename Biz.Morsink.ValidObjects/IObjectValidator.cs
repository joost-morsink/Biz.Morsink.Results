namespace Biz.Morsink.ValidObjects;

public interface IObjectValidator<Vo, Dto>
{
    Result<Vo, ErrorList> TryCreate(Dto dto);
    Dto GetDto(Vo validObject);
}
