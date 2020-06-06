using System;
namespace Biz.Morsink.Results
{
    public interface IResult<out T, out E>
    {
        R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError);
        void Act(Action<T> onSuccess, Action<E> onError);
    }
}
