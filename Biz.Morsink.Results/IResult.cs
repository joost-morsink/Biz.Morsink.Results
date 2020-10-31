using System;
namespace Biz.Morsink.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }
        R SwitchUntyped<R>(Func<ISuccess, R> onSuccess, Func<IFailure, R> onError);
        void ActUntyped(Action<ISuccess> onSuccess, Action<IFailure> onError);
    }
    public interface IResult<out T, out E> : IResult
    {
        R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError);
        void Act(Action<T> onSuccess, Action<E> onError);
    }
}
