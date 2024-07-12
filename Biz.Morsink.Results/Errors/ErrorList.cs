using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Biz.Morsink.Results.Errors
{
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

        public Error this[int index] 
            => _errors?[index] ?? throw new ArgumentOutOfRangeException(nameof(index));

        public int Count => _errors?.Count ?? 0;
        public bool IsEmpty => Count == 0;

        public ErrorList Aggregate(ErrorList error)
            => new ErrorList((_errors ?? ImmutableList<Error>.Empty).AddRange(error._errors));

        public IEnumerator<Error> GetEnumerator()
            => (_errors ?? Enumerable.Empty<Error>()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public ErrorList Select(Func<Error, Error> manipulate)
            => _errors is null
                ? new ErrorList(ImmutableList<Error>.Empty)
                : new ErrorList(_errors.Select(manipulate));

        public override string ToString()
            => string.Join(Environment.NewLine, _errors);

        public static implicit operator ErrorList(Error error)
            => new ErrorList(new[] {error});

        public static ErrorList Create(params Error[] errors)
            => new ErrorList(errors);
    }
}