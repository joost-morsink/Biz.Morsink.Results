namespace Biz.Morsink.ValidObjects.Constraints;

public class ValueBetween<Min, Max> : IConstraint<int>
    where Min : IInteger
    where Max : IInteger
{
    public static ValueBetween<Min, Max> Instance { get; } = new();
    private static readonly ErrorList Error = ConstraintErrors.Current.ValueBetween(Min.Value, Max.Value).ToList();
    public Result<int, ErrorList> Check(int item)
    {
        if (item < Min.Value || item > Max.Value)
            return Error;
        else
            return item;
    }
}