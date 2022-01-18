// ReSharper disable InconsistentNaming
namespace Biz.Morsink.ValidObjects.Constraints;

public class BindAnd<T, C, D> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
{
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item).Bind(x => _d.Check(x));
}
public class BindAnd<T, C, D, E> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
    where E : IConstraint<T>, new()
{
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    private static readonly E _e = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item)
            .Bind(x => _d.Check(x))
            .Bind(x => _e.Check(x));
}
public class BindAnd<T, C, D, E, F> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
    where E : IConstraint<T>, new()
    where F : IConstraint<T>, new()
{
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    private static readonly E _e = new ();
    private static readonly F _f = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item)
            .Bind(x => _d.Check(x))
            .Bind(x => _e.Check(x))
            .Bind(x => _f.Check(x));
}
public class BindAnd<T, C, D, E, F, G> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
    where E : IConstraint<T>, new()
    where F : IConstraint<T>, new()
    where G : IConstraint<T>, new()
{
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    private static readonly E _e = new ();
    private static readonly F _f = new ();
    private static readonly G _g = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item)
            .Bind(x => _d.Check(x))
            .Bind(x => _e.Check(x))
            .Bind(x => _f.Check(x))
            .Bind(x => _g.Check(x));
}
