using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Biz.Morsink.Results.Errors
{
    public struct ErrorList : IReadOnlyList<Error>, IErrorAggregable<ErrorList>
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

        public ErrorList Aggregate(ErrorList error)
            => new ErrorList(_errors.AddRange(error._errors));

        public IEnumerator<Error> GetEnumerator()
            => _errors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public ErrorList Select(Func<Error, Error> manipulate)
            => new ErrorList(_errors.Select(manipulate));
        public override string ToString()
            => string.Join(Environment.NewLine, _errors);
    }
}
