namespace Biz.Morsink.ValidObjects;

public class Valid<T, C> : IValidObject<T>, IHasStaticValidator<Valid<T, C>, T>
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
}
