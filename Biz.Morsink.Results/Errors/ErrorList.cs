namespace Biz.Morsink.Results.Errors;

using System.Collections;
public readonly struct ErrorList : IReadOnlyList<Error>, IErrorAggregable<ErrorList>
{
    private readonly ImmutableList<Error> _errors;

    private ErrorList(ImmutableList<Error> errors)
    {
        _errors = errors;
    }
    public ErrorList(IEnumerable<Error> errors)
    {
        _errors = errors.ToImmutableList();
    }

    public Error this[int index] => _errors[index];
    public int Count => _errors.Count;
    public bool IsEmpty => Count == 0;

    public ErrorList Aggregate(ErrorList error)
        => new (_errors.AddRange(error._errors));

    public IEnumerator<Error> GetEnumerator()
        => _errors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public ErrorList Select(Func<Error, Error> manipulate)
        => new (_errors.Select(manipulate));
    public override string ToString()
        => string.Join(Environment.NewLine, _errors);

    public static implicit operator ErrorList(Error error)
        => new (new[] { error });
    public static ErrorList Create(params Error[] errors)
        => new (errors);

}
