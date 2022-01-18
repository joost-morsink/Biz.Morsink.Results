namespace Biz.Morsink.ValidObjects;

public interface IValidObject<Vo, out Dto> : IValidObject<Dto>
    where Vo : class, IValidObject<Vo, Dto>
    where Dto : IDto<Vo, Dto>
{
}
public interface IValidObject<out Dto> : IValidObject
    where Dto : notnull
{
    new Dto GetDto();
    object IValidObject.GetDto() => GetDto();
}
public interface IValidObject
{
    object GetDto();
}
