namespace Biz.Morsink.ValidObjects;


public interface IValidObjectWithToMutable<Vo, out Mut>
    : IToMutable<Mut>, IValidObject
    where Vo : class, IToMutable<Mut>
    where Mut : IValidationCell<Vo>, IDto<Vo>
{
 
}

public interface IValidObjectWithToMutable<Vo, out Mut, MutDto>
    : IValidObjectWithToMutable<Vo, Mut>
    where Vo : class, IValidObject, IToMutable<Mut>
    where Mut : IValidationCell<Vo, MutDto>, IDto<Vo>
    where MutDto : IToMutable<Mut>, IDto<Vo>
{
    MutDto GetMutableDto();
}

public interface IValidObjectWithToMutable<Vo, Dto, out Mut, MutDto>
    : IValidObjectWithToMutable<Vo, Mut, MutDto>, IToMutable<Vo, Mut, MutDto>, IValidObject<Vo, Dto, MutDto>
    where Vo : class, IValidObject<Vo, Dto, MutDto>, IToMutable<Mut>
    where Dto : IDto<Vo, Dto>, IToMutable<Mut>
    where Mut : IValidationCell<Vo, MutDto>, IDto<Vo>
    where MutDto : IToMutable<Mut>, IDto<Vo>
{
 
}