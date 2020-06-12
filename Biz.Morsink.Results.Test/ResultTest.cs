using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;

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

        [TestMethod]
        public void ErrorKeyTest()
        {
            var r = IsOdd(42).StringToError().Prefix("def").Prefix("abc");
            r.AssertFailure(f =>
            {
                Assert.AreEqual(1, f.Count);
                Assert.AreEqual("abc.def", f[0].Key.ToString());
            });
            r = IsOdd(42).StringToError().Prefix("abc","def");
            r.AssertFailure(f =>
            {
                Assert.AreEqual(1, f.Count);
                Assert.AreEqual("abc.def", f[0].Key.ToString());
            });
        }
        
    }
}
