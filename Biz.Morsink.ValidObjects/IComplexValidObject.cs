namespace Biz.Morsink.ValidObjects;

public interface IComplexValidObject<Vo, Int, out Dto, out MutDto> : IComplexValidObject<Vo, Int, Dto>,
    IValidObject<Vo, Dto, MutDto>
    where Vo : class, IComplexValidObject<Vo, Int, Dto, MutDto>
    where Int : class, IIntermediateDto<Vo, Dto>
    where Dto : IComplexDto<Vo, Int, Dto>
    where MutDto : IDto<Vo>
{
}

public interface IComplexValidObject<Vo, Int, out Dto> : IValidObject<Vo, Dto>
    where Vo : class, IComplexValidObject<Vo, Int, Dto>
    where Int : class, IIntermediateDto<Vo, Dto>
    where Dto : IComplexDto<Vo, Int, Dto>
{
    Int GetIntermediate();
}