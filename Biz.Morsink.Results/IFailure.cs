namespace Biz.Morsink.Results;

public interface IFailure
{
    object Error { get; }
}
public interface IFailure<out E> : IFailure
{
    new E Error { get; }
    object IFailure.Error => Error!;
}
