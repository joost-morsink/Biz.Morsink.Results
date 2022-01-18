namespace Biz.Morsink.ValidObjects.Constraints;

public class MaxLength<L> : IConstraint<string>
    where L : INat
{
    public static MaxLength<L> Instance { get; } = new ();
    private static readonly ErrorList Error = ConstraintErrors.Current.MaxLength("String", L.Value).ToList();
    public Result<string, ErrorList> Check(string item)
    {
        if (item.Length > L.Value)
            return Error;
        else
            return item;
    }
}
public class MaxLength<L, E> :
    IConstraint<ImmutableList<E>>,
    IConstraint<ImmutableArray<E>>,
    IConstraint<ImmutableQueue<E>>,
    IConstraint<ImmutableStack<E>>,
    IConstraint<ImmutableHashSet<E>>,
    IConstraint<ImmutableSortedSet<E>>,
    IConstraint<IImmutableList<E>>,
    IConstraint<IImmutableSet<E>>
    where L : INat
{
    private Result<T, ErrorList> CollectionCheck<T>(T item, string type)
        where T : IReadOnlyCollection<E>
    {
        if (item.Count > L.Value)
            return ConstraintErrors.Current.MaxLength(type, L.Value).ToList();
        else
            return item;
    }
    private Result<T, ErrorList> EnumerableCheck<T>(T item, string type)
        where T : IEnumerable<E>
    {
        if (item.Count() > L.Value)
            return ConstraintErrors.Current.MaxLength(type, L.Value).ToList();
        else
            return item;
    }
    public Result<ImmutableList<E>, ErrorList> Check(ImmutableList<E> item)
        => CollectionCheck(item, "List");
    public Result<ImmutableArray<E>, ErrorList> Check(ImmutableArray<E> item)
        => CollectionCheck(item, "Array");
    public Result<ImmutableQueue<E>, ErrorList> Check(ImmutableQueue<E> item)
        => EnumerableCheck(item, "Empty");
    public Result<ImmutableStack<E>, ErrorList> Check(ImmutableStack<E> item)
        => EnumerableCheck(item, "Stack");
    public Result<ImmutableHashSet<E>, ErrorList> Check(ImmutableHashSet<E> item)
        => CollectionCheck(item, "Set");
    public Result<ImmutableSortedSet<E>, ErrorList> Check(ImmutableSortedSet<E> item)
        => CollectionCheck(item, "Set");
    public Result<IImmutableList<E>, ErrorList> Check(IImmutableList<E> item)
        => CollectionCheck(item, "List");
    public Result<IImmutableSet<E>, ErrorList> Check(IImmutableSet<E> item)
        => CollectionCheck(item, "Set");
}
