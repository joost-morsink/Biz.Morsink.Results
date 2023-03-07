namespace Biz.Morsink.ValidObjects.Test;

public class NatTest
{
    [Test]
    public void ValueTest()
    {
        Zero.Value.Should().Be(0);
        One.Value.Should().Be(1);
        Twenty<Four>.Value.Should().Be(24);
        One<Hundred<Seven>>.Value.Should().Be(107);
        Six<Hundred<Eighty<Five>>>.Value.Should().Be(685);
        Nine<Thousand<Two<Hundred<Thirty<Six>>>>>.Value.Should().Be(9236);
        One<Million>.Value.Should().Be(1000000);
        One<Million<Forty<Eight<Thousand<Five<Hundred<Seventy<Six>>>>>>>>.Value.Should().Be(1048576);
        Forty<Million<Three<Hundred<Twelve<Thousand<Four<Hundred<Seventy<Eight>>>>>>>>>.Value.Should().Be(40312478);
        Two<Billion<Four<Million<Fifteen<Thousand<Ninety<One>>>>>>>.Value.Should().Be(2004015091);
    }
}
