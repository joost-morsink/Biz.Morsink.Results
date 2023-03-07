using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Biz.Morsink.Results.Test
{
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
