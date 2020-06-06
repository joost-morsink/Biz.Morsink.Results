using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.Results;
using System;

namespace Biz.Morsink.Results.Test
{
    [TestClass]
    public class ResultTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            var res = IsOdd(42);
            res.AssertFailure(msg => Assert.AreEqual("Number is not odd", msg));
            res = IsOdd(5);
            res.AssertSuccess(v => Assert.AreEqual(5, v));
        }

        public Result<int, string> IsOdd(int i)
        {
            var res = Result.For<int, string>();
            return i % 2 == 0 ? res.Failure("Number is not odd") : res.Success(i);
        }
    }
    public static class TestUtils
    {
        public static void AssertSuccess<T, E>(this Result<T, E> result, Action<T> act)
        {
            result.Act(act, e => Assert.Fail(e.ToString()));
        }
        public static void AssertFailure<T, E>(this Result<T, E> result, Action<E> act)
        {
            result.Act(v => Assert.Fail($"Value ({v}) found, error expected"), act);
        }
    }
}
