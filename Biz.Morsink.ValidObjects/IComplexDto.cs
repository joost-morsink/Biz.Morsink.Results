namespace Biz.Morsink.ValidObjects;

public interface IComplexDto<Vo, Int, out Dto> : IDto<Vo, Dto>
    where Vo : class, IComplexValidObject<Vo, Int, Dto>
    where Int : class, IIntermediateDto<Vo, Dto>
    where Dto : IComplexDto<Vo, Int, Dto>
{
    Result<Int, ErrorList> TryCreateIntermediate();
    Result<Vo, ErrorList> IDto<Vo>.TryCreate() => TryCreate();
    public new Result<Vo, ErrorList> TryCreate()
        => TryCreateIntermediate().Bind(i => i.TryCreate());
}
