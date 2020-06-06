namespace Biz.Morsink.Results
{
    public interface ISuccess
    {
        object Value { get; }
    }
    public interface ISuccess<out T> : ISuccess
    {
        new T Value { get; }
        object ISuccess.Value => Value!;
    }
}
