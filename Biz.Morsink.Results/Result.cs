namespace Biz.Morsink.Results;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
    public static Result<T, E> BindIgnore<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> f)
        => result.Bind(t => f(t).Select(_ => t));

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

    public static (ImmutableList<ISuccess<T>>, ImmutableList<IFailure<E>>) Split<T, E>(this IEnumerable<Result<T, E>> results)
    {
        var successes =  ImmutableList<ISuccess<T>>.Empty.ToBuilder();
        var failures = ImmutableList<IFailure<E>>.Empty.ToBuilder();
        foreach (var r in results)
        {
            if(r is ISuccess<T> s)
                successes.Add(s);
            if(r is IFailure<E> f)
                failures.Add(f);
        }
        return (successes.ToImmutable(), failures.ToImmutable());
    }
    public static Result<R, E> Apply<T, R, E>(this Result<T, E> result, Result<Func<T, R>, E> f, Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { result, f }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f.GetValue()(result.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, R, E>(this (Result<T, E>, Result<U, E>) results, Result<Func<T, U, R>, E> f, Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, f }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>) results,
                                                    Result<Func<T, U, V, R>, E> f,
                                                    Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, f }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>) results,
                                                       Result<Func<T, U, V, W, R>, E> f,
                                                       Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, f }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, X, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>) results,
                                                          Result<Func<T, U, V, W, X, R>, E> f,
                                                          Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, f }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(),
                results.Item5.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, X, Y, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>) results,
                                                             Result<Func<T, U, V, W, X, Y, R>, E> f,
                                                             Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, f }.OfType<IFailure<E>>()
            .ToList();
        if (failures.Count == 0)
            return f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(),
                results.Item5.GetValue(), results.Item6.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, X, Y, Z, R, E>(
        this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>, Result<Z, E>) results,
        Result<Func<T, U, V, W, X, Y, Z, R>, E> f,
        Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, results.Item7, f }
            .OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f.GetValue()(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(),
                results.Item5.GetValue(), results.Item6.GetValue(), results.Item7.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }

    public static Result<R, E> Apply<T, U, R, E>(this (Result<T, E>, Result<U, E>) results, Func<T, U, R> f, Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2 }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f(results.Item1.GetValue(), results.Item2.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>) results,
                                                    Func<T, U, V, R> f,
                                                    Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3 }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>) results,
                                                       Func<T, U, V, W, R> f,
                                                       Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4 }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, X, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>) results,
                                                          Func<T, U, V, W, X, R> f,
                                                          Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5 }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, X, Y, R, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>) results,
                                                             Func<T, U, V, W, X, Y, R> f,
                                                             Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6 }.OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(),
                results.Item6.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
    }
    public static Result<R, E> Apply<T, U, V, W, X, Y, Z, R, E>(
        this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>, Result<Z, E>) results,
        Func<T, U, V, W, X, Y, Z, R> f,
        Func<E, E, E>? errorAggregator = null)
    {
        var failures = new object[] { results.Item1, results.Item2, results.Item3, results.Item4, results.Item5, results.Item6, results.Item7 }
            .OfType<IFailure<E>>().ToList();
        if (failures.Count == 0)
            return f(results.Item1.GetValue(), results.Item2.GetValue(), results.Item3.GetValue(), results.Item4.GetValue(), results.Item5.GetValue(),
                results.Item6.GetValue(), results.Item7.GetValue());
        else
            return failures.Select(fail => fail.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
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
        var (successes, failures) = results.Split();
        if (failures.Count > 0)
            return failures.Select(f => f.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
        else
            return successes.Select(s => s.Value).ToImmutableArray();
    }
    public static Result<ImmutableList<T>, E> SequenceList<T, E>(this IEnumerable<Result<T, E>> results, Func<E, E, E>? errorAggregator = null)
    {
        var (successes, failures) = results.Split();
        if (failures.Count > 0)
            return failures.Select(f => f.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
        else
            return successes.Select(s => s.Value).ToImmutableList();
    }
    public static Result<IImmutableSet<T>, E> SequenceSet<T, E>(this IEnumerable<Result<T, E>> results, Func<E, E, E>? errorAggregator = null)
    {
        var (successes, failures) = results.Split();
        if (failures.Count > 0)
            return failures.Select(f => f.Error).Aggregate(errorAggregator ?? ErrorAggregation.Get<E>());
        else
            return successes.Select(s => s.Value).ToImmutableHashSet();
    }

    public static Result<(T, U), E> Sequence<T, U, E>(this (Result<T, E>, Result<U, E>) result, Func<E, E, E>? errorAggregator = null)
        => result.Apply((t, u) => (t, u), errorAggregator);
    public static Result<(T, U, V), E> Sequence<T, U, V, E>(this (Result<T, E>, Result<U, E>, Result<V, E>) result, Func<E, E, E>? errorAggregator = null)
        => result.Apply((t, u, v) => (t, u, v), errorAggregator);
    public static Result<(T, U, V, W), E> Sequence<T, U, V, W, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>) result,
                                                                  Func<E, E, E>? errorAggregator = null)
        => result.Apply((t, u, v, w) => (t, u, v, w), errorAggregator);
    public static Result<(T, U, V, W, X), E> Sequence<T, U, V, W, X, E>(this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>) result,
                                                                        Func<E, E, E>? errorAggregator = null)
        => result.Apply((t, u, v, w, x) => (t, u, v, w, x), errorAggregator);
    public static Result<(T, U, V, W, X, Y), E> Sequence<T, U, V, W, X, Y, E>(
        this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>) result,
        Func<E, E, E>? errorAggregator = null)
        => result.Apply((t, u, v, w, x, y) => (t, u, v, w, x, y), errorAggregator);
    public static Result<(T, U, V, W, X, Y, Z), E> Sequence<T, U, V, W, X, Y, Z, E>(
        this (Result<T, E>, Result<U, E>, Result<V, E>, Result<W, E>, Result<X, E>, Result<Y, E>, Result<Z, E>) result,
        Func<E, E, E>? errorAggregator = null)
        => result.Apply((t, u, v, w, x, y, z) => (t, u, v, w, x, y, z), errorAggregator);

    public static ValueTask<Result<U, E>> SelectAsync<T, U, E>(this Result<T, E> result, Func<T, Task<U>> f)
    {
        var res = ForError<E>();
        return result.Switch<ValueTask<Result<U, E>>>(async t => res.Success(await f(t)), e => new ValueTask<Result<U, E>>(res.Failure<U>(e)));
    }
    public static ValueTask<Result<U, E>> SelectAsyncValue<T, U, E>(this Result<T, E> result, Func<T, ValueTask<U>> f)
    {
        var res = ForError<E>();
        return result.Switch<ValueTask<Result<U, E>>>(async t => res.Success(await f(t)), e => new ValueTask<Result<U, E>>(res.Failure<U>(e)));
    }

    public static ValueTask<Result<T, E>> TraverseAsync<T, E>(this Result<ValueTask<T>, E> result)
        => result.SelectAsyncValue(x => x);
    public static ValueTask<Result<T, E>> TraverseAsync<T, E>(this Result<Task<T>, E> result)
        => result.SelectAsyncValue(async x => await x);

    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, R, E>(this (Task<Result<T, E>>, Task<Result<U, E>>) results, Func<T, U, Task<R>> func)
        => await (await results.Item1, await results.Item2).Sequence().SelectAsync(t => func(t.Item1, t.Item2));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, R, E>(this (Task<Result<T, E>>, Task<Result<U, E>>, Task<Result<V, E>>) results,
                                                                          Func<T, U, V, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3).Sequence().SelectAsync(t => func(t.Item1, t.Item2, t.Item3));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, R, E>(
        this (Task<Result<T, E>>, Task<Result<U, E>>, Task<Result<V, E>>, Task<Result<W, E>>) results,
        Func<T, U, V, W, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4).Sequence()
            .SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, X, R, E>(
        this (Task<Result<T, E>>, Task<Result<U, E>>, Task<Result<V, E>>, Task<Result<W, E>>, Task<Result<X, E>>) results,
        Func<T, U, V, W, X, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4, await results.Item5).Sequence()
            .SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, X, Y, R, E>(
        this (Task<Result<T, E>>, Task<Result<U, E>>, Task<Result<V, E>>, Task<Result<W, E>>, Task<Result<X, E>>, Task<Result<Y, E>>) results,
        Func<T, U, V, W, X, Y, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4, await results.Item5, await results.Item6).Sequence()
            .SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, X, Y, Z, R, E>(
        this (Task<Result<T, E>>, Task<Result<U, E>>, Task<Result<V, E>>, Task<Result<W, E>>, Task<Result<X, E>>, Task<Result<Y, E>>, Task<Result<Z, E>>)
            results,
        Func<T, U, V, W, X, Y, Z, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4, await results.Item5, await results.Item6,
            await results.Item7).Sequence().SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, R, E>(this (ValueTask<Result<T, E>>, ValueTask<Result<U, E>>) results,
                                                                       Func<T, U, Task<R>> func)
        => await (await results.Item1, await results.Item2).Sequence().SelectAsync(t => func(t.Item1, t.Item2));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, R, E>(
        this (ValueTask<Result<T, E>>, ValueTask<Result<U, E>>, ValueTask<Result<V, E>>) results,
        Func<T, U, V, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3).Sequence().SelectAsync(t => func(t.Item1, t.Item2, t.Item3));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, R, E>(
        this (ValueTask<Result<T, E>>, ValueTask<Result<U, E>>, ValueTask<Result<V, E>>, ValueTask<Result<W, E>>) results,
        Func<T, U, V, W, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4).Sequence()
            .SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, X, R, E>(
        this (ValueTask<Result<T, E>>, ValueTask<Result<U, E>>, ValueTask<Result<V, E>>, ValueTask<Result<W, E>>, ValueTask<Result<X, E>>) results,
        Func<T, U, V, W, X, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4, await results.Item5).Sequence()
            .SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, X, Y, R, E>(
        this (ValueTask<Result<T, E>>, ValueTask<Result<U, E>>, ValueTask<Result<V, E>>, ValueTask<Result<W, E>>, ValueTask<Result<X, E>>,
            ValueTask<Result<Y, E>>) results,
        Func<T, U, V, W, X, Y, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4, await results.Item5, await results.Item6).Sequence()
            .SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));
    public static async ValueTask<Result<R, E>> ApplyAsync<T, U, V, W, X, Y, Z, R, E>(
        this (ValueTask<Result<T, E>>, ValueTask<Result<U, E>>, ValueTask<Result<V, E>>, ValueTask<Result<W, E>>, ValueTask<Result<X, E>>,
            ValueTask<Result<Y, E>>, ValueTask<Result<Z, E>>) results,
        Func<T, U, V, W, X, Y, Z, Task<R>> func)
        => await (await results.Item1, await results.Item2, await results.Item3, await results.Item4, await results.Item5, await results.Item6,
            await results.Item7).Sequence().SelectAsync(t => func(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));

    public static ValueTask<Result<U, E>> BindAsync<T, U, E>(this Result<T, E> result, Func<T, Task<Result<U, E>>> f)
    {
        return result.Switch<ValueTask<Result<U, E>>>(async t => await f(t), e => new ValueTask<Result<U, E>>(e));
    }
    public static ValueTask<Result<U, E>> BindAsyncValue<T, U, E>(this Result<T, E> result, Func<T, ValueTask<Result<U, E>>> f)
    {
        return result.Switch<ValueTask<Result<U, E>>>(async t => await f(t), e => new ValueTask<Result<U, E>>(e));
    }
    public readonly struct Caster<T, E>
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
            return _result.Switch(v => v is U u ? res.Success(u) : error, e => e);
        }
    }
    public static Caster<T, E> Cast<T, E>(this Result<T, E> result)
        where T : notnull
        => new (result);

    public static T GetOrThrow<T, E>(this Result<T, E> result)
        => result.Switch(s => s, _ => throw new InvalidOperationException());
    public static T? GetOrNull<T, E>(this Result<T, E> result)
        where T : class
        => result.Switch<T?>(s => s, _ => null);
    public static T? GetOrNullValue<T, E>(this Result<T, E> result)
        where T : struct
        => result.Switch<T?>(s => s, _ => default);
    public static T? GetOrDefault<T, E>(this Result<T, E> result, T? @default = default)
        where T : notnull
        => result.Switch(v => v, _ => @default);
    public static bool TryGetValue<T, E>(this Result<T, E> result, [NotNullWhen(returnValue: true)] out T? val)
        where T : class
    {
        if (result is ISuccess<T> suc)
        {
            val = suc.Value;
            return true;
        }
        else
        {
            val = default;
            return false;
        }
    }

    public static Result<T, E> ToResult<T, E>(this IResult<T, E> result)
        => result as Result<T, E>
            ?? result.Switch<Result<T, E>>(s => s, e => e);

    public static Result<T, F> SetError<T, E, F>(this Result<T, E> result, F error)
        => result.SelectError(_ => error);

    public static Result<T, string> FirstToResult<T>(this IEnumerable<T> src)
    {
        foreach (var x in src)
            return x;
        return "Sequence is empty, element not found.";
    }
    public static Result<T, E> FirstToResult<T,E>(this IEnumerable<T> src, Func<E> errorCreator)
    {
        foreach (var x in src)
            return x;
        return errorCreator();
    }
    public static Result<T, E> FirstToResult<T,E>(this IEnumerable<T> src, E error)
    {
        foreach (var x in src)
            return x;
        return error;
    }
    public static Result<T, string> ParseEnum<T>(this string str, bool caseSensitive = true)
        where T : struct, Enum
    {
        if (Enum.TryParse<T>(str, !caseSensitive, out var result))
            return result;
        return "Cannot parse enum value.";
    }

    public delegate bool TryParseDelegateCls<T>(string val, [NotNullWhen(returnValue: true)] out T? output) where T : class;

    public delegate bool TryParseDelegateStr<T>(string val, [NotNullWhen(returnValue: true)] out T? output) where T : struct;

    public static Result<T, string> ParseResult<T>(string val, TryParseDelegateCls<T> tp)
        where T : class
    {
        if (tp(val, out var result))
            return result;
        return "Parse failure.";
    }
    public static Result<T, string> ParseResult<T>(string val, TryParseDelegateStr<T> tp)
        where T : struct
    {
        if (tp(val, out var result))
            return result.Value;
        return "Parse failure.";
    }
    public static R Switch<T, E, R>(this IResult<T, E> result, Func<T, R> onSuccess, Func<E, R> onFailure)
        => result switch
        {
            ISuccess<T> suc => onSuccess(suc.Value),
            IFailure<E> fai => onFailure(fai.Error),
            _ => throw new InvalidOperationException()
        };
}
public abstract class Result<T, E> : IResult<T, E>
{
    private Result() { }
    public abstract bool IsSuccess { get; }
    public abstract Result<U, E> Select<U>(Func<T, U> f);
    public abstract Result<U, E> SelectMany<U>(Func<T, Result<U, E>> f);
    public abstract Result<V, E> SelectMany<U, V>(Func<T, Result<U, E>> f, Func<T, U, V> g);
    public abstract Result<T, F> SelectError<F>(Func<E, F> f);
    public abstract Result<T, E> Where(Func<T, bool> predicate, E error);
    public abstract Result<T, E> Where(Func<T, bool> predicate, Func<E> errorCreator);
    public abstract R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError);
    public abstract void Act(Action<T> onSuccess, Action<E> onError);
    public abstract R SwitchUntyped<R>(Func<ISuccess, R> onSuccess, Func<IFailure, R> onError);
    public abstract void ActUntyped(Action<ISuccess> onSuccess, Action<IFailure> onError);

    public class Success : Result<T, E>, ISuccess<T>
    {
        public Success(T value)
        {
            Value = value;
        }

        public T Value { get; }
        public override bool IsSuccess => true;
        public override Result<U, E> Select<U>(Func<T, U> f)
            => f(Value);
        public override Result<U, E> SelectMany<U>(Func<T, Result<U, E>> f)
            => f(Value);
        public override Result<V, E> SelectMany<U, V>(Func<T, Result<U, E>> f, Func<T, U, V> g)
            => f(Value).Select(u => g(Value, u));
        public override Result<T, F> SelectError<F>(Func<E, F> f)
            => Value;
        public override Result<T, E> Where(Func<T, bool> predicate, E error)
            => predicate(Value) ? this : error;
        public override Result<T, E> Where(Func<T, bool> predicate, Func<E> errorCreator)
            => predicate(Value) ? this : errorCreator();
        public override void Act(Action<T> onSuccess, Action<E> onError)
            => onSuccess(Value);
        public override R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError)
            => onSuccess(Value);
        public override R SwitchUntyped<R>(Func<ISuccess, R> onSuccess, Func<IFailure, R> onError)
            => onSuccess(this);
        public override void ActUntyped(Action<ISuccess> onSuccess, Action<IFailure> onError)
            => onSuccess(this);
    }
    public class Failure : Result<T, E>, IFailure<E>
    {
        public Failure(E error)
        {
            Error = error;
        }

        public E Error { get; }

        public override bool IsSuccess => false;
        public override Result<U, E> Select<U>(Func<T, U> f)
            => Error;
        public override Result<U, E> SelectMany<U>(Func<T, Result<U, E>> f)
            => Error;
        public override Result<V, E> SelectMany<U, V>(Func<T, Result<U, E>> f, Func<T, U, V> g)
            => Error;
        public override Result<T, F> SelectError<F>(Func<E, F> f)
            => f(Error);
        public override Result<T, E> Where(Func<T, bool> predicate, E error)
            => this;
        public override Result<T, E> Where(Func<T, bool> predicate, Func<E> errorCreator)
            => this;
        public override R Switch<R>(Func<T, R> onSuccess, Func<E, R> onError)
            => onError(Error);
        public override void Act(Action<T> onSuccess, Action<E> onError)
            => onError(Error);
        public override R SwitchUntyped<R>(Func<ISuccess, R> onSuccess, Func<IFailure, R> onError)
            => onError(this);
        public override void ActUntyped(Action<ISuccess> onSuccess, Action<IFailure> onError)
            => onError(this);
    }

    public static implicit operator Result<T, E>(T value)
        => new Success(value);
    public static implicit operator Result<T, E>(E error)
        => new Failure(error);
}
