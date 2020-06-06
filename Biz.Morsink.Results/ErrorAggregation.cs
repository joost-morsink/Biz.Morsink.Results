using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Results
{
    public static class ErrorAggregation
    {
        public static ConcurrentDictionary<Type, Delegate> aggregators = new ConcurrentDictionary<Type, Delegate>();
        public static void Set<E>(Func<E, E, E> agg)
        {
            aggregators[typeof(E)] = agg;
        }

#nullable disable
        public static Func<E, E, E> Get<E>()
        {
            if (aggregators.TryGetValue(typeof(E), out var del))
                return (Func<E, E, E>)del;
            if (typeof(IErrorAggregable<E>).IsAssignableFrom(typeof(E)))
                return (Func<E, E, E>)aggregators.GetOrAdd(typeof(E), _ => new Func<E, E, E>((e, f) => ((IErrorAggregable<E>)e).Aggregate(f)));
            var par = GetGenericParameter(typeof(E), typeof(ImmutableList<>));
            if (par != null)
                return (Func<E, E, E>)aggregators.GetOrAdd(typeof(E), _ =>
                {
                    var p1 = Ex.Parameter(typeof(E));
                    var p2 = Ex.Parameter(typeof(E));
                    var type = typeof(ImmutableList<>).MakeGenericType(par);
                    var func = Ex.Lambda<Func<E, E, E>>(
                        Ex.Call(Ex.Convert(p1, type),
                            type.GetMethod(nameof(ImmutableList<object>.AddRange)),
                            Ex.Convert(p2, type)), p1, p2).Compile();
                    return func;
                });
            par = GetGenericParameter(typeof(E), typeof(IEnumerable<>));
            if (par != null)
                return (Func<E, E, E>)aggregators.GetOrAdd(typeof(E), _ =>
                {
                    var p1 = Ex.Parameter(typeof(E));
                    var p2 = Ex.Parameter(typeof(E));
                    var type = typeof(IEnumerable<>).MakeGenericType(par);
                    var func = Ex.Lambda<Func<E, E, E>>(
                        Ex.Call(typeof(Enumerable).GetMethod(nameof(Enumerable.Concat)).MakeGenericMethod(par),
                            Ex.Convert(p1, type),
                            Ex.Convert(p2, type)), p1, p2).Compile();
                    return func;
                });
            return (Func<E, E, E>)aggregators.GetOrAdd(typeof(E), _ => new Func<E, E, E>((e, f) => e));
        }
#nullable restore
        private static Type? GetGenericParameter(Type t, Type intface)
            => t.GetInterfaces().Prepend(t)
                .Where(i => i.GetGenericArguments().Length == 1 && i.GetGenericTypeDefinition() == intface)
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();

    }
}
