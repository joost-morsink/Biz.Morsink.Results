namespace Biz.Morsink.ValidObjects;

public interface IComplexValidObject<Vo, Int, out Dto> : IValidObject<Vo, Dto>
    where Vo : class, IComplexValidObject<Vo, Int, Dto>
    where Int : class, IIntermediateDto<Vo, Dto>
    where Dto : IComplexDto<Vo, Int, Dto>
{
    Int GetIntermediate();
}
