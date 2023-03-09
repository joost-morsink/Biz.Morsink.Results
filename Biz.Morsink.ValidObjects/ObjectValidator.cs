using System.Globalization;
namespace Biz.Morsink.ValidObjects;

public static class ObjectValidator
{
    #region Standard implementations
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

    private class MutableImpl<Vo, Mut> : IObjectValidator<Vo, Mut>
        where Vo : class, IValidObjectWithToMutable<Vo, Mut>
        where Mut : IValidationCell<Vo, Mut>, IDto<Vo>
    {
        public Result<Vo, ErrorList> TryCreate(Mut dto)
            => dto.AsResult();

        public Mut GetDto(Vo validObject)
            => validObject.GetMutable();
    }
    public static IObjectValidator<Vo,Mut> ForMutable<Vo,Mut>()
        where Vo : class, IValidObjectWithToMutable<Vo, Mut>
        where Mut : IValidationCell<Vo, Mut>, IDto<Vo>
        => new MutableImpl<Vo, Mut>();

    private class MutableDtoImpl<Vo, Mut, MutDto> : IObjectValidator<Vo, MutDto>
        where Vo : class, IValidObjectWithToMutable<Vo, Mut, MutDto>
        where Mut : IValidationCell<Vo, MutDto>, IDto<Vo>
        where MutDto : IToMutable<Mut>, IDto<Vo>
    {
        public Result<Vo, ErrorList> TryCreate(MutDto dto)
            => dto.TryCreate();

        public MutDto GetDto(Vo validObject)
            => validObject.GetMutableDto();
    }
    public static IObjectValidator<Vo,MutDto> ForMutableDto<Vo,Mut,MutDto>()
        where Vo : class, IValidObjectWithToMutable<Vo, Mut, MutDto>
        where Mut : IValidationCell<Vo, MutDto>, IDto<Vo>
        where MutDto : IToMutable<Mut>, IDto<Vo>
        => new MutableDtoImpl<Vo, Mut, MutDto>();
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

    private class ImplOtherHalf<Vo, Dto> : IObjectValidator<Vo, Dto>
        where Vo : class, IValidObject
        where Dto : IDto<Vo>
    {
        private readonly Func<Vo, Dto> _getDto;

        public ImplOtherHalf(Func<Vo, Dto> getDto)
        {
            _getDto = getDto;
        }

        public Result<Vo, ErrorList> TryCreate(Dto dto)
            => dto.TryCreate();

        public Dto GetDto(Vo validObject)
            => _getDto(validObject);
    }
    public static IObjectValidator<Vo, Dto> For<Vo, Dto>(Func<Vo, Dto> getDto)
        where Vo : class, IValidObject
        where Dto : IDto<Vo>
        => new ImplOtherHalf<Vo, Dto>(getDto);
    
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
    #endregion
    
    #region List
    private class ListImpl<Vo, Dto> : IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>>
    {
        private readonly IObjectValidator<Vo, Dto> _baseValidator;
        private readonly Func<Dto, int, object> _indexer;
        public ListImpl(IObjectValidator<Vo, Dto> baseValidator, Func<Dto, int, object> indexer)
        {
            _baseValidator = baseValidator;
            _indexer = indexer;
        }
        public Result<ImmutableList<Vo>, ErrorList> TryCreate(ImmutableList<Dto> dtos)
            => dtos.Select((dto, index) => _baseValidator.TryCreate(dto).Prefix(_indexer(dto, index))).SequenceList();
        public ImmutableList<Dto> GetDto(ImmutableList<Vo> validObjects)
            => validObjects.Select(vo => _baseValidator.GetDto(vo)).ToImmutableList();
    }
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>> ToListValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator,
                                                                                                   Func<Dto, int, object> indexer)
        => new ListImpl<Vo, Dto>(baseValidator, indexer);
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Dto>> ToListValidator<Vo, Dto>(this IObjectValidator<Vo, Dto> baseValidator)
        where Dto : class
        => baseValidator.ToListValidator((_, idx) => idx);
    
    private class MutableListImpl<Vo, Dto, Mut, MutDto> : IObjectValidator<ImmutableList<Vo>, ImmutableList<Mut>>
        where Vo : class, IValidObjectWithToMutable<Vo, Dto, Mut, MutDto>
        where Dto : IDto<Vo, Dto>, IToMutable<Mut>
        where Mut : ValidationCell<Vo, MutDto>, IDto<Vo>
        where MutDto : IDto<Vo>, IToMutable<Mut>
    {
        private readonly Func<Mut, int, object> _indexer;
        public MutableListImpl(Func<Mut, int, object> indexer)
        {
            _indexer = indexer;
        }

        public Result<ImmutableList<Vo>, ErrorList> TryCreate(ImmutableList<Mut> dto)
            => dto.Select((d, index) => d.AsResult().Prefix(_indexer(d, index))).SequenceList();

        public ImmutableList<Mut> GetDto(ImmutableList<Vo> validObject)
            => validObject.Select(vo => vo.GetMutable()).ToImmutableList();
    }
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Mut>> MakeMutableListValidator<Vo, Dto,Mut, MutDto>(Func<Mut, int, object> indexer)
        where Vo : class, IValidObjectWithToMutable<Vo, Dto, Mut, MutDto>
        where Dto : IDto<Vo, Dto>, IToMutable<Mut>
        where Mut : ValidationCell<Vo, MutDto>, IDto<Vo>
        where MutDto : IDto<Vo>, IToMutable<Mut>
        => new MutableListImpl<Vo, Dto, Mut, MutDto>(indexer);
    public static IObjectValidator<ImmutableList<Vo>, ImmutableList<Mut>> MakeMutableListValidator<Vo, Dto, Mut, MutDto>()
        where Vo : class, IValidObjectWithToMutable<Vo, Dto, Mut, MutDto>
        where Dto : IDto<Vo, Dto>, IToMutable<Mut>
        where Mut : ValidationCell<Vo, MutDto>, IDto<Vo>
        where MutDto : IDto<Vo>, IToMutable<Mut>
        => MakeMutableListValidator<Vo, Dto, Mut, MutDto>((_, idx) => idx);
    #endregion
    #region Set
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
    #endregion
    
    #region Null
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
    #endregion
    
    #region Dictionary
    private class DictImpl<K, Vo, Dto, Dvo, Ddto> : IObjectValidator<Dvo, Ddto>
        where K : notnull
        where Dvo : IImmutableDictionary<K, Vo>
        where Ddto : IImmutableDictionary<K, Dto>
    {
        private readonly IObjectValidator<Vo, Dto> _baseValidator;
        private readonly Func<Dto, object> _indexer;
        private readonly Func<IEnumerable<(K, Vo)>, Dvo> _toVoDict;
        private readonly Func<IEnumerable<(K, Dto)>, Ddto> _toDtoDict;
        public DictImpl(IObjectValidator<Vo, Dto> baseValidator, Func<Dto, object> indexer, 
                        Func<IEnumerable<(K, Vo)>, Dvo> toVoDict,
                        Func<IEnumerable<(K, Dto)>, Ddto> toDtoDict)
        {
            _baseValidator = baseValidator;
            _indexer = indexer;
            _toVoDict = toVoDict;
            _toDtoDict = toDtoDict;
        }
        public Result<Dvo, ErrorList> TryCreate(Ddto dto)
            => dto.Select(kvp => new KeyValuePair<K, Result<Vo, ErrorList>>(kvp.Key, _baseValidator.TryCreate(kvp.Value)))
                .SequenceDictionary(_toVoDict);
        public Ddto GetDto(Dvo validObject)
            => _toDtoDict(validObject.Select(kvp => (kvp.Key, _baseValidator.GetDto(kvp.Value))));
    }
    public static IObjectValidator<IImmutableDictionary<K, Vo>, IImmutableDictionary<K, Dto>> ToDictionaryValidator<K, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner, Func<Dto, object> indexer)
        where K : notnull
        => new DictImpl<K, Vo, Dto, IImmutableDictionary<K, Vo>, IImmutableDictionary<K, Dto>>(inner, indexer,
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2),
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2));
    public static IObjectValidator<IImmutableDictionary<K, Vo>, IImmutableDictionary<K, Dto>> ToDictionaryValidator<K, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner)
        where K : notnull
        where Dto : class
        => inner.ToDictionaryValidator<K, Vo, Dto>(d => d);
    public static IObjectValidator<ImmutableDictionary<K, Vo>, ImmutableDictionary<K, Dto>> ToHashDictionaryValidator<K, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner, Func<Dto, object> indexer)
        where K : notnull
        => new DictImpl<K, Vo, Dto, ImmutableDictionary<K, Vo>, ImmutableDictionary<K, Dto>>(inner, indexer,
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2),
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2));
    public static IObjectValidator<ImmutableDictionary<K, Vo>, ImmutableDictionary<K, Dto>> ToHashDictionaryValidator<K, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner)
        where K : notnull
        where Dto : class
        => inner.ToHashDictionaryValidator<K, Vo, Dto>(d => d);
    public static IObjectValidator<ImmutableSortedDictionary<K, Vo>, ImmutableSortedDictionary<K, Dto>> ToSortedDictionaryValidator<K, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner, Func<Dto, object> indexer)
        where K : notnull
        => new DictImpl<K, Vo, Dto, ImmutableSortedDictionary<K, Vo>, ImmutableSortedDictionary<K, Dto>>(inner, indexer,
            kvps => kvps.ToImmutableSortedDictionary(x => x.Item1, x => x.Item2),
            kvps => kvps.ToImmutableSortedDictionary(x => x.Item1, x => x.Item2));
    public static IObjectValidator<ImmutableSortedDictionary<K, Vo>, ImmutableSortedDictionary<K, Dto>> ToSortedDictionaryValidator<K, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner)
        where K : notnull
        where Dto : class
        => inner.ToSortedDictionaryValidator<K, Vo, Dto>(d => d);
    #endregion
        #region Dictionary
    private class KeyDictImpl<Kvo, Kdto, Vo, Dto, Dvo, Ddto> : IObjectValidator<Dvo, Ddto>
        where Kvo : notnull
        where Kdto : notnull
        where Dvo : IImmutableDictionary<Kvo, Vo>
        where Ddto : IImmutableDictionary<Kdto, Dto>
    {
        private readonly IObjectValidator<Vo, Dto> _baseValueValidator;
        private readonly Func<Dto, object> _indexer;
        private readonly Func<IEnumerable<(Kvo, Vo)>, Dvo> _toVoDict;
        private readonly Func<IEnumerable<(Kdto, Dto)>, Ddto> _toDtoDict;
        private readonly IObjectValidator<Kvo, Kdto> _baseKeyValidator;
        public KeyDictImpl(IObjectValidator<Vo, Dto> baseValueValidator,
                        IObjectValidator<Kvo, Kdto> baseKeyValidator,
                        Func<Dto, object> indexer, 
                        Func<IEnumerable<(Kvo, Vo)>, Dvo> toVoDict,
                        Func<IEnumerable<(Kdto, Dto)>, Ddto> toDtoDict)
        {
            _baseValueValidator = baseValueValidator;
            _baseKeyValidator = baseKeyValidator;
            _indexer = indexer;
            _toVoDict = toVoDict;
            _toDtoDict = toDtoDict;
        }
        public Result<Dvo, ErrorList> TryCreate(Ddto dto)
            => dto.Select(kvp => new KeyValuePair<Result<Kvo, ErrorList>, Result<Vo, ErrorList>>(_baseKeyValidator.TryCreate(kvp.Key), _baseValueValidator.TryCreate(kvp.Value)))
                .SequenceKeyDictionary(_toVoDict);
        public Ddto GetDto(Dvo validObject)
            => _toDtoDict(validObject.Select(kvp => (_baseKeyValidator.GetDto(kvp.Key), _baseValueValidator.GetDto(kvp.Value))));
    }
    public static IObjectValidator<IImmutableDictionary<Kvo, Vo>, IImmutableDictionary<Kdto, Dto>> ToDictionaryValidator<Kvo, Kdto, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner, 
        IObjectValidator<Kvo, Kdto> key,
        Func<Dto, object> indexer)
        where Kvo : notnull
        where Kdto : notnull
        => new KeyDictImpl<Kvo, Kdto, Vo, Dto, IImmutableDictionary<Kvo, Vo>, IImmutableDictionary<Kdto, Dto>>(inner, key, indexer,
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2),
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2));
    public static IObjectValidator<IImmutableDictionary<Kvo, Vo>, IImmutableDictionary<Kdto, Dto>> ToDictionaryValidator<Kvo, Kdto, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner, 
        IObjectValidator<Kvo,Kdto> key)
        where Kvo : notnull
        where Kdto : notnull
        where Dto : class
        => inner.ToDictionaryValidator(key, d => d);
    public static IObjectValidator<ImmutableDictionary<Kvo, Vo>, ImmutableDictionary<Kdto, Dto>> ToHashDictionaryValidator<Kvo, Kdto, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner,
        IObjectValidator<Kvo, Kdto> key,
        Func<Dto, object> indexer)
        where Kvo : notnull
        where Kdto : notnull
        => new KeyDictImpl<Kvo, Kdto, Vo, Dto, ImmutableDictionary<Kvo, Vo>, ImmutableDictionary<Kdto, Dto>>(inner, key, indexer,
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2),
            kvps => kvps.ToImmutableDictionary(x => x.Item1, x => x.Item2));
    public static IObjectValidator<ImmutableDictionary<Kvo, Vo>, ImmutableDictionary<Kdto, Dto>> ToHashDictionaryValidator<Kvo, Kdto, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner,
        IObjectValidator<Kvo, Kdto> key)
        where Kvo : notnull
        where Kdto : notnull
        where Dto : class
        => inner.ToHashDictionaryValidator(key, d => d);
    public static IObjectValidator<ImmutableSortedDictionary<Kvo, Vo>, ImmutableSortedDictionary<Kdto, Dto>> ToSortedDictionaryValidator<Kvo, Kdto, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner, 
        IObjectValidator<Kvo, Kdto> key,
        Func<Dto, object> indexer)
        where Kvo : notnull
        where Kdto : notnull
        => new KeyDictImpl<Kvo, Kdto, Vo, Dto, ImmutableSortedDictionary<Kvo, Vo>, ImmutableSortedDictionary<Kdto, Dto>>(inner, key, indexer,
            kvps => kvps.ToImmutableSortedDictionary(x => x.Item1, x => x.Item2),
            kvps => kvps.ToImmutableSortedDictionary(x => x.Item1, x => x.Item2));
    public static IObjectValidator<ImmutableSortedDictionary<Kvo, Vo>, ImmutableSortedDictionary<Kdto, Dto>> ToSortedDictionaryValidator<Kvo, Kdto, Vo, Dto>(
        this IObjectValidator<Vo, Dto> inner,
        IObjectValidator<Kvo, Kdto> key)
        where Kvo : notnull
        where Kdto : notnull
        where Dto : class
        => inner.ToSortedDictionaryValidator(key, d => d);
    #endregion

}
