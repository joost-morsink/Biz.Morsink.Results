namespace Biz.Morsink.ValidObjects;

public interface IConstraint<T> 
{
    Result<T, ErrorList> Check(T item);
}

public interface IConstraint<T, R> : IConstraint<T>
{
    new Result<(T,R),ErrorList> Check(T item);
}