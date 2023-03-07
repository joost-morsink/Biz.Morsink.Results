namespace Biz.Morsink.ValidObjects;

public interface IIntermediateDto<Vo, out Dto> : IIntermediateDto<Vo>
    where Vo : class
    where Dto : notnull
{
    new Dto GetDto();
    object IIntermediateDto.GetDto() => GetDto();
}
public interface IIntermediateDto<Vo> : IIntermediateDto
    where Vo : class
{
    new Result<Vo, ErrorList> TryCreate();
    IResult<object, ErrorList> IIntermediateDto.TryCreate() => TryCreate();
}
public interface IIntermediateDto
{
    IResult<object, ErrorList> TryCreate();
    object GetDto();
}
