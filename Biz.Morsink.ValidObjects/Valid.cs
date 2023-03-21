namespace Biz.Morsink.ValidObjects;

public class Valid<T, C> : IValidObject<T>, IHasStaticValidator<Valid<T, C>, T>, IValidObjectWithToMutable<Valid<T,C>, Valid<T,C>.Mutable>
    where C : IConstraint<T>, new()
    where T : notnull
{
    public static IObjectValidator<Valid<T, C>, T> Validator { get; }
        = ObjectValidator.For<Valid<T, C>, T>(TryCreate);
    public static C Constraint { get; } = new ();
    private Valid(T value)
    {
        Value = value;
    }
    public T Value { get; }
    public static implicit operator T(Valid<T, C> value)
        => value.Value;
    public static Result<Valid<T, C>, ErrorList> TryCreate(T value)
        => Constraint.Check(value).Select(v => new Valid<T, C>(v));
    public T GetDto()
        => Value;

    public override string ToString()
        => $"{{{Value}}}";

    public class Mutable : ValidationCell<Valid<T, C>, T>, IDto<Valid<T,C>>
    {
        public Mutable(Valid<T, C> validObject) : base(Validator, validObject)
        {
        }
        public Mutable(T value) : base(Validator, value)
        {
        }

        public Mutable() : base(Validator, typeof(T) == typeof(string) ? (T)(object)"" :default!)
        {
        }

        public static implicit operator Mutable(T value)
            => new(value);
        
        public Result<Valid<T, C>, ErrorList> TryCreate()
            => AsResult();

        public override string ToString()
            => $"{{{Value}}}";
    }

    public Mutable GetMutable()
        => new(this);

    public static class Validators
    {
        public static IObjectValidator<Valid<T, C>, T> Standard => Validator;
        public static readonly IObjectValidator<ImmutableList<Valid<T,C>>, ImmutableList<T>> List =
            Standard.ToListValidator();

        public static readonly IObjectValidator<IImmutableSet<Valid<T,C>>, IImmutableSet<T>> Set =
            Standard.ToSetValidator();

        public static readonly IObjectValidator<Valid<T, C>, Mutable> Mutable
            = ObjectValidator.For<Valid<T, C>, Mutable>(vo => vo.GetMutable());

        public static readonly IObjectValidator<ImmutableList<Valid<T,C>>, ImmutableList<Mutable>> MutableList 
            = Mutable.ToListValidator();
        
        public static readonly IObjectValidator<IImmutableSet<Valid<T,C>>, IImmutableSet<Mutable>> MutableSet =
            Mutable.ToSetValidator(d => d);

    }
}
