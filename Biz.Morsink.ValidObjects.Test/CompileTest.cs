using System.Collections.Generic;
using System.Collections.Immutable;
using Biz.Morsink.Results;

namespace Biz.Morsink.ValidObjects.Test;

using NonEmptyString = Valid<string, NotEmpty>;
using ZipCodeString = Valid<string, DutchZipCode>;
using NaturalNumber = Valid<int, MinValue<Zero>>;
using AgeNumber = Valid<int,ValueBetween<Zero, Plus<One<Hundred<Twenty>>>>>;
using NonEmptyString64 = Valid<string, And<string, NotEmpty, MaxLength<Sixty<Four>>>>;
public class DutchZipCode : RegexConstraint
{
    public static DutchZipCode Instance { get; } = new();

    public DutchZipCode() : base("^[1-9][0-9]{3}[A-Z]{2}$")
    {
    }
}

public class Identifier : RegexConstraint
{
    public static Identifier Instance { get; } = new();

    public Identifier() : base("^[A-Za-z_][A-Za-z_0-9]*$")
    {
    }
}

[ValidObject(Mutable = true)]
public partial class Address
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
}

[ValidObject(Mutable = true)]
public partial class Person
{
    public NonEmptyString64 FirstName { get; }
    public NonEmptyString LastName { get; }
    public AgeNumber Age { get; }
    public Address MainAddress { get; }
    public ImmutableList<Address> Addresses { get; }
    public IImmutableSet<Valid<string, Identifier>> Tags { get; }

    [ValidationMethod]
    private IEnumerable<string> Check()
    {
        if (Equals(FirstName, LastName))
            yield return "First and lastnames should be different.";
    }
}
[ValidObject]
public partial class AddressImm
{
    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
}

[ValidObject]
public partial class PersonImm
{
    public NonEmptyString64 FirstName { get; }
    public NonEmptyString LastName { get; }
    public AgeNumber Age { get; }
    public AddressImm MainAddress { get; }
    public ImmutableList<AddressImm> Addresses { get; }
    public IImmutableSet<Valid<string, Identifier>> Tags { get; }

    [ValidationMethod]
    private IEnumerable<string> Check()
    {
        if (Equals(FirstName, LastName))
            yield return "First and lastnames should be different.";
    }
}

[ValidObject]
public partial class DictContainer
{
    public ImmutableDictionary<string, string> Regular { get; }
    public ImmutableSortedDictionary<Valid<string, Identifier>, Valid<string, NotEmpty>> Both { get; }
    public IImmutableDictionary<string, Valid<string, NotEmpty>> Value { get; }
    public ImmutableList<string> Strings { get; }
}

public struct Natural
{
    private readonly Valid<int, MinValue<Zero>> _value;

    public Natural(Valid<int, MinValue<Zero>> value)
    {
        _value = value;
    }

    public int Value => _value.Value;

    public static implicit operator Natural(Valid<int, MinValue<Zero>> value)
        => new(value);

    public static implicit operator Valid<int, MinValue<Zero>>(Natural value)
        => value._value;

    public static implicit operator int(Natural value)
        => value.Value;

    public static Natural operator +(Natural x, Natural y)
        => new(Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());

    public static Natural operator *(Natural x, Natural y)
        => new(Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());
}

public class CompileTest
{
    private Person.Mutable GetTestPerson()
        => new ()
        {
            FirstName = "Joost",
            LastName = "Morsink",
            Age = 43,
            MainAddress =
            {
                Street = "Teststraat",
                HouseNumber = "1",
                ZipCode = "1234AB",
                City = "Teststad"
            },
            Addresses =
            {
                new ()
                {
                    Street = "Teststraat",
                    HouseNumber = "1",
                    ZipCode = "1234AB",
                    City = "Teststad"
                }
            }
        };

    [Test]
    public void ValidDataShouldResultInValidObject()
    {
        var p = GetTestPerson();
        p.FirstName = "Joost-Willem";
        p.Age = 105;
        p.ValidObject.Should().NotBeNull();
        p.Errors.Should().HaveCount(0);
        p.ValidObject!.Tags.Should().BeEmpty();
    }

    [Test]
    public void DifferentPathsToSameObjectShouldBeSameReference()
    {
        var p = GetTestPerson();

        p.Addresses[0].ValidObject.Should().Be(p.ValidObject!.Addresses[0]);
        p.Addresses[0].ValidObject.Should().BeSameAs(p.ValidObject!.Addresses[0]);
        p.Value!.Addresses[0].Should().BeSameAs(p.Addresses[0]);
    }

    [Test]
    public void ChangingOneReferencesPropertyShouldNotChangeAnotherValidatedReference()
    {
        var p = GetTestPerson();
        p.IsValid.Should().BeTrue();
        var mainAddress = p.ValidObject!.MainAddress;
        p.Addresses[0].City = "Testdorp";

        p.ValidObject.MainAddress.Should().BeSameAs(mainAddress);
    }

    [Test]
    public void ChangingToInvalidDataShouldPropagateErrors()
    {
        var p = GetTestPerson();
        p.IsValid.Should().BeTrue();

        p.Addresses[0].ZipCode = "12345";
        p.IsValid.Should().BeFalse();
        p.Errors.Should().Contain(e => e.Key.ToString() == "Addresses.0.ZipCode");
        p.MainAddress.IsValid.Should().BeTrue();
        p.Addresses[0].IsValid.Should().BeFalse();
    }

    [Test]
    public void SetElementsShouldBeValidatedCorrectly()
    {
        var p = GetTestPerson();
        p.IsValid.Should().BeTrue();

        p.Tags.Add("test");
        p.IsValid.Should().BeTrue();
        p.Tags.Add("foute test");
        p.IsValid.Should().BeFalse();
    }
    [Test]
    public void SetElementsShouldBeValidatedCorrectlyWhenUsingMutables()
    {
        var p = GetTestPerson();
        p.IsValid.Should().BeTrue();

        var vc = "test".Constrain().With<Identifier>().GetMutable();
        p.Tags.Add(vc);
        p.IsValid.Should().BeTrue();
        vc.Value = "foute test";
        var result = p.AsResult();
        p.IsValid.Should().BeFalse();
    }

    [Test]
    public void AddressTest()
    {
        var p =
            new Address.Mutable()
            {
                Street = "Teststraat",
                HouseNumber = "1",
                ZipCode = "1234AB",
                City = "Teststad"
            };
        var vo = p.ValidObject!;
        p.ValidObject.Should().NotBeNull();
        p.Errors.Should().HaveCount(0);
        
        p.ZipCode = "12345";
        p.IsValid.Should().BeFalse();
        p.Errors.Should().Contain(e => e.Key.ToString() == "ZipCode");

        p.ZipCode = "1234AB";
        p.ValidObject.Should().BeEquivalentTo(vo);
        p.ValidObject.Should().NotBe(vo);
    }

    [Test]
    public void ImmutableTest()
    {
        var p = new PersonImm.Dto
        {
            FirstName = "Joost",
            LastName = "Morsink",
            Age = 43,
            MainAddress = new()
            {
                Street = "Teststraat",
                HouseNumber = "1",
                ZipCode = "1234AB",
                City = "Teststad"
            }
        };
        p = p with {Addresses = p.Addresses.Add(p.MainAddress)};
        p.TryCreate().Should().BeSuccess();
    }
}
