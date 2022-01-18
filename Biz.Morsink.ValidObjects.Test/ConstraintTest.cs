using System.Collections.Immutable;
namespace Biz.Morsink.ValidObjects.Test;

public class ConstraintTest
{
    [Test]
    public void EmptyStringTest()
    {
        Valid<string, NotEmpty>.TryCreate("   ").Should().BeFailure()
            .Which.Should().HaveCount(1);
        Valid<string, NotEmpty>.TryCreate("").Should().BeFailure()
            .Which.Should().HaveCount(1);
        Valid<string, NotEmpty>.TryCreate("\t\t\t").Should().BeFailure()
            .Which.Should().HaveCount(1);
        Valid<string, NotEmpty>.TryCreate("\n\n\n").Should().BeFailure()
            .Which.Should().HaveCount(1);
        Valid<string, NotEmpty>.TryCreate(" \n").Should().BeFailure()
            .Which.Should().HaveCount(1);
        Valid<string, NotEmpty>.TryCreate("x").Should().BeSuccess()
            .Which.Value.Should().Be("x");
    }
    [Test]
    public void EmptyListTest()
    {
        Valid<ImmutableList<int>, NotEmpty<int>>.TryCreate(ImmutableList<int>.Empty).Should().BeFailure()
            .Which.Should().HaveCount(1);
        Valid<ImmutableList<int>, NotEmpty<int>>.TryCreate(ImmutableList.Create(1, 2, 3)).Should().BeSuccess()
            .Which.Value.Should().HaveCount(3)
            .And.ContainSingle(x => x == 1)
            .And.ContainSingle(x => x == 2)
            .And.ContainSingle(x => x == 3);
    }
    [Test]
    public void MinValueTest()
    {
        var atLeastPlusFive = MinValue<Plus<Five>>.Instance;
        atLeastPlusFive.Check(-4).Should().BeFailure();
        atLeastPlusFive.Check(0).Should().BeFailure();
        atLeastPlusFive.Check(4).Should().BeFailure();
        for(int i=5; i<20; i++)
            atLeastPlusFive.Check(i).Should().BeSuccess();
        atLeastPlusFive.Check(555).Should().BeSuccess();

        var atLeastMinusFive = MinValue<Minus<Five>>.Instance;
        for (int i = -5; i <= 10; i++)
            atLeastMinusFive.Check(i).Should().BeSuccess();
        atLeastMinusFive.Check(-6).Should().BeFailure();
        atLeastMinusFive.Check(-555).Should().BeFailure();
    }
    [Test]
    public void MaxValueTest()
    {
        var atMostPlusFive = MaxValue<Plus<Five>>.Instance;
        for (int i = -10; i <= 5; i++)
            atMostPlusFive.Check(i).Should().BeSuccess();
        atMostPlusFive.Check(555).Should().BeFailure();

        var atMostMinusFive = MaxValue<Minus<Five>>.Instance;
        atMostMinusFive.Check(4).Should().BeFailure();
        atMostMinusFive.Check(0).Should().BeFailure();
        atMostMinusFive.Check(-4).Should().BeFailure();
        for (int i = -100; i <= -5; i++)
            atMostMinusFive.Check(i).Should().BeSuccess();
    }
    [Test]
    public void IntervalTest()
    {
        var betweenMinusSixAndThree = ValueBetween<Minus<Six>, Plus<Three>>.Instance;
        for (int i = -100; i <= -7; i++)
            betweenMinusSixAndThree.Check(i).Should().BeFailure();
        for (int i = -6; i <= 3; i++)
            betweenMinusSixAndThree.Check(i).Should().BeSuccess();
        for (int i = 4; i <= 100; i++)
            betweenMinusSixAndThree.Check(i).Should().BeFailure();
    }

    [Test]
    public void DisjointIntervalTest()
    {
        var validator = Or<int, ValueBetween<Minus<Two>, Plus<Nine>>, ValueBetween<Plus<Thirty<Seven>>, Plus<Fifty<One>>>>.Instance;

        validator.Check(-3).Should().BeFailure();
        for (int i = -2; i <= 9; i++)
            validator.Check(i).Should().BeSuccess();
        for (int i = 10; i < 37; i++)
            validator.Check(i).Should().BeFailure();
        for (int i = 37; i <= 51; i++)
            validator.Check(i).Should().BeSuccess();
        validator.Check(52).Should().BeFailure();
    }

    [Test]
    public void IntersectionTest()
    {
        var validator = And<int, MinValue<Zero>, MaxValue<Plus<Five>>>.Instance;
        validator.Check(-1).Should().BeFailure();
        for (int i = 0; i <= 5; i++)
            validator.Check(i).Should().BeSuccess();
        validator.Check(6).Should().BeFailure();
    }
}
