namespace Biz.Morsink.Results
{
    public interface ISuccess
    {
        object Value { get; }
    }
    public interface ISuccess<out T> : ISuccess
    {
        new T Value { get; }
#if NET6_0_OR_GREATER
        object ISuccess.Value => Value!;
#endif
    }
}
