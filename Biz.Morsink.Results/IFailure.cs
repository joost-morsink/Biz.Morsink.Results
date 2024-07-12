namespace Biz.Morsink.Results
{
    public interface IFailure
    {
        object Error { get; }
    }
    public interface IFailure<out E> : IFailure
    {
        new E Error { get; }
#if NET6_0_OR_GREATER
        object IFailure.Error => Error!;
#endif
    }
}
