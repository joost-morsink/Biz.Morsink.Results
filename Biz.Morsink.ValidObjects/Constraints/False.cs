namespace Biz.Morsink.ValidObjects.Constraints;

public class False<T> : IConstraint<T>
{
    public static False<T> Instance { get; } = new ();

    public Result<T, ErrorList> Check(T item)
        => Result.For<T, ErrorList>().Failure(new Error("","ALWAYS_FAIL","Always fails."));
}