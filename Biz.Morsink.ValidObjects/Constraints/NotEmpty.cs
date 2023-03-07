namespace Biz.Morsink.ValidObjects.Constraints;

public class NotEmpty : IConstraint<string>
{
    public static NotEmpty Instance { get; } = new ();
    private static readonly ErrorList Error = ConstraintErrors.Current.NotEmpty("String").ToList();
    public Result<string, ErrorList> Check(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            return Error;
        else
            return item;
    }
}


public class NotEmpty<E> : 
    IConstraint<ImmutableList<E>>,
    IConstraint<ImmutableArray<E>>,
    IConstraint<ImmutableQueue<E>>,
    IConstraint<ImmutableStack<E>>,
    IConstraint<ImmutableHashSet<E>>, 
    IConstraint<ImmutableSortedSet<E>>,
    IConstraint<IImmutableList<E>>,
    IConstraint<IImmutableSet<E>>
{
    private Result<T, ErrorList> CollectionCheck<T>(T item, string type)
        where T : IReadOnlyCollection<E>
    {
        if (item.Count == 0)
            return ConstraintErrors.Current.NotEmpty(type).ToList();
        else
            return item;    
    }
    private Result<T, ErrorList> EnumerableCheck<T>(T item, string type)
        where T : IEnumerable<E>
    {
        if (!item.Any())
            return ConstraintErrors.Current.NotEmpty(type).ToList();
        else
            return item;    
    }

    public Result<ImmutableList<E>, ErrorList> Check(ImmutableList<E> item)
        => CollectionCheck(item, "List");
    public Result<ImmutableArray<E>, ErrorList> Check(ImmutableArray<E> item)
        => CollectionCheck(item, "Array");
    public Result<ImmutableQueue<E>, ErrorList> Check(ImmutableQueue<E> item)
        => EnumerableCheck(item, "Queue");
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

public class NotEmpty<K, V> : 
    IConstraint<ImmutableDictionary<K, V>>,
    IConstraint<ImmutableSortedDictionary<K,V>>,
    IConstraint<IImmutableDictionary<K,V>>
    where K : notnull
{
    private Result<T, ErrorList> CollectionCheck<T>(T item)
        where T : IReadOnlyCollection<KeyValuePair<K,V>>
    {
        if (item.Count == 0)
            return ConstraintErrors.Current.NotEmpty("Dictionary").ToList();
        else
            return item;    
    }

    public Result<ImmutableDictionary<K, V>, ErrorList> Check(ImmutableDictionary<K, V> item)
        => CollectionCheck(item);
    public Result<ImmutableSortedDictionary<K, V>, ErrorList> Check(ImmutableSortedDictionary<K, V> item)
        => CollectionCheck(item);
    public Result<IImmutableDictionary<K, V>, ErrorList> Check(IImmutableDictionary<K, V> item)
        => CollectionCheck(item);
}