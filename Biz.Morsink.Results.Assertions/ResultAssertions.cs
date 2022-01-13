using FluentAssertions.Primitives;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Linq;

namespace Biz.Morsink.Results.Assertions
{
    public static class ResultAssertions
    {
        public static ResultAssertions<T, F> Should<T, F>(this Result<T, F> subject)
            => new (subject);
    }
    public class ResultAssertions<T, F> : ReferenceTypeAssertions<Result<T, F>, ResultAssertions<T, F>>
    {
        public ResultAssertions(Result<T, F> subject) 
        {
            Subject = subject;
        }
        public AndWhichConstraint<ResultAssertions<T, F>, T> BeSuccess()
        {
            Execute.Assertion.ForCondition(Subject is ISuccess<T>)
                .FailWith("subject is not a success value");
            return new AndWhichConstraint<ResultAssertions<T, F>, T>(this, new[] { Subject }.OfType<ISuccess<T>>().Select(s => s.Value));
        }
        public AndWhichConstraint<ResultAssertions<T, F>, F> BeFailure()
        {
            Execute.Assertion.ForCondition(Subject is IFailure<F>)
                .FailWith("subject is not a failure value");
            return new AndWhichConstraint<ResultAssertions<T, F>, F>(this, new[] { Subject }.OfType<IFailure<F>>().Select(s => s.Error));
        }
        protected override string Identifier => "Result";
    }
}
