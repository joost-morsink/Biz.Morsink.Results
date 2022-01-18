namespace Biz.Morsink.ValidObjects.Constraints;

public class MinValue<T> : IConstraint<int>
    where T : IInteger
{
    public static MinValue<T> Instance { get; } = new ();
    private static readonly ErrorList Error = ConstraintErrors.Current.MinValue(T.Value).ToList();
    public Result<int, ErrorList> Check(int item)
    {
        if (item < T.Value)
            return Error;
        else
            return item;
    }
}