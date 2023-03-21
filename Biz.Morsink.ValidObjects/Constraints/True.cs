namespace Biz.Morsink.ValidObjects.Constraints;

public class True<T> : IConstraint<T>
{
    public static True<T> Instance { get; } = new ();

    public Result<T, ErrorList> Check(T item)
        => Result.For<T, ErrorList>().Success(item);
}