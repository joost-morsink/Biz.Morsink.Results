using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Biz.Morsink.Results.Errors
{
    public static class ErrorExt
    {
        public static Result<T, ErrorList> Prefix<T>(this Result<T, ErrorList> result, object prefix)
            => result.SelectError(errors => errors.Select(error => error.Prefix(prefix)));
        public static Result<ImmutableArray<R>, ErrorList> CreateAll<T, R>(this IEnumerable<T> elements, Func<T, Result<R, ErrorList>> create)
            where T : notnull
            => elements.Select(e => create(e).Prefix(e)).Sequence();
        public static Result<ImmutableArray<R>, ErrorList> CreateAllWithIndex<T, R>(this IEnumerable<T> elements, Func<T, Result<R, ErrorList>> create)
            where T : notnull
            => elements.Select((e, i) => create(e).Prefix(i)).Sequence();
        public static Result<ImmutableArray<R>, ErrorList> CreateAllWithKey<T, R>(this IEnumerable<T> elements, Func<T, Result<R, ErrorList>> create, Func<T, string> keyFunc)
            where T : notnull
            => elements.Select(e => create(e).Prefix(keyFunc(e))).Sequence();
    }
}
