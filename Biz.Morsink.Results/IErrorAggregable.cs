namespace Biz.Morsink.Results
{
    public interface IErrorAggregable<E>
    {
        E Aggregate(E error);
    }
}
