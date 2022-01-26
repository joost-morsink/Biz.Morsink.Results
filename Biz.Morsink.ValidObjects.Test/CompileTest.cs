using System.Collections.Immutable;
using Biz.Morsink.Results;
using Biz.Morsink.Results.Errors;
using NUnit.Framework.Constraints;
using RegexConstraint=Biz.Morsink.ValidObjects.Constraints.RegexConstraint;
namespace Biz.Morsink.ValidObjects.Test;

using NonEmptyString = Valid<string, NotEmpty>;
using ZipCodeString = Valid<string, DutchZipCode>;
using NaturalNumber = Valid<int, MinValue<Zero>>;

public class DutchZipCode : RegexConstraint
{
    public static DutchZipCode Instance { get; } = new ();
    public DutchZipCode() : base("^[0-9]{4}[A-Z]{2}$")
    {
    }
}
public class Identifier : RegexConstraint
{
    public static Identifier Instance { get; } = new ();
    public Identifier() : base("^[A-Za-z_][A-Za-z_0-9]*")
    {
    }
}
[Generate]
public partial class Address
    #if !GENERATE
    : IValidObject<Address, Address.Dto>, IHasStaticValidator<Address, Address.Dto>
    #endif
{
    #if !GENERATE
    private Address(NonEmptyString street, NonEmptyString houseNumber, ZipCodeString zipCode, NonEmptyString city)
    {
        Street = street;
        HouseNumber = houseNumber;
        ZipCode = zipCode;
        City = city;
    }
    public static IObjectValidator<Address, Dto> Validator { get; } = ObjectValidator.For<Address,Dto>();
    #endif

    public NonEmptyString Street { get; }
    public NonEmptyString HouseNumber { get; }
    public ZipCodeString ZipCode { get; }
    public NonEmptyString City { get; }
    #if !GENERATE
    public record Dto(string Street, string HouseNumber, string ZipCode, string City) : IDto<Address,Dto>
    {
        public Result<Address, ErrorList> TryCreate()
            => (NonEmptyString.TryCreate(Street).Prefix(nameof(Street)),
                    NonEmptyString.TryCreate(HouseNumber).Prefix(nameof(HouseNumber)),
                    ZipCodeString.TryCreate(ZipCode).Prefix(nameof(ZipCode)),
                    NonEmptyString.TryCreate(City).Prefix(nameof(City)))
                .Apply((street, houseNumber, zipCode, city)
                    => new Address(street, houseNumber, zipCode, city));
    }
    public Dto GetDto()
        => new (Street.GetDto(), HouseNumber.GetDto(), ZipCode.GetDto(), City.GetDto());
    #endif
}
[Generate]
public partial class Person
    #if !GENERATE
    : IValidObject<Person, Person.Dto>, IHasStaticValidator<Person, Person.Dto>
    #endif
{
    #if !GENERATE
    public static IObjectValidator<Person, Dto> Validator { get; } = ObjectValidator.For<Person, Dto>();
    private Person(NonEmptyString firstName, NonEmptyString lastName, NaturalNumber age, ImmutableList<Address> addresses)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Addresses = addresses;
    }
    #endif
    
    public NonEmptyString FirstName { get; }
    public NonEmptyString LastName { get; }
    public NaturalNumber Age { get; }
    public ImmutableList<Address> Addresses { get; }
    public IImmutableSet<Valid<string, Identifier>> Tags { get; }
    #if !GENERATE
    public Dto GetDto()
        => new (FirstName.GetDto(), LastName.GetDto(), Age.GetDto(), Address.Validator.ToListValidator().GetDto(Addresses));

    public record Dto(string FirstName, string LastName, int Age, ImmutableList<Address.Dto> Addresses) : IDto<Person, Dto>
    {
        public Result<Person, ErrorList> TryCreate()
            => (NonEmptyString.TryCreate(FirstName).Prefix(nameof(FirstName)),
                    NonEmptyString.TryCreate(LastName).Prefix(nameof(LastName)),
                    NaturalNumber.TryCreate(Age).Prefix(nameof(Age)),
                    Address.Validator.ToListValidator().TryCreate(Addresses).Prefix(nameof(Addresses)))
                .Apply((firstName, lastName, age, addresses) =>
                    new Person(firstName, lastName, age, addresses));
    }
    #endif
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
        => new (value);
    public static implicit operator Valid<int, MinValue<Zero>>(Natural value)
        => value._value;
    public static implicit operator int(Natural value)
        => value.Value;
    
    public static Natural operator +(Natural x, Natural y)
        => new (Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());
    public static Natural operator *(Natural x, Natural y)
        => new (Valid<int, MinValue<Zero>>.TryCreate(x.Value + y.Value).GetOrThrow());
}

// [Generate]
// public partial class GenerationTest
// {
//     public void Test()
//     {
//         Console.WriteLine(Hallo);
//     }
// }