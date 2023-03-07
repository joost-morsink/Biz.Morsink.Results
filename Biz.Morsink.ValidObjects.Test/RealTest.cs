namespace Biz.Morsink.ValidObjects.Test;

public class RealTest
{
    [Test]
    public void TestValue()
    {
        Point<Fourteen<Thousand<One<Hundred<Fifty<Nine>>>>>>.Fraction.Should().Be(0.14159m);
        Real<Plus<Three>, Point<Fourteen<Thousand<One<Hundred<Fifty<Nine>>>>>>>.Value.Should().Be(3.14159m);
        Real<Plus<Three>, Point<Fourteen, One<Hundred<Fifty<Nine>>>>>.Value.Should().Be(3.14159m);
        Real<Minus<Two>, Point<Seventy<One>, Eighty<Two>, Eight>>.Value.Should().Be(-2.71828m);
    }    
}
