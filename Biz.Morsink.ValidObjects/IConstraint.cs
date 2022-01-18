namespace Biz.Morsink.ValidObjects;

public interface IConstraint<T> 
{
    Result<T, ErrorList> Check(T item);
}
