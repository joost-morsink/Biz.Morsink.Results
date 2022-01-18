namespace Biz.Morsink.ValidObjects.Constraints;

public class MaxValue<T> : IConstraint<int>
    where T : IInteger
{
    public static MaxValue<T> Instance { get; } = new ();
    private static readonly ErrorList Error = ConstraintErrors.Current.MaxValue(T.Value).ToList();
    public Result<int, ErrorList> Check(int item)
    {
        if (item > T.Value)
            return Error;
        else
            return item;
    }
}