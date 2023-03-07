namespace Biz.Morsink.ValidObjects;

public interface IDto<Vo,  out Dto> : IDto<Vo>
    where Vo : class, IValidObject<Vo, Dto>
    where Dto : IDto<Vo, Dto>
{
}
public interface IDto<Vo> : IDto
    where Vo : class
{
    new Result<Vo, ErrorList> TryCreate();
    IResult<object, ErrorList> IDto.TryCreate() => TryCreate();
}
public interface IDto
{
    IResult<object, ErrorList> TryCreate();
}
