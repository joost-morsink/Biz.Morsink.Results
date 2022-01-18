namespace Biz.Morsink.ValidObjects;

public interface IHasStaticValidator<Vo, Dto>
    where Vo : class, IHasStaticValidator<Vo,Dto>
{
    static abstract IObjectValidator<Vo, Dto> Validator { get; }
}
