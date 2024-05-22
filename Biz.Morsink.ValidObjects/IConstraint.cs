namespace Biz.Morsink.ValidObjects;

public interface IConstraint<T> 
{
    Result<T, ErrorList> Check(T item);
}

public interface IConstraint<T, R> : IConstraint<T>
{
    Result<T, ErrorList> IConstraint<T>.Check(T item) => Check(item).Select(r => r.Item1);
    new Result<(T,R),ErrorList> Check(T item);
}