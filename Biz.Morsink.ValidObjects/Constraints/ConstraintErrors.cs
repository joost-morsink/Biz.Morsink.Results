namespace Biz.Morsink.ValidObjects.Constraints;

public interface IConstraintErrors
{
    Error MaxLength(string type, int length);
    Error MaxValue(int value);
    Error NotEmpty(string type);
    Error MinValue(int value);
    Error MinLength(string type, int length);
    Error Regex(string regexString);
    Error ValueBetween(int min, int max);
    Error ValueBetween(decimal min, decimal max);
}
public class ConstraintErrors : IConstraintErrors
{
    public static IConstraintErrors Current { get; set; } = new ConstraintErrors();

    public virtual Error MaxLength(string type, int length)
        => new (default, type + nameof(MaxLength), $"{type} exceeds max length {length}");
    public virtual Error MaxValue(int value)
        => new ( default, nameof(MaxValue), $"Value should be at most {value}");
    public virtual Error MinLength(string type, int length)
        => new (default, type + nameof(MinLength), $"{type} should have at least length {length}");
    public virtual Error MinValue(int value)
        => new (default, nameof(MaxValue), $"Value should be at least {value}");
    public virtual Error ValueBetween(int min, int max)
        => new (default, nameof(ValueBetween), $"Value should be between {min} and {max}");
    public virtual Error ValueBetween(decimal min, decimal max)
        => new (default, nameof(ValueBetween), $"Value should be between {min} and {max}");
    public virtual Error NotEmpty(string type)
        => new (default, type + nameof(NotEmpty), $"{type} is not empty");
    public virtual Error Regex(string regexString)
        => new (default, nameof(Regex), $"Value does not satisfy regular expression '{regexString}'");
}
