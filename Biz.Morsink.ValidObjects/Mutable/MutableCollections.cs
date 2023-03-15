using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Biz.Morsink.ValidObjects.Mutable;

public static class MutableCollectionsExt
{
    public static MutableList<T> ToMutableList<T>(this ImmutableList<T> list)
        => new (list);
    public static MutableList<T> ToMutableList<T>(this IEnumerable<T> list)
        => list is ImmutableList<T> imList ? imList.ToMutableList() : new (list.ToImmutableList());
    public static MutableSet<T> ToMutableSet<T>(this IImmutableSet<T> coll)
        => new (coll);
    public static MutableSet<T> ToMutableSet<T>(this IEnumerable<T> coll)
        => coll is IImmutableSet<T> imSet ? imSet.ToMutableSet() : new (coll.ToImmutableHashSet());
}

public class MutableList<T> : INotifyCollectionChanged, IReadOnlyList<T>, INotifyPropertyChanged
{
    private ImmutableList<T> _inner;
    public MutableList() : this(ImmutableList<T>.Empty) { }
    public MutableList(ImmutableList<T> inner)
    {
        _inner = inner;
        foreach (var x in _inner)
            if(x is INotifyPropertyChanged npc)
                npc.PropertyChanged += OnItemChanged;
    }

    private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged("Item");
    }
    public ImmutableList<T> ToImmutable()
        => _inner;
    public void Add(T item)
    {
        _inner = _inner.Add(item);
        if(item is INotifyPropertyChanged npc)
            npc.PropertyChanged += OnItemChanged;
        OnNotifyCollectionChanged(new (NotifyCollectionChangedAction.Add, item));
    }

    public void SetItem(int index, T item)
    {
        var old = _inner[index];
        if(old is INotifyPropertyChanged npc)
            npc.PropertyChanged -= OnItemChanged;
        _inner = _inner.SetItem(index, item);
        if(item is INotifyPropertyChanged npc2)
            npc2.PropertyChanged += OnItemChanged;
        OnNotifyCollectionChanged(new (NotifyCollectionChangedAction.Replace, old, item, index));
    }
    public void Remove(int index)
    {
        var old = _inner[index];
        if(old is INotifyPropertyChanged npc)
            npc.PropertyChanged -= OnItemChanged;
        _inner = _inner.RemoveAt(index);
        OnNotifyCollectionChanged(new (NotifyCollectionChangedAction.Remove));
    }
    public void Clear()
    {
        foreach(var x in _inner)
            if(x is INotifyPropertyChanged npc)
                npc.PropertyChanged -= OnItemChanged;
        _inner = _inner.Clear();
        OnNotifyCollectionChanged(new (NotifyCollectionChangedAction.Reset));
    }
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }
    public IEnumerator<T> GetEnumerator()
    {
        return _inner.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_inner).GetEnumerator();
    }
    public int Count => _inner.Count;
    public T this[int index] => _inner[ index];

    public void AddRange(IEnumerable<T> select)
    {
        foreach(var item in select)
            Add(item);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Result<ImmutableList<Vo>, ErrorList> AsResult<Vo>(IObjectValidator<ImmutableList<Vo>, ImmutableList<T>> validator)
        => validator.TryCreate(_inner);
}

public class MutableSet<T> : INotifyCollectionChanged, IReadOnlyCollection<T>
{
    private IImmutableSet<T> _inner;
    public MutableSet(IImmutableSet<T> inner)
    {
        _inner = inner;
    }

    public IImmutableSet<T> ToImmutable()
        => _inner;

    public void Add(T item)
    {
        _inner = _inner.Add(item);
        OnNotifyCollectionChanged(new(NotifyCollectionChangedAction.Add, item));
    }

    public void Remove(T item)
    {
        _inner = _inner.Remove(item);
        OnNotifyCollectionChanged(new(NotifyCollectionChangedAction.Remove));
    }

    public void Clear()
    {
        _inner = _inner.Clear();
        OnNotifyCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _inner).GetEnumerator();
    }

    public int Count => _inner.Count;
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected virtual void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
    {
        CollectionChanged?.Invoke(this, eventArgs);
    }

}