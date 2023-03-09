namespace Biz.Morsink.ValidObjects;

public interface IToMutable<out Mut>
{
    Mut GetMutable();
}
public interface IToMutable<Vo, out Mut, MutDto> : IToMutable<Mut>
    where Vo : class, IToMutable<Mut>
    where Mut : IValidationCell<Vo, MutDto>
    where MutDto : IToMutable<Mut>
{
}

