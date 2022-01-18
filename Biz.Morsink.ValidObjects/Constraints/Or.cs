// ReSharper disable InconsistentNaming
namespace Biz.Morsink.ValidObjects.Constraints;

public class Or<T, C, D> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
{
    public static Or<T, C, D> Instance { get; } = new ();
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item).BindError(_ => _d.Check(item));
}
public class Or<T, C, D, E> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
    where E : IConstraint<T>, new()
{
    public static Or<T, C, D, E> Instance { get; } = new ();
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    private static readonly E _e = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item)
            .BindError(_ => _d.Check(item))
            .BindError(_ => _e.Check(item));
}
public class Or<T, C, D, E, F> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
    where E : IConstraint<T>, new()
    where F : IConstraint<T>, new()
{
    public static Or<T, C, D, E, F> Instance { get; } = new ();
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    private static readonly E _e = new ();
    private static readonly F _f = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item)
            .BindError(_ => _d.Check(item))
            .BindError(_ => _e.Check(item))
            .BindError(_ => _f.Check(item));
}
public class Or<T, C, D, E, F, G> : IConstraint<T>
    where C : IConstraint<T>, new()
    where D : IConstraint<T>, new()
    where E : IConstraint<T>, new()
    where F : IConstraint<T>, new()
    where G : IConstraint<T>, new()
{
    public static Or<T, C, D, E, F, G> Instance { get; } = new ();
    private static readonly C _c = new ();
    private static readonly D _d = new ();
    private static readonly E _e = new ();
    private static readonly F _f = new ();
    private static readonly G _g = new ();
    public Result<T, ErrorList> Check(T item)
        => _c.Check(item)
            .BindError(_ => _d.Check(item))
            .BindError(_ => _e.Check(item))
            .BindError(_ => _f.Check(item))
            .BindError(_ => _g.Check(item));
}
