using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
namespace Biz.Morsink.Results
{
    public static class Result
    {
        public struct ForErrorType<E>
        {
            public Result<T, E> Success<T>(T value)
                => new Result<T, E>.Success(value);
            public Result<T, E> Failure<T>(E error)
                => new Result<T, E>.Failure(error);
        }
        public static ForErrorType<E> ForError<E>()
            => default;
        public struct ForTypes<T, E>
        {
            public Result<T, E> Success(T value)
                => new Result<T, E>.Success(value);
            public Result<T, E> Failure(E error)
                => new Result<T, E>.Failure(error);
        }
        public static ForTypes<T, E> For<T, E>()
            => default;
        public static Result<U, E> Bind<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> f)
            => result.SelectMany(f);

        public static Result<R, E> Select<T, U, R, E>(this Result<(T, U), E> result, Func<T, U, R> f)
            => result.Select(v => f(v.Item1, v.Item2));
        public static Result<R, E> Select<T, U, V, R, E>(this Result<(T, U, V), E> result, Func<T, U, V, R> f)
            => result.Select(v => f(v.Item1, v.Item2, v.Item3));
        public static Result<R, E> Select<T, U, V, W, R, E>(this Result<(T, U, V, W), E> result, Func<T, U, V, W, R> f)
            => result.Select(v => f(v.Item1, v.Item2, v.Item3, v.Item4));
        public static Result<R, E> Select<T, U, V, W, X, R, E>(this Result<(T, U, V, W, X), E> result, Func<T, U, V, W, X, R> f)
            => result.Select(v => f(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5));
        public static Result<R, E> Select<T, U, V, W, X, Y, R, E>(this Result<(T, U, V, W, X, Y), E> result, Func<T, U, V, W, X, Y, R> f)
            => result.Select(v => f(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6));
        public static Result<R, E> Select<T, U, V, W, X, Y, Z, R, E>(this Result<(T, U, V, W, X, Y, Z), E> result, Func<T, U, V, W, X, Y, Z, R> f)
            => result.Select(v => f(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6, v.Item7));

        private static T GetValue<T, E>(this Result<T, E> o)
            => ((ISuccess<T>)o).Value;

        public static Result<R, E> Apply<T, R, E>(this Result<T, E> result, Result<Func<T, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { result, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(result.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, R, E>(this (Result<T, E>, Result<U, E>) results, Result<Func<T, U, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>) results, Result<Func<T, U, V, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>) results, Result<Func<T, U, V, W, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, X, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>) results, Result<Func<T, U, V, W, X, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, X, Y, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>) results, Result<Func<T, U, V, W, X, Y, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(), results.Item6.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, X, Y, Z, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>, Result<Z, E>) results, Result<Func<T, U, V, W, X, Y, Z, R>, E> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, results.Item7, f }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(), results.Item6.GetValue(), results.Item7.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }

        public static Result<(T, U), E> Sequence<T, U, E>(this (Result<T, E>, Result<U, E>) results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<(T, U), E>();
            var failures = new object[] { results.Item1, results.Item2 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success((results.Item1.GetValue(), results.Item2.GetValue()));
            else
                return res.Failure(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<(T, U, V), E> Sequence<T, U, V, E>(this (Result<T, E>, Result<U, E>, Result<V, E>) results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<(T, U, V), E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success((results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue()));
            else
                return res.Failure(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<(T, U, V, W), E> Sequence<T, U, V, W, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>) results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<(T, U, V, W), E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success((results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue()));
            else
                return res.Failure(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<(T, U, V, W, X), E> Sequence<T, U, V, W, X, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>) results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<(T, U, V, W, X), E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success((results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue()));
            else
                return res.Failure(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<(T, U, V, W, X, Y), E> Sequence<T, U, V, W, X, Y, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>) results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<(T, U, V, W, X, Y), E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success((results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(), results.Item6.GetValue()));
            else
                return res.Failure(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<(T, U, V, W, X, Y, Z), E> Sequence<T, U, V, W, X, Y, Z, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>, Result<Z, E>) results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<(T, U, V, W, X, Y, Z), E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, results.Item7 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success((results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(), results.Item6.GetValue(), results.Item7.GetValue()));
            else
                return res.Failure(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }

        public static Result<R, E> Apply<T, U, R, E>(this (Result<T, E>, Result<U, E>) results, Func<T, U, R> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f(results.Item1.GetValue(), results.Item2.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>) results, Func<T, U, V, R> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>) results, Func<T, U, V, W, R> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, X, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>) results, Func<T, U, V, W, X, R> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, X, Y, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>) results, Func<T, U, V, W, X, Y, R> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(), results.Item6.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }
        public static Result<R, E> Apply<T, U, V, W, X, Y, Z, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>, Result<Z, E>) results, Func<T, U, V, W, X, Y, Z, R> f, Func<E, E, E>? errorAggregator = null)
        {
            var res = ForError<E>();
            var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, results.Item7 }.OfType<IFailure<E>>().ToList();
            if (failures.Count == 0)
                return res.Success(f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(), results.Item6.GetValue(), results.Item7.GetValue()));
            else
                return res.Failure<R>(failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
        }

        public static IEnumerable<T> DropFailures<T, E>(this IEnumerable<Result<T, E>> results, Action<E>? onError = null)
        {
            if (onError == null)
                return results.OfType<ISuccess<T>>().Select(s => s.Value);
            else
            {
                var result = new List<T>();
                foreach (var r in results)
                    r.Act(result.Add, onError);
                return result;
            }
        }
        public static Result<ImmutableArray<T>, E> Sequence<T, E>(this IEnumerable<Result<T, E>> results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<ImmutableArray<T>, E>();
            var failures = results.OfType<IFailure<E>>();
            if (failures.Any())
                return res.Failure(failures.Select(f => f.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
            else
                return res.Success(results.OfType<ISuccess<T>>().Select(s => s.Value).ToImmutableArray());
        }
        public static Result<ImmutableList<T>, E> SequenceList<T, E>(this IEnumerable<Result<T, E>> results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<ImmutableList<T>, E>();
            var failures = results.OfType<IFailure<E>>();
            if (failures.Any())
                return res.Failure(failures.Select(f => f.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
            else
                return res.Success(results.OfType<ISuccess<T>>().Select(s => s.Value).ToImmutableList());
        }
        public static Result<IImmutableSet<T>, E> SequenceSet<T, E>(this IEnumerable<Result<T, E>> results, Func<E, E, E>? errorAggregator = null)
        {
            var res = For<IImmutableSet<T>, E>();
            var failures = results.OfType<IFailure<E>>();
            if (failures.Any())
                return res.Failure(failures.Select(f => f.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>()));
            else
                return res.Success(results.OfType<ISuccess<T>>().Select(s => s.Value).ToImmutableHashSet());
        }
        public static ValueTask<Result<U, E>> SelectAsync<T, U, E>(this Result<T, E> result, Func<T, Task<U>> f)
        {
            var res = ForError<E>();
            return result.Switch<ValueTask<Result<U, E>>>(async t => res.Success(await f(t)), e => new ValueTask<Result<U, E>>(res.Failure<U>(e)));
        }
        public static ValueTask<Result<U, E>> BindAsync<T, U, E>(this Result<T, E> result, Func<T, Task<Result<U, E>>> f)
        {
            var res = ForError<E>();
            return result.Switch<ValueTask<Result<U, E>>>(async t => await f(t), e => new ValueTask<Result<U, E>>(res.Failure<U>(e)));
        }
        public struct Caster<T, E>
            where T : notnull
        {
            private readonly Result<T, E> _result;

            public Caster(Result<T, E> result)
            {
                _result = result;
            }

            public Result<U, E> To<U>()
                => _result.Select(t => (U)(object)t);
            public Result<U, E> TryTo<U>(E error)
            {
                var res = For<U, E>();
                return _result.Switch(v => v is U u ? res.Success(u) : res.Failure(error), e => res.Failure(e));
            }
        }
        public static Caster<T, E> Cast<T, E>(this Result<T, E> result)
            where T : notnull
            => new Caster<T, E>(result);

        public static T GetOrThrow<T, E>(this Result<T, E> result)
            => result.Switch(s => s, _ => throw new InvalidOperationException());
        public static T? GetOrNull<T, E>(this Result<T, E> result)
            where T : class
            => result.Switch<T?>(s => s, _ => null);
        public static T? GetOrNullValue<T, E>(this Result<T, E> result)
            where T : struct
            => result.Switch<T?>(s => s, _ => default);
        public static Result<T, E> ToResult<T, E>(this IResult<T, E> result)
            => result as Result<T, E>
            ?? result.Switch<Result<T, E>>(s => new Result<T, E>.Success(s), e => new Result<T, E>.Failure(e));
    }
    public abstract class Result<T, E> : IResult<T, E>
    {
        private Result() { }
        public abstract Result<U, E> Select<U>(Func<T, U> f);
        public abstract Result<U, E> SelectMany<U>(Func<T, Result<U, E>> f);
        public abstract Result<V, E> SelectMany<U, V>(Func<T, Result<U, E>> f, Func<T, U, V> g);
        public abstract Result<T, F> SelectError<F>(Func<E, F> f);
        public abstract Result<T, E> Where(Func<T, bool> predicate, E error);
        public abstract Result<T, E> Where(Func<T, bool> predicate, Func<E> errorCreator);
        public abstract R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError);
        public abstract void Act(Action<T> onSuccess, Action<E> onError);

        public class Success : Result<T, E>, ISuccess<T>
        {
            public Success(T value)
            {
                Value = value;
            }

            public T Value { get; }
            public override Result<U, E> Select<U>(Func<T, U> f)
                => new Result<U, E>.Success(f(Value));
            public override Result<U, E> SelectMany<U>(Func<T, Result<U, E>> f)
                => f(Value);
            public override Result<V, E> SelectMany<U, V>(Func<T, Result<U, E>> f, Func<T, U, V> g)
                => f(Value).Select(u => g(Value, u));
            public override Result<T, F> SelectError<F>(Func<E, F> f)
                => new Result<T, F>.Success(Value);
            public override Result<T, E> Where(Func<T, bool> predicate, E error)
                => predicate(Value) ? (Result<T, E>)this : new Failure(error);
            public override Result<T, E> Where(Func<T, bool> predicate, Func<E> errorCreator)
                => predicate(Value) ? (Result<T, E>)this : new Failure(errorCreator());
            public override void Act(Action<T> onSuccess, Action<E> onError)
                => onSuccess(Value);
            public override R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError)
                => onSuccess(Value);
        }
        public class Failure : Result<T, E>, IFailure<E>
        {
            public Failure(E error)
            {
                Error = error;
            }

            public E Error { get; }

            public override Result<U, E> Select<U>(Func<T, U> f)
                => new Result<U, E>.Failure(Error);
            public override Result<U, E> SelectMany<U>(Func<T, Result<U, E>> f)
                => new Result<U, E>.Failure(Error);
            public override Result<V, E> SelectMany<U, V>(Func<T, Result<U, E>> f, Func<T, U, V> g)
                => new Result<V, E>.Failure(Error);
            public override Result<T, F> SelectError<F>(Func<E, F> f)
                => new Result<T, F>.Failure(f(Error));
            public override Result<T, E> Where(Func<T, bool> predicate, E error)
                => this;
            public override Result<T, E> Where(Func<T, bool> predicate, Func<E> errorCreator)
                => this;
            public override R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError)
                => onError(Error);
            public override void Act(Action<T> onSuccess, Action<E> onError)
                => onError(Error);
        }
    }
}
