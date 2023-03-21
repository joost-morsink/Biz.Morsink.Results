namespace Biz.Morsink.ValidObjects;

public interface IObjectValidator<Vo, Dto>
{
    Result<Vo, ErrorList> TryCreate(Dto smuts);
    Dto GetDto(Vo validObject);
}
