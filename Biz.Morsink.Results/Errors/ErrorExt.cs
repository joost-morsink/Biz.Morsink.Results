namespace Biz.Morsink.Results.Errors;

public static class ErrorExt
{
    public static Result<T, ErrorList> Prefix<T>(this Result<T, ErrorList> result, object prefix)
        => result.SelectError(errors => errors.Select(error => error.Prefix(prefix)));
    public static Result<T, ErrorList> Prefix<T>(this Result<T, ErrorList> result, params object[] prefix)
        => result.SelectError(errors => errors.Select(error => error.Prefix(prefix)));
    public static Result<ImmutableArray<R>, ErrorList> CreateAll<T, R>(this IEnumerable<T> elements, Func<T, Result<R, ErrorList>> create)
        where T : notnull
        => elements.Select(e => create(e).Prefix(e)).Sequence();
    public static Result<ImmutableArray<R>, ErrorList> CreateAllWithIndex<T, R>(this IEnumerable<T> elements, Func<T, Result<R, ErrorList>> create)
        where T : notnull
        => elements.Select((e, i) => create(e).Prefix(i)).Sequence();
    public static Result<ImmutableArray<R>, ErrorList> CreateAllWithKey<T, R>(this IEnumerable<T> elements,
                                                                              Func<T, Result<R, ErrorList>> create,
                                                                              Func<T, string> keyFunc)
        where T : notnull
        => elements.Select(e => create(e).Prefix(keyFunc(e))).Sequence();
    public static Result<T, ErrorList> IfValidThen<T>(this IEnumerable<Error> errors, Func<T> creator)
    {
        var errlist = errors.ToList();
        if (errlist.Count > 0)
            return new ErrorList(errlist);
        else
            return creator();
    }
    public static Result<T, ErrorList> IfValidThen<T>(this IEnumerable<Error> errors, Func<Result<T, ErrorList>> creator)
    {
        var errlist = errors.ToList();
        if (errlist.Count > 0)
            return new ErrorList(errlist);
        else
            return creator();
    }
    public static Result<T, ErrorList> StringToError<T>(this Result<T, string> result)
        => result.SelectError(e => new ErrorList(new[] { new Error(default, default, e) }));
    public static Result<T, ErrorList> Failure<T>(this Result.ForTypes<T, ErrorList> @for, Error error)
        => @for.Failure(new ErrorList(new[] { error }));
    public static Result<T, ErrorList> Failure<T>(this Result.ForTypes<T, ErrorList> @for, ErrorKey key, string? code, IErrorMessage message)
        => @for.Failure(new Error(key, code, message));
    public static Result<T, ErrorList> Failure<T>(this Result.ForTypes<T, ErrorList> @for, ErrorKey key, string? code, ErrorMessage message)
        => @for.Failure(new Error(key, code, message));
    public static ErrorList ToList(this Error singleError)
        => new (new[] { singleError });
}
