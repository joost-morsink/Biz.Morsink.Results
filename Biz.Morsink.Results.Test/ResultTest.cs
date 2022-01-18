using Biz.Morsink.Results.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.Results.Errors;
using FluentAssertions;

namespace Biz.Morsink.Results.Test
{
    [TestClass]
    public class ResultTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            var res = IsOdd(42);
            res.Should().BeFailure()
                .Which.Should().Be("Number is not odd");
            res = IsOdd(5);
            res.Should().BeSuccess().Which.Should().Be(5);
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
            r.Should().BeFailure()
                .Which.Should().HaveCount(1)
                .And.ContainSingle(x => x.Key.ToString() == "abc.def");

            r = IsOdd(42).StringToError().Prefix("abc","def");
            r.Should().BeFailure()
                .Which.Should().HaveCount(1)
                .And.ContainSingle(x => x.Key.ToString() == "abc.def");
        }
    }
}
