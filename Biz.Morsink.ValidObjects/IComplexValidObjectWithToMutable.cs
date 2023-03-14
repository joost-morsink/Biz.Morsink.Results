namespace Biz.Morsink.ValidObjects;

public interface IComplexValidObjectWithToMutable<Vo, Int, Dto, out Mut, MutDto>
    : IComplexValidObject<Vo, Int, Dto, MutDto>, IValidObjectWithToMutable<Vo, Dto, Mut, MutDto>
    where Vo : class, IComplexValidObject<Vo, Int, Dto, MutDto>, IToMutable<Vo, Mut, MutDto>
    where Int : class, IIntermediateDto<Vo, Dto>, IToMutable<Mut>
    where Dto : IComplexDto<Vo, Int, Dto>, IToMutable<Mut>
    where Mut : IValidationCell<Vo, MutDto>, IDto<Vo>
    where MutDto : IToMutable<Mut>, IDto<Vo>
{
}