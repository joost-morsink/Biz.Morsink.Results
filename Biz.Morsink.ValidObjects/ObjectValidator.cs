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
    private class SetImpl<Vo, Dto, Svo, Sdto> : IObjectValidator<Svo, Sdto>
        where Svo : IImmutableSet<Vo>
        where Sdto : IImmutableSet<Dto>
    {
        private readonly IObjectValidator<Vo, Dto> _baseValidator;
        private readonly Func<Dto, object> _indexer;
        private readonly Func<IEnumerable<Vo>, Svo> _toVoSet;
        private readonly Func<IEnumerable<Dto>, Sdto> _toDtoSet;

        public SetImpl(IObjectValidator<Vo, Dto> baseValidator, Func<Dto, object> indexer, 
                       Func<IEnumerable<Vo>, Svo> toVoSet,
                       Func<IEnumerable<Dto>, Sdto> toDtoSet)
        {
            _baseValidator = baseValidator;
            _indexer = indexer;
            _toVoSet = toVoSet;
            _toDtoSet = toDtoSet;
        }
        public Result<Svo, ErrorList> TryCreate(Sdto dtos)
            => dtos.Select((dto) => _baseValidator.TryCreate(dto).Prefix(_indexer(dto))).SequenceSet(_toVoSet);
        public Sdto GetDto(Svo validObjects)
            => _toDtoSet(validObjects.Select(vo => _baseValidator.GetDto(vo)));
    }
    
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>> ToListValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
                                                                                                   Func<Dto, int, object> indexer)
        => new ListImpl<Vo, Dto>(baseValidator, indexer);
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>> ToListValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToListValidator((d, _) => d);
    public static IObjectValidator<IImmutableSet<Vo>, IImmutableSet<Dto>> ToSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
                                                                                                  Func<Dto, object> indexer)
        => new SetImpl<Vo, Dto,IImmutableSet<Vo>, IImmutableSet<Dto>>(baseValidator, indexer,
                ts => ts.ToImmutableHashSet(),
                ts => ts.ToImmutableHashSet());
    public static IObjectValidator<IImmutableSet<Vo>, IImmutableSet<Dto>> ToSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToSetValidator(d => d);
    public static IObjectValidator<ImmutableHashSet<Vo>, ImmutableHashSet<Dto>> ToHashSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
                                                                                                  Func<Dto, object> indexer)
        => new SetImpl<Vo, Dto,ImmutableHashSet<Vo>, ImmutableHashSet<Dto>>(baseValidator, indexer,
            ts => ts.ToImmutableHashSet(),
            ts => ts.ToImmutableHashSet());
    public static IObjectValidator<ImmutableHashSet<Vo>, ImmutableHashSet<Dto>> ToHashSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToHashSetValidator(d => d);
    public static IObjectValidator<ImmutableSortedSet<Vo>, ImmutableSortedSet<Dto>> ToSortedSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
        Func<Dto, object> indexer)
        => new SetImpl<Vo, Dto,ImmutableSortedSet<Vo>, ImmutableSortedSet<Dto>>(baseValidator, indexer,
            ts => ts.ToImmutableSortedSet(),
            ts => ts.ToImmutableSortedSet());
    public static IObjectValidator<ImmutableSortedSet<Vo>, ImmutableSortedSet<Dto>> ToSortedSetValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToSortedSetValidator(d => d);

    private class NullableImpl<Vo, Dto> : IObjectValidator<Vo?, Dto?>
        where Vo : notnull
        where Dto : notnull
    {
        private readonly IObjectValidator<Vo, Dto> _inner;
        public NullableImpl(IObjectValidator<Vo,Dto> inner)
        {
            _inner = inner;
        }

        public Result<Vo?, ErrorList> TryCreate(Dto? dto)
            => dto.PropagateNull(_inner.TryCreate);
        public Dto? GetDto(Vo? validObject)
            => validObject == null ? default : _inner.GetDto(validObject);
    }
    public static IObjectValidator<Vo?, Dto?> ToNullableValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> validator)
        where Vo : notnull
        where Dto : notnull
        => new NullableImpl<Vo, Dto>(validator);
}
