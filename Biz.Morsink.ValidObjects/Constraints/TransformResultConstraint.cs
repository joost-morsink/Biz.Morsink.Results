namespace Biz.Morsink.ValidObjects.Constraints;

public abstract class TransformResultConstraint<T, C, R, S> : IConstraint<T, S>
    where C : IConstraint<T, R>, new()
    where R : notnull
    where S : notnull
{
    private static readonly C _constraint = new();

    Result<T, ErrorList> IConstraint<T>.Check(T item)
    {
        throw new NotImplementedException();
    }

    public Result<(T, S), ErrorList> Check(T item)
        => _constraint.Check(item).Select(t => (t.Item1, TransformResult(t.Item2)));
    
    protected abstract S TransformResult(R result);
}