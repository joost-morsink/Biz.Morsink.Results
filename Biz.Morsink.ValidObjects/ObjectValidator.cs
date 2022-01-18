namespace Biz.Morsink.ValidObjects;

public static class ObjectValidator
{
    private class Impl<Vo, Dto> : IObjectValidator<Vo, Dto>
        where Vo : class, IValidObject<Vo, Dto>
        where Dto : IDto<Vo, Dto>
    {
        public Result<Vo, ErrorList> TryCreate(Dto dto)
            => dto.TryCreate();
        public Dto GetDto(Vo validObject)
            => validObject.GetDto();
    }
    public static IObjectValidator<Vo, Dto> For<Vo, Dto>()
        where Vo : class, IValidObject<Vo, Dto>
        where Dto : IDto<Vo, Dto>
        => new Impl<Vo, Dto>();
    private class ImplHalf<Vo, Dto> : IObjectValidator<Vo, Dto>
        where Vo : class, IValidObject<Dto>
        where Dto : notnull
    {
        private readonly Func<Dto, Result<Vo, ErrorList>> _tryCreate;
        public ImplHalf(Func<Dto, Result<Vo, ErrorList>> tryCreate)
        {
            _tryCreate = tryCreate;
        }

        public Result<Vo, ErrorList> TryCreate(Dto dto)
            => _tryCreate(dto);
        public Dto GetDto(Vo validObject)
            => validObject.GetDto();
    }
    public static IObjectValidator<Vo, Dto> For<Vo, Dto>(Func<Dto, Result<Vo, ErrorList>> tryCreate)
        where Vo : class, IValidObject<Dto>
        where Dto : notnull
        => new ImplHalf<Vo, Dto>(tryCreate);
    
    private class ComplexImpl<Vo, Int, Dto> : IComplexObjectValidator<Vo, Int, Dto>
        where Vo : class, IComplexValidObject<Vo, Int, Dto>
        where Int : class, IIntermediateDto<Vo, Dto>
        where Dto : IComplexDto<Vo, Int, Dto>
    {
        public Result<Vo, ErrorList> TryCreate(Dto dto)
            => dto.TryCreate();
        public Result<Vo, ErrorList> TryCreate(Int dto)
            => dto.TryCreate();
        public Result<Int, ErrorList> TryCreateIntermediate(Dto dto)
            => dto.TryCreateIntermediate();
        public Dto GetDto(Vo validObject)
            => validObject.GetDto();
        public Dto GetDto(Int intermediate)
            => intermediate.GetDto();
        public Int GetIntermediate(Vo validObject)
            => validObject.GetIntermediate();
    }
    public static IComplexObjectValidator<Vo, Int, Dto> For<Vo, Int, Dto>()
        where Vo : class, IComplexValidObject<Vo, Int, Dto>
        where Int : class, IIntermediateDto<Vo, Dto>
        where Dto : IComplexDto<Vo, Int, Dto>
        => new ComplexImpl<Vo, Int, Dto>();

    private class ListImpl<Vo, Dto> : IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>>
    {
        private readonly IObjectValidator<Vo, Dto> _baseValidator;
        private readonly Func<Dto, int, object> _indexer;
        public ListImpl(IObjectValidator<Vo, Dto> baseValidator, Func<Dto,int, object> indexer)
        {
            _baseValidator = baseValidator;
            _indexer = indexer;
        }
        public Result<ImmutableList<Vo>, ErrorList> TryCreate(ImmutableList<Dto> dtos)
            => dtos.Select((dto, index) => _baseValidator.TryCreate(dto).Prefix(_indexer(dto, index))).SequenceList();
        public ImmutableList<Dto> GetDto(ImmutableList<Vo> validObjects)
            => validObjects.Select(vo => _baseValidator.GetDto(vo)).ToImmutableList();
    }
    private class SetImpl<Vo, Dto> : IObjectValidator<IImmutableSet<Vo>, IImmutableSet<Dto>>
    {
        private readonly IObjectValidator<Vo, Dto> _baseValidator;
        private readonly Func<Dto, object> _indexer;
        public SetImpl(IObjectValidator<Vo, Dto> baseValidator, Func<Dto, object> indexer)
        {
            _baseValidator = baseValidator;
            _indexer = indexer;
        }
        public Result<IImmutableSet<Vo>, ErrorList> TryCreate(IImmutableSet<Dto> dtos)
            => dtos.Select((dto) => _baseValidator.TryCreate(dto).Prefix(_indexer(dto))).SequenceSet();
        public IImmutableSet<Dto> GetDto(IImmutableSet<Vo> validObjects)
            => validObjects.Select(vo => _baseValidator.GetDto(vo)).ToImmutableHashSet();
    }
    
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>> ToListValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
                                                                                                   Func<Dto, int, object> indexer)
        => new ListImpl<Vo, Dto>(baseValidator, indexer);
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>> ToListValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToListValidator((d, _) => d);
    public static IObjectValidator<IImmutableSet<Vo>, IImmutableSet<Dto>> ToSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
                                                                                                  Func<Dto, object> indexer)
        => new SetImpl<Vo, Dto>(baseValidator, indexer);
    public static IObjectValidator<IImmutableSet<Vo>, IImmutableSet<Dto>> ToSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToSetValidator(d => d);
}
