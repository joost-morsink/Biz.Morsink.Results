namespace Biz.Morsink.ValidObjects.Constraints;

public class RealValueBetween<Min, Max> : IConstraint<decimal>, IConstraint<double>, IConstraint<float>
    where Min : IReal
    where Max : IReal
{
    public static RealValueBetween<Min, Max> Instance { get; } = new();
    
    private static readonly ErrorList Error = ConstraintErrors.Current.ValueBetween(Min.Value, Max.Value).ToList();
    public Result<decimal, ErrorList> Check(decimal item)
    {
        if (item < Min.Value || item > Max.Value)
            return Error;
        else
            return item;
    }
    public Result<double, ErrorList> Check(double item)
    {
        if (item < (double)Min.Value || item > (double)Max.Value)
            return Error;
        else
            return item;
    }
    public Result<float, ErrorList> Check(float item)
    {
        if (item < (float)Min.Value || item > (float)Max.Value)
            return Error;
        else
            return item;
    }

}
