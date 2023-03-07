namespace Biz.Morsink.ValidObjects;

public static class ValidationCellExtensions
{
    public static ValidationCell<TVo, TDto> ValidToCell<TVo, TDto>(this TVo validObject, Func<TDto, Result<TVo, ErrorList>> validator)
        where TVo : class, IValidObject<TDto>
        where TDto : notnull
        => new (ObjectValidator.For(validator), validObject);
    public static ValidationCell<TVo, TDto> DtoToCell<TVo, TDto>(this TDto dto, Func<TDto, Result<TVo, ErrorList>> validator)
        where TVo : class, IValidObject<TDto>
        where TDto : notnull
        => new (ObjectValidator.For(validator), dto);
    public static ValidationCell<TVo, TDto> ValidToCell<TVo, TDto>(this TVo validObject)
        where TVo : class, IValidObject<TVo, TDto>
        where TDto : class, IDto<TVo, TDto>
        => new (ObjectValidator.For<TVo,TDto>(), validObject);
    public static ValidationCell<TVo, TDto> DtoToCell<TVo, TDto>(this TDto dto)
        where TVo : class, IValidObject<TVo, TDto>
        where TDto : class, IDto<TVo, TDto>
        => new (ObjectValidator.For<TVo,TDto>(), dto);

    public static ValidationCell<TVo, TDto> CreateCell<TVo, TDto>(this IObjectValidator<TVo, TDto> validator, TVo validObject)
        where TVo : class
        where TDto : notnull
        => new(validator, validObject);
    public static ValidationCell<TVo, TDto> CreateCell<TVo, TDto>(this IObjectValidator<TVo, TDto> validator, TDto dto)
        where TVo : class
        where TDto : notnull
        => new(validator, dto);
    
    public static ValidationCell<Valid<TDto, TConstraint>, TDto> ValidToCell<TDto,TConstraint>(this Valid<TDto, TConstraint> validObject)
        where TDto : notnull
        where TConstraint : IConstraint<TDto>, new()
        => new (Valid<TDto,TConstraint>.Validator, validObject);
    
    public static ValidationCell<Valid<TDto, TConstraint>, TDto> Constrain<TDto, TConstraint>(this TDto dto)
        where TDto : notnull
        where TConstraint : IConstraint<TDto>, new()
        => new (Valid<TDto,TConstraint>.Validator, dto);

    public static ConstrainedBuilder<TDto> Constrain<TDto>(this TDto dto)
        where TDto : notnull
        => new(dto);

    public readonly struct ConstrainedBuilder<TDto>
        where TDto : notnull
    {
        private readonly TDto _dto;

        public ConstrainedBuilder(TDto dto)
        {
            _dto = dto;
        }
        public ValidationCell<Valid<TDto, TConstraint>, TDto> With<TConstraint>()
            where TConstraint : IConstraint<TDto>, new()
            => new (Valid<TDto,TConstraint>.Validator, _dto);
    }
}